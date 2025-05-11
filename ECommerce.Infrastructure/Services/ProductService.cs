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
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "api/products");
            var httpClient = _httpClientFactory.CreateClient("balance-management-api");

            try
            {
                var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Balance API failed with status code {httpResponseMessage.StatusCode}");
                }

                await using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var response = await JsonSerializer.DeserializeAsync<ApiResponse<List<BalanceApiProductDto>>>(contentStream, options);

                if (response?.Success == false || response?.Data == null)
                {
                    return new List<ProductDto>();
                }

                return _mapper.Map<List<ProductDto>>(response.Data);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request error while fetching products.");
                throw;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON deserialization error while fetching products.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while fetching products.");
                throw;
            }
        }

    }
}
