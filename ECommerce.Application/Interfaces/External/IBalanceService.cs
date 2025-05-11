

using ECommerce.Application.Dtos;

namespace ECommerce.Application.Interfaces.External
{
    public interface IBalanceService
    {
        Task<UserBalanceDto> GetUserBalanceAsync();
        Task<PreOrderDto> PrePrder(string orderId, decimal amount);
        Task<CompleteDto> Complete(string orderId);
        Task<CancelDto> Cancel(string orderId);
    }
}
