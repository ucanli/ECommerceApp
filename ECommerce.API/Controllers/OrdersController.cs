using AutoMapper;
using ECommerce.API.Dtos;
using ECommerce.API.DTOs;
using ECommerce.Application.Commands;
using ECommerce.Application.Dtos;
using ECommerce.Application.DTOs;
using ECommerce.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : Controller
    {
        private readonly IOrderManager _orderManager;
        private readonly IMapper _mapper;

        public OrdersController(IOrderManager orderManager, IMapper mapper)
        {
            _orderManager = orderManager;
            _mapper = mapper;
        }


        [HttpPost("create")]
        [ProducesResponseType(typeof(OrderResponseDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Create([FromBody] CreateOrderRequestDto request)
        {
            var command = _mapper.Map<CreateOrderCommand>(request);

            var orderDto = await _orderManager.CreateOrderAsync(command);

            var response = _mapper.Map<OrderResponseDto>(orderDto);


            return Ok(response);
        }

        [HttpPost("{id}/complete")]
        [ProducesResponseType(typeof(CompleteOrderResponseDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Complete(string id)
        {
            var complateDto = await _orderManager.CompleteOrderAsync(id);
            var response = _mapper.Map<CompleteOrderResponseDto>(complateDto);

            return Ok(response);
        }
    }
}
