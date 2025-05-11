using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.DTOs.BalanceApi
{
    public class BalanceApiPreOrderDto
    {
        public BalanceApiOrderDto PreOrder { get; set; }
        public BalanceApiUserBalanceDto UpdatedBalance { get; set; }
    }
}
