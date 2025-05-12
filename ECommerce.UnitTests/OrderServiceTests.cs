using ECommerce.Application.Commands;
using ECommerce.Application.Dtos;
using ECommerce.Application.DTOs;
using ECommerce.Application.Exceptions;
using ECommerce.Application.Interfaces.Concurrency;
using ECommerce.Application.Interfaces.External;
using ECommerce.Application.Interfaces.Persistence;
using ECommerce.Application.Services;
using ECommerce.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ECommerce.UnitTests
{
    public class OrderServiceTests
    {
        private readonly Mock<IProductService> _productServiceMock;
        private readonly Mock<IBalanceService> _balanceServiceMock;
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly OrderManager _orderManager;
        private readonly Mock<ILogger<OrderManager>> _loggerMock;
        private readonly Mock<ILockProvider> _lockProvideMock;


        public OrderServiceTests()
        {
            _balanceServiceMock = new Mock<IBalanceService>();
            _productServiceMock = new Mock<IProductService>();
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _lockProvideMock = new Mock<ILockProvider>();
            _loggerMock = new Mock<ILogger<OrderManager>>();

            LockProvider lockProvider = new LockProvider();

            _orderManager = new OrderManager(
                _productServiceMock.Object,
                _balanceServiceMock.Object,
                _orderRepositoryMock.Object,
                _loggerMock.Object,
                lockProvider);
        }


        [Fact]
        public async Task CreateOrderAsync_ShouldSucceed_WhenValidRequest()
        {
            // Arrange
            var request = new CreateOrderCommand
            {
                UserId = "user-123",
                Products = new List<OrderProductItem>
                {
                    new OrderProductItem(){ ProductId = "p1", Quantity = 2}
                }
            };

            _productServiceMock.Setup(x => x.GetProductsAsync())
                .ReturnsAsync(new List<ProductDto> {
                new() { Id = "p1", Price = 100, Name = "Test", Stock = 5, Currency = "TRY" }
                });

            _balanceServiceMock.Setup(x => x.PreOrderAsync(It.IsAny<string>(), It.IsAny<decimal>()))
                .ReturnsAsync(new PreOrderDto { 
                    PreOrder = new OrderDto
                    {
                        Amount = 100,
                        
                    }
                });

            _balanceServiceMock.Setup(x => x.GetUserBalanceAsync()).ReturnsAsync(new UserBalanceDto
            {
                UserId = "user-123",
                AvailableBalance = 10000,
                BlockedBalance = 0,
                TotalBalance = 10000,
            });



            // Act
            var result = await _orderManager.CreateOrderAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.OrderId);
            Assert.NotEmpty(result.OrderId);
            _orderRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Order>()), Times.Once);
        }


        [Fact]
        public async Task CreateOrderAsync_ShouldCreateOrder_WhenProductsAreValid()
        {
            var productIds = new List<string> { "1", "2" };
            var products = new List<ProductDto>
        {
            new() { Id = "1", Price = 100, Stock = 1},
            new() { Id = "2", Price = 200 , Stock = 1}
        };

            _productServiceMock.Setup(p => p.GetProductsAsync())
                .ReturnsAsync(products);


            _balanceServiceMock.Setup(x => x.PreOrderAsync(It.IsAny<string>(), It.IsAny<decimal>()))
                .ReturnsAsync(new PreOrderDto
                {
                    PreOrder = new OrderDto
                    {
                        Amount = 300,

                    }
                });

            _orderRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Order>()))
                .Returns(Task.CompletedTask);


            _balanceServiceMock.Setup(x => x.GetUserBalanceAsync()).ReturnsAsync(new UserBalanceDto
            {
                UserId = "user-123",
                AvailableBalance = 10000,
                BlockedBalance = 0,
                TotalBalance = 10000,
            });

            var request = new CreateOrderCommand
            {
                UserId = "user-123",
                Products = new List<OrderProductItem>
                {
                    new (){ ProductId = "1", Quantity = 1},
                    new (){ ProductId = "2", Quantity = 1}

                }
            };


            var order = await _orderManager.CreateOrderAsync(request);


            // Assert
            Assert.NotNull(order);
            Assert.Equal(300, order.Amount);
            _balanceServiceMock.Verify(b => b.PreOrderAsync(order.OrderId, 300), Times.Once);
        }


        [Fact]
        public async Task CreateOrderAsync_ShouldThrowException_WhenNoStock()
        {
            // Arrange
            var productIds = new List<string> { "1" };
            var products = new List<ProductDto> { new() { Id = "1", Price = 150, Stock = 0 } };

            _productServiceMock.Setup(p => p.GetProductsAsync())
                .ReturnsAsync(products);

            _balanceServiceMock.Setup(x => x.PreOrderAsync(It.IsAny<string>(), It.IsAny<decimal>()))
         .ReturnsAsync(new PreOrderDto
         {
             PreOrder = new OrderDto
             {
                 Amount = 300,

             }
         });

            var orderRequestCreateDto = new CreateOrderCommand
            {
                UserId = "user-123",
                Products = new List<OrderProductItem>
              {
                  new() { ProductId = "1", Quantity = 1 }
              },
            };

            _balanceServiceMock.Setup(x => x.GetUserBalanceAsync()).ReturnsAsync(new UserBalanceDto
            {
                UserId = "user-123",
                AvailableBalance = 1,
                BlockedBalance = 0,
                TotalBalance = 0,
            });

            await Assert.ThrowsAsync<ApplicationException>(() => _orderManager.CreateOrderAsync(orderRequestCreateDto));
        }
    }
}
