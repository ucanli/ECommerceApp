using ECommerce.Application.DTOs.BalanceApi;

namespace ECommerce.Application.Interfaces.External
{
    public interface IBalanceService
    {
        Task<BalanceApiUserBalanceDto> GetUserBalanceAsync();
        Task<BalanceApiPreOrderDto> PrePrder(string orderId, decimal amount);
    }
}
