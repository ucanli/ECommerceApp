using ECommerce.Application.Commands;
using ECommerce.Application.Dtos;
using ECommerce.Application.Interfaces.External;
using ECommerce.Application.Interfaces.Persistence;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
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
        //private readonly IMapper _mapper;

        public OrderManager(IProductService productService, IBalanceService balanceService, IOrderRepository orderRepository)
        {
            _productService = productService;
            _balanceService = balanceService;
            _orderRepository = orderRepository;
        }

        public async Task<OrderDto> CreateOrderAsync(CreateOrderCommand command)
        {
            //check user has enough balance

            var userBalance =  await _balanceService.GetUserBalanceAsync();


            if (userBalance == null || userBalance.AvailableBalance <= 0 )
            {
                throw new ArgumentException("user not has balance");
            }

            var products = await _productService.GetProductsAsync();

            var orderEntityProducts = new List<Product>();

            foreach (var commandProductItem in command.Products) 
            {
                var product = products.FirstOrDefault(p => p.Id == commandProductItem.ProductId);
                if (product == null || product.Stock < commandProductItem.Quantity)
                {
                    throw new ArgumentException($"Product not found or stock insufficient: {commandProductItem.ProductId}");
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

            var orderEntity = new Order()
            {
                UserId = command.UserId,
                Products = orderEntityProducts,
                Status = Domain.Enums.OrderStatus.Created,
                CreatedAt = DateTime.UtcNow,
            };

            if (userBalance.AvailableBalance < orderEntity.TotalAmount)
            {
                throw new ArgumentException("user balance not enough for total amount");
            }

            //Creates a pre-order and blocks the specified amount
            var preOrder = await _balanceService.PreOrderAsync(orderEntity.Id, orderEntity.TotalAmount);

            if (preOrder == null) {
                throw new ArgumentException("pre order not successful");
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

        public async Task<CompleteDto> CompleteOrderAsync(string orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                throw new ArgumentException("Order not found");
            }

            if (order.Status != OrderStatus.Blocked)
            {
                throw new InvalidOperationException("Order is not in reserved");
            }

            var completeDto = await _balanceService.CompleteAsync(orderId);

            if (completeDto == null)
            {
                throw new ArgumentException("order complate not successful");
            }

            order.Status = OrderStatus.Completed;

            await _orderRepository.UpdateAsync(order);

            return completeDto;
        }
    }
}
