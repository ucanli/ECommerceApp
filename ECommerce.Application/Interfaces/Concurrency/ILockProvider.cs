

namespace ECommerce.Application.Interfaces.Concurrency
{
    public interface ILockProvider
    {
        Task ExecuteWithLockAsync(string key, Func<Task> action);
    }
}
