using AutoMapper;
using ECommerce.Application.DTOs;
using ECommerce.Application.Interfaces.External;
using ECommerce.Infrastructure.Services.Dtos;
using Microsoft.Extensions.Logging;

using System.Text.Json;

namespace ECommerce.Infrastructure.Services
{
    public class ProductService : IProductService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ProductService> _logger;
        private readonly IMapper _mapper;

        public ProductService(IHttpClientFactory httpClientFactory, ILogger<ProductService> logger, IMapper mapper)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<List<ProductDto>> GetProductsAsync()
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

                    if (response?.Data != null)
                    {
                        var result = _mapper.Map<List<ProductDto>>(response.Data);
                        return result;
                    }
                }

                return new List<ProductDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to fetch products. ErrorMessage: {ex.Message}");
                return new List<ProductDto>();
            }
        }
    }
}
