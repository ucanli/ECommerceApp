using ECommerce.Application.DTOs;
using ECommerce.Application.DTOs.BalanceApi;
using ECommerce.Application.Interfaces.External;
using Microsoft.Extensions.Logging;

using System.Text.Json;

namespace ECommerce.Infrastructure.Services
{
    public class ProductService : IProductService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IHttpClientFactory httpClientFactory, ILogger<ProductService> logger)
        {
            _httpClientFactory = httpClientFactory;

            _logger = logger;
        }

        public async Task<List<BalanceApiProductDto>> GetProductsAsync()
        {
            try
            {
                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "api/products");

                var httpClient = _httpClientFactory.CreateClient("balance-management-api");
                var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    await using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var response = await JsonSerializer.DeserializeAsync<ApiResponse<List<BalanceApiProductDto>>>(contentStream, options);
                    return response?.Data ?? new List<BalanceApiProductDto>();
                }

                return new List<BalanceApiProductDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to fetch products. ErrorMessage: {ex.Message}");
                return new List<BalanceApiProductDto>();
            }
        }
    }
}
