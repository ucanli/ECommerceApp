using ECommerce.Application.DTOs;
using ECommerce.Application.Interfaces.External;
using ECommerce.Application.Interfaces.Services;

namespace ECommerce.Application.Services
{
    public class ProductManager : IProductManager
    {
        private readonly IProductService _productService;

        public ProductManager(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<List<ProductDto>> GetProductsAsync()
        {
            var result = await _productService.GetProductsAsync();
            return result;
        }
    }
}
