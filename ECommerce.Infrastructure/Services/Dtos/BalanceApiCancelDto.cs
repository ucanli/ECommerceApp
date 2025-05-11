using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Services.Dtos
{
    public class BalanceApiCancelDto
    {
        public BalanceApiOrderDto Order { get; set; }
        public BalanceApiUserBalanceDto UpdatedBalance { get; set; }
    }
}
