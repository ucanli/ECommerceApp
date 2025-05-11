using ECommerce.Domain.Entities;

namespace ECommerce.Application.Interfaces.Persistence
{
    public interface IOrderRepository
    {
        Task AddAsync(Order order);
        Task<Order> GetByIdAsync(string id);
        Task UpdateAsync(Order order);
        Task DeleteAsync(string id);
    }
}
