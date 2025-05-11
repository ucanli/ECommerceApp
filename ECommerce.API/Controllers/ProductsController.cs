using AutoMapper;
using ECommerce.API.DTOs;
using ECommerce.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ECommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductManager _productManager;
        private readonly IMapper _mapper;

        public ProductsController(IProductManager productManager, IMapper mapper)
        {
            _productManager = productManager;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<ProductResponseDto>), 200)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Get()
        {
            var productDtos = await _productManager.GetProductsAsync();
            var response = _mapper.Map<List<ProductResponseDto>>(productDtos);

            return Ok(response);
        }
    }
}
