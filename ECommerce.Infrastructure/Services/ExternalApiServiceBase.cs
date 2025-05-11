using ECommerce.Infrastructure.Services.Dtos;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace ECommerce.Infrastructure.Services
{
    public abstract class ExternalApiServiceBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger _logger;
        private readonly JsonSerializerOptions _serializerOptions;

        protected ExternalApiServiceBase(IHttpClientFactory httpClientFactory, ILogger logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;

            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            _serializerOptions.Converters.Add(new JsonStringEnumConverter());
        }

        protected async Task<T?> SendRequestAsync<T>(
            string clientName,
            HttpRequestMessage request,
            string operationName)
        {
            var httpClient = _httpClientFactory.CreateClient(clientName);

            try
            {
                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"{operationName} failed. Status code: {response.StatusCode}");
                }

                await using var stream = await response.Content.ReadAsStreamAsync();
                var apiResponse = await JsonSerializer.DeserializeAsync<ApiResponse<T>>(stream, _serializerOptions);

                if (apiResponse == null || apiResponse?.Success == false || apiResponse.Data == null)
                {
                    return default;
                }

                return apiResponse.Data;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"{operationName} HTTP error.");
                throw;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, $"{operationName} deserialization error.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{operationName} unexpected error.");
                throw;
            }
        }
    }

}
