

using ECommerce.Application.Dtos;

namespace ECommerce.Application.Interfaces.External
{
    public interface IBalanceService
    {
        Task<UserBalanceDto> GetUserBalanceAsync();
        Task<PreOrderDto> PreOrderAsync(string orderId, decimal amount);
        Task<CompleteDto> CompleteAsync(string orderId);
        Task<CancelDto> CancelAsync(string orderId);
    }
}
