using ECommerce.Application.Commands;
using ECommerce.Application.Dtos;
using ECommerce.Application.Exceptions;
using ECommerce.Application.Interfaces.Concurrency;
using ECommerce.Application.Interfaces.External;
using ECommerce.Application.Interfaces.Persistence;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Services
{
    public class OrderManager : IOrderManager
    {
        private readonly IProductService _productService;
        private readonly IBalanceService _balanceService;
        private readonly IOrderRepository _orderRepository;
        private readonly ILockProvider _lockProvider;
        private readonly ILogger<OrderManager> _logger;

        public OrderManager(IProductService productService, IBalanceService balanceService, IOrderRepository orderRepository, ILogger<OrderManager> logger, ILockProvider lockProvider)
        {
            _productService = productService;
            _balanceService = balanceService;
            _orderRepository = orderRepository;
            _lockProvider = lockProvider;
            _logger = logger;
        }

        public async Task<OrderDto> CreateOrderAsync(CreateOrderCommand command)
        {
            OrderDto result = null;

            await _lockProvider.ExecuteWithLockAsync($"order-lock:{command.UserId}", async () =>
            {
                result = await CreateOrderInternalAsync(command);
            });

            return result;
        }

        private async Task<OrderDto> CreateOrderInternalAsync(CreateOrderCommand command)
        {
            var orderEntity = new Order();

            try
            {
                //check user has enough balance

                var userBalance = await _balanceService.GetUserBalanceAsync();

                if (userBalance == null || userBalance.AvailableBalance <= 0)
                {
                    throw new UserBalanceException("User doesnot have balance.");
                }

                var products = await _productService.GetProductsAsync();

                var orderEntityProducts = new List<Product>();

                foreach (var commandProductItem in command.Products)
                {
                    var product = products.FirstOrDefault(p => p.Id == commandProductItem.ProductId);
                    if (product == null || product.Stock < commandProductItem.Quantity)
                    {
                        throw new InvalidDataException($"Product not found or stock not enough: {commandProductItem.ProductId}");
                    }

                    orderEntityProducts.Add(new Product
                    {
                        ProductId = product.Id,
                        Name = product.Name,
                        Description = product.Description,
                        Currency = product.Currency,
                        Price = product.Price,
                        Quantity = commandProductItem.Quantity,
                    });
                }


                orderEntity.UserId = command.UserId;
                orderEntity.Products = orderEntityProducts;
                orderEntity.Status = Domain.Enums.OrderStatus.Created;
                orderEntity.CreatedAt = DateTime.UtcNow;
                

                if (userBalance.AvailableBalance < orderEntity.TotalAmount)
                {
                    throw new UserBalanceException("user balance not enough for total amount");
                }

                //Creates a pre-order and blocks the specified amount
                var preOrder = await _balanceService.PreOrderAsync(orderEntity.Id, orderEntity.TotalAmount);

                if (preOrder == null)
                {
                    throw new PreOrderException("pre order not successful");
                }

                orderEntity.Status = preOrder.PreOrder.Status;

                await _orderRepository.AddAsync(orderEntity);

                var orderDto = new OrderDto
                {
                    Amount = orderEntity.TotalAmount,
                    OrderId = orderEntity.Id,
                    Status = orderEntity.Status,
                    Timestamp = orderEntity.CreatedAt,
                    UserId = orderEntity.UserId,
                };

                return orderDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error while creating order: {ex.Message}");

                try
                {
                    await RollbackOrderAsync(orderEntity.Id);
                }
                catch (Exception rollbackEx)
                {
                    _logger.LogError(rollbackEx, "Failed to rollback order creation.");
                }

                throw new ApplicationException($"Unexpected error while creating order: {ex.Message}", ex);
            }

        }

        private async Task RollbackOrderAsync(string orderId)
        {
            // Balance işlemini iptal et
            await _balanceService.CancelAsync(orderId);

            // orde sil veya statüsü update edilebilir!!
            await _orderRepository.DeleteAsync(orderId);

            _logger.LogInformation($"Order {orderId} rollbacked.");
        }


        public async Task<CompleteDto> CompleteOrderAsync(string orderId)
        {

            //await TestLockProvider();

            CompleteDto result = null;

            await _lockProvider.ExecuteWithLockAsync($"complete-lock:{orderId}", async () =>
            {
                result = await CompleteOrderInternalAsync(orderId);
            });

            return result;
        }

        private async Task<CompleteDto> CompleteOrderInternalAsync(string orderId)
        {
            try
            {
                var order = await _orderRepository.GetByIdAsync(orderId);
                if (order == null)
                {
                    throw new NotFoundException(nameof(order), $"{orderId}");
                }

                if (order.Status != OrderStatus.Blocked)
                {
                    throw new InvalidOperationException("Order is not in Blocked status to complete");
                }

                var completeDto = await _balanceService.CompleteAsync(orderId);

                if (completeDto == null)
                {
                    throw new ArgumentException("order complete not success");
                }

                order.Status = OrderStatus.Completed;

                await _orderRepository.UpdateAsync(order);

                return completeDto;
            }
            catch (NotFoundException ex)
            {
                _logger.LogError(ex, $"Unexpected error while complete order: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error while complete order: {ex.Message}");

                //TODO: business akışa göre rollback koşabilir veya başka bir akış olabilir?
                //await RollbackOrderAsync(orderEntity.Id); 

                throw new ApplicationException($"Unexpected error while complete order: {ex.Message}", ex);
            }
        }

        private async Task TestLockProvider()
        {

            var tasks = new Task[5];

            for (int i = 0; i < 5; i++)
            {
                int id = i;
                tasks[i] = Task.Run(async () =>
                {
                    var key = "user-123";
                    try
                    {
                        await _lockProvider.ExecuteWithLockAsync(key, async () =>
                        {
                            Console.WriteLine($"[{id}] Lock acquired, processing...");
                            await Task.Delay(3000);
                            Console.WriteLine($"[{id}] Done.");
                        });
                    }
                    catch (TooManyRequestsException ex)
                    {
                        Console.WriteLine($"[{id}] Failed to acquire lock: {ex.Message}");
                    }
                });
            }

            await Task.WhenAll(tasks);
        }
    }
}
