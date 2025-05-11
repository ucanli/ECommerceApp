using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.DTOs.BalanceApi
{
    public class BalanceApiCompleteDto
    {
        public BalanceApiOrderDto Order { get; set; }
        public BalanceApiUserBalanceDto UpdatedBalance { get; set; }
    }
}
