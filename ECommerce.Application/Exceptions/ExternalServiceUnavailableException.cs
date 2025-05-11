
namespace ECommerce.Application.Exceptions
{
    public class ExternalServiceUnavailableException : Exception
    {
        public ExternalServiceUnavailableException(string message) : base(message) { }
    }
}
