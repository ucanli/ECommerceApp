using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Dtos
{
    public class PreOrderDto
    {
        public OrderDto PreOrder { get; set; }
        public UserBalanceDto UpdatedBalance { get; set; }
    }
}
