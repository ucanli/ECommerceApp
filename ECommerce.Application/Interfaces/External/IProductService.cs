using ECommerce.Application.DTOs.BalanceApi;

namespace ECommerce.Application.Interfaces.External
{
    public interface IProductService
    {
        Task<List<BalanceApiProductDto>> GetProductsAsync();
    }
}
