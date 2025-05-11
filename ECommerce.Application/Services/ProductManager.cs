using ECommerce.Application.DTOs;
using ECommerce.Application.Exceptions;
using ECommerce.Application.Interfaces.External;
using ECommerce.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ECommerce.Application.Services
{
    public class ProductManager : IProductManager
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductManager> _logger;

        public ProductManager(IProductService productService, ILogger<ProductManager> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        public async Task<List<ProductDto>> GetProductsAsync()
        {
            try
            {
                var products = await _productService.GetProductsAsync();

                if (products == null || !products.Any())
                {
                    _logger.LogWarning("No products were returned by Balance API.");

                    throw new InvalidDataException("No products were returned by Balance API.");
                }

                return products;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Product API could not be reached.");
                throw new ExternalServiceUnavailableException("Product API could not be reached.");
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Product API response couldnot be parsed.");
                throw new ExternalServiceDataException("Product API response couldnot be parsed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while getting products.");
                throw new ApplicationException("Unexpected error while getting products.");
            }
        }
    }
}
