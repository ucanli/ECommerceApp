using ECommerce.Application.DTOs;

namespace ECommerce.Application.Interfaces.Services
{
    public interface IProductManager
    {
        Task<List<ProductDto>> GetProductsAsync();
    }
}
