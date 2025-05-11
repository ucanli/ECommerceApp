using ECommerce.Application.Commands;
using ECommerce.Application.Dtos;

namespace ECommerce.Application.Interfaces.Services
{
    public interface IOrderManager
    {
        Task<OrderDto> CreateOrderAsync(CreateOrderCommand command);
    }
}
