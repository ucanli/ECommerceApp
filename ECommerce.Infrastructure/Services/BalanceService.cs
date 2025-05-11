using AutoMapper;
using ECommerce.Application.Dtos;
using ECommerce.Application.Interfaces.External;
using ECommerce.Infrastructure.Services.Dtos;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ECommerce.Infrastructure.Services
{
    public class BalanceService : ExternalApiServiceBase, IBalanceService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ProductService> _logger;

        private readonly IMapper _mapper;

        public BalanceService(IHttpClientFactory httpClientFactory, ILogger<ProductService> logger, IMapper mapper) : base(httpClientFactory, logger)
        {
            _httpClientFactory = httpClientFactory;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<UserBalanceDto> GetUserBalanceAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/balance");
            var data = await SendRequestAsync<BalanceApiUserBalanceDto>("balance-management-api", request, "GetUserBalance");
            return data != null ? _mapper.Map<UserBalanceDto>(data) : new UserBalanceDto();
        }

        public async Task<PreOrderDto> PreOrderAsync(string orderId, decimal amount)
        {
            var body = JsonSerializer.Serialize(new { orderId, amount });
            var request = new HttpRequestMessage(HttpMethod.Post, "api/balance/preorder")
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            };

            var data = await SendRequestAsync<BalanceApiPreOrderDto>("balance-management-api", request, "PreOrder");
            return data != null ? _mapper.Map<PreOrderDto>(data) : new PreOrderDto();
        }

        public async Task<CompleteDto> CompleteAsync(string orderId)
        {
            var body = JsonSerializer.Serialize(new { orderId });
            var request = new HttpRequestMessage(HttpMethod.Post, "api/balance/complete")
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            };

            var data = await SendRequestAsync<BalanceApiCompleteDto>("balance-management-api", request, "Complete");
            return data != null ? _mapper.Map<CompleteDto>(data) : new CompleteDto();
        }

        public async Task<CancelDto> CancelAsync(string orderId)
        {
            var requestBody = JsonSerializer.Serialize(new { orderId });

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "api/balance/cancel")
            {
                Content = new StringContent(requestBody, Encoding.UTF8, "application/json")
            };

            var response = await SendRequestAsync<BalanceApiCancelDto>("balance-management-api",requestMessage, "Cancel");

            return response != null
                ? _mapper.Map<CancelDto>(response)
                : new CancelDto();
        }

    }
}
