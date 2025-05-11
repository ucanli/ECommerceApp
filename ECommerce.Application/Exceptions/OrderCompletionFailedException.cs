

namespace ECommerce.Application.Exceptions
{

    public class OrderCompletionFailedException : Exception
    {
        public OrderCompletionFailedException(string message) : base(message) { }
    }
}
