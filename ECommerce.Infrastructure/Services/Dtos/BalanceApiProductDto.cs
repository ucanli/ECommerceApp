using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Services.Dtos
{
    public class BalanceApiProductDto
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public string Category { get; set; }
        public int Stock { get; set; }
    }
}
