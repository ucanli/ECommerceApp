using AutoMapper;
using ECommerce.Application.Dtos;
using ECommerce.Application.Interfaces.External;
using ECommerce.Infrastructure.Services.Dtos;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace ECommerce.Infrastructure.Services
{
    public class BalanceService : IBalanceService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ProductService> _logger;

        private readonly IMapper _mapper;

        public BalanceService(IHttpClientFactory httpClientFactory, ILogger<ProductService> logger, IMapper mapper)
        {
            _httpClientFactory = httpClientFactory;
            _mapper = mapper;
            _logger = logger;
        }


        public async Task<UserBalanceDto> GetUserBalanceAsync()
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

                    if (response?.Data != null)
                    {
                        var result = _mapper.Map<UserBalanceDto>(response.Data);
                        return result;
                    }
                }

                return new UserBalanceDto();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to fetch products. ErrorMessage: {ex.Message}");
                return new UserBalanceDto();
            }
        }

        public async Task<PreOrderDto> PrePrder(string orderId, decimal amount)
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

                    if (response?.Data != null)
                    {
                        var result = _mapper.Map<PreOrderDto>(response.Data);
                        return result;
                    }
                }

                return new PreOrderDto();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to post preorder. ErrorMessage: {ex.Message}");
                return new PreOrderDto();
            }
        }


        public async Task<CompleteDto> Complete(string orderId)
        {
            try
            {
                var jsonReuest = JsonSerializer.Serialize(new { orderId });

                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "api/balance/complete");
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

                    var response = await JsonSerializer.DeserializeAsync<ApiResponse<BalanceApiCompleteDto>>(contentStream, options);

                    if (response?.Data != null)
                    {
                        var result = _mapper.Map<CompleteDto>(response.Data);
                        return result;
                    }
                }

                return new CompleteDto();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to post complate. ErrorMessage: {ex.Message}");
                return new CompleteDto();
            }
        }

        public async Task<CancelDto> Cancel(string orderId)
        {
            try
            {
                var jsonReuest = JsonSerializer.Serialize(new { orderId });

                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "api/balance/cencel");
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

                    var response = await JsonSerializer.DeserializeAsync<ApiResponse<BalanceApiCancelDto>>(contentStream, options);


                    if (response?.Data != null)
                    {
                        var result = _mapper.Map<CancelDto>(response.Data);
                        return result;
                    }
                }

                return new CancelDto();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to post cancel. ErrorMessage: {ex.Message}");
                return new CancelDto();
            }
        }

    }
}
