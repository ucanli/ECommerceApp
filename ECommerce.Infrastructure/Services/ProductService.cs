using AutoMapper;
using ECommerce.Application.Dtos;
using ECommerce.Application.DTOs;
using ECommerce.Application.Interfaces.External;
using ECommerce.Infrastructure.Services.Dtos;
using Microsoft.Extensions.Logging;

using System.Text.Json;

namespace ECommerce.Infrastructure.Services
{
    public class ProductService : ExternalApiServiceBase, IProductService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ProductService> _logger;
        private readonly IMapper _mapper;

        public ProductService(IHttpClientFactory httpClientFactory, ILogger<ProductService> logger, IMapper mapper) : base(httpClientFactory, logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<List<ProductDto>> GetProductsAsync()
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "api/products");

            var response = await SendRequestAsync<List<BalanceApiProductDto>>("balance-management-api", requestMessage, "GetProducts");

            return response != null
                ? _mapper.Map<List<ProductDto>>(response)
                : new List<ProductDto>();
        }


    }
}
