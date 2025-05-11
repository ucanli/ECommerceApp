using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Dtos
{
    public class CancelDto
    {
        public OrderDto Order { get; set; }
        public UserBalanceDto UpdatedBalance { get; set; }
    }
}
