using AutoMapper;
using ECommerce.Application.Dtos;
using ECommerce.Application.DTOs;
using ECommerce.Infrastructure.Services.Dtos;

namespace ECommerce.Infrastructure.MappingProfiles
{

    public class BalanceApiMappingProfile : Profile
    {
        public BalanceApiMappingProfile()
        {
            CreateMap<BalanceApiProductDto, ProductDto>();

            CreateMap<BalanceApiUserBalanceDto, UserBalanceDto>();
            CreateMap<BalanceApiOrderDto, OrderDto>();
            CreateMap<BalanceApiCancelDto, CancelDto>();
            CreateMap<BalanceApiCompleteDto, CompleteDto>();
            CreateMap<BalanceApiPreOrderDto, PreOrderDto>();

        }
    }
}
