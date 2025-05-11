using AutoMapper;
using ECommerce.API.Dtos;
using ECommerce.API.DTOs;
using ECommerce.Application.Commands;
using ECommerce.Application.Dtos;
using ECommerce.Application.DTOs;

namespace ECommerce.API.Mapping
{
    public class ProductMappingProfile : Profile
    {
        public ProductMappingProfile()
        {
            CreateMap<ProductDto, ProductResponseDto>();
            CreateMap<OrderProductItemDto, OrderProductItem>();
            CreateMap<CreateOrderRequestDto, CreateOrderCommand>();
            CreateMap<OrderResponseDto, OrderDto>().ReverseMap();
            CreateMap<UserBalanceResponseDto, UserBalanceDto>().ReverseMap();
            CreateMap<CompleteOrderResponseDto, CompleteDto>().ReverseMap();
        }
    }
}
