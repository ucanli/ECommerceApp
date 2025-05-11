using ECommerce.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Entities
{
    public class Order
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; }
        public List<Product> Products { get; set; } = new();
        public decimal TotalAmount => Products.Sum(i => i.Price * i.Quantity);
        public OrderStatus Status { get; set; } = OrderStatus.Created;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
