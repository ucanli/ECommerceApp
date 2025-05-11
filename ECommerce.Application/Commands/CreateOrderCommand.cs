

namespace ECommerce.Application.Commands
{
    public class CreateOrderCommand
    {
        public string UserId { get; set; }
        public List<OrderProductItem> Products { get; set; }
    }


    public class OrderProductItem
    {
        public string ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
