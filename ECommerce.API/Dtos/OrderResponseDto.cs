using ECommerce.Domain.Enums;

namespace ECommerce.API.Dtos
{
    public class OrderResponseDto
    {
        public string OrderId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Timestamp { get; set; }
        public OrderStatus Status { get; set; }
        public string UserId { get; set; }
    }
}
