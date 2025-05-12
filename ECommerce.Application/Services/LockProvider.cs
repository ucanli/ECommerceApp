using ECommerce.Application.Exceptions;
using ECommerce.Application.Interfaces.Concurrency;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Application.Services
{
    public class LockProvider : ILockProvider
    {
        private readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();

        public async Task ExecuteWithLockAsync(string key, Func<Task> action)
        {
            var semaphore = _locks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
 
            if (semaphore.CurrentCount == 0)
            {
                throw new TooManyRequestsException("Another request is already being processed.");
            }

            await semaphore.WaitAsync();
            try
            {
                await action();
            }
            finally
            {
                semaphore.Release();

                //temizle
                if (semaphore.CurrentCount == 1)
                {
                    _locks.TryRemove(key, out _);
                }
            }
        }
    }
}
