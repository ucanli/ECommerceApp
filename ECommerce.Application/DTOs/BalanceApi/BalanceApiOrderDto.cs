using ECommerce.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.DTOs.BalanceApi
{
    public class BalanceApiOrderDto
    {
        public string OrderId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Timestamp { get; set; }
        public OrderStatus Status { get; set; }
        public string UserId { get; set; }
        public DateTime CompletedAt { get; set; }
        public DateTime CancelledAt { get; set; }

    }
}
