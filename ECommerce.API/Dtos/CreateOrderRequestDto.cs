using ECommerce.Application.Commands;

namespace ECommerce.API.Dtos
{
    public class CreateOrderRequestDto
    {
        public string UserId { get; set; }
        public List<OrderProductItemDto> Products { get; set; }
    }

    public class OrderProductItemDto
    {
        public string ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
