using ECommerce.Application.DTOs;
using ECommerce.Application.DTOs.BalanceApi;
using ECommerce.Application.Interfaces.External;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Services
{
    public class BalanceService : IBalanceService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ProductService> _logger;

        public BalanceService(IHttpClientFactory httpClientFactory, ILogger<ProductService> logger)
        {
            _httpClientFactory = httpClientFactory;

            _logger = logger;
        }
        public async Task<BalanceApiUserBalanceDto> GetUserBalanceAsync()
        {
            try
            {
                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "api/balance");

                var httpClient = _httpClientFactory.CreateClient("balance-management-api");
                var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    await using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var response = await JsonSerializer.DeserializeAsync<ApiResponse<BalanceApiUserBalanceDto>>(contentStream, options);

                    return response?.Data ?? new BalanceApiUserBalanceDto();
                }

                return new BalanceApiUserBalanceDto();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to fetch products. ErrorMessage: {ex.Message}");
                return new BalanceApiUserBalanceDto();
            }
        }

        public async Task<BalanceApiPreOrderDto> PrePrder(string orderId, decimal amount)
        {
            try
            {
                var jsonReuest = JsonSerializer.Serialize(new { orderId, amount });

                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "api/balance/preorder");
                httpRequestMessage.Content = new StringContent(jsonReuest, Encoding.UTF8, "application/json");

                var httpClient = _httpClientFactory.CreateClient("balance-management-api");

                var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    await using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var response = await JsonSerializer.DeserializeAsync<ApiResponse<BalanceApiPreOrderDto>>(contentStream, options);

                    return response?.Data ?? new BalanceApiPreOrderDto();
                }

                return new BalanceApiPreOrderDto();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to post preorder. ErrorMessage: {ex.Message}");
                return new BalanceApiPreOrderDto();
            }
        }
    }
}
