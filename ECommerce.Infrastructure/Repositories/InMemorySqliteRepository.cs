using ECommerce.Application.Interfaces.Persistence;
using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Repositories
{
    public class InMemorySqliteRepository  : IOrderRepository
    {
        private readonly ECommerceDbContext _dbContext;

        public InMemorySqliteRepository(ECommerceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Order order)
        {
            await _dbContext.Orders.AddAsync(order);
            await _dbContext.SaveChangesAsync();
        }

        public Task DeleteAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<Order> GetByIdAsync(string id)
        {
            return await _dbContext.Orders.FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task UpdateAsync(Order order)
        {
            var existingOrder = await _dbContext.Orders.FirstOrDefaultAsync(o => o.Id == order.Id);
            if (existingOrder == null)
            {
                throw new ArgumentException("Order not found.");
            }

            //replace
            _dbContext.Entry(existingOrder).CurrentValues.SetValues(order);

            await _dbContext.SaveChangesAsync();
        }
    }
}
