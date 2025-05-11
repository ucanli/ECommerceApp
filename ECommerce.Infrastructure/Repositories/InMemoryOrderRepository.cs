using ECommerce.Application.Interfaces.Persistence;
using ECommerce.Domain.Entities;

namespace ECommerce.Infrastructure.Repositories
{
    public class InMemoryOrderRepository : IOrderRepository
    {
        private readonly Dictionary<string, Order> _orders = new();

        public Task AddAsync(Order order)
        {
            _orders[order.Id] = order;
            return Task.CompletedTask;
        }

        public Task<Order> GetByIdAsync(string id)
        {
            _orders.TryGetValue(id, out var order);
            return Task.FromResult(order);
        }

        public Task UpdateAsync(Order order)
        {
            _orders[order.Id] = order;
            return Task.CompletedTask;
        }
    }
}
