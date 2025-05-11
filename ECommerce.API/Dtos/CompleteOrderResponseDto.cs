using ECommerce.Application.Dtos;

namespace ECommerce.API.Dtos
{
    public class CompleteOrderResponseDto
    {
        public OrderResponseDto Order { get; set; }
        public UserBalanceResponseDto UpdatedBalance { get; set; }
    }

    public class UserBalanceResponseDto
    {
        public string UserId { get; set; }
        public decimal TotalBalance { get; set; }
        public decimal AvailableBalance { get; set; }
        public decimal BlockedBalance { get; set; }
        public string Currency { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
