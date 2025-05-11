using AutoMapper;
using ECommerce.API.DTOs;
using ECommerce.Application.DTOs;

namespace ECommerce.API.Mapping
{
    public class ProductMappingProfile : Profile
    {
        public ProductMappingProfile()
        {
            CreateMap<ProductDto, ProductResponseDto>();
        }
    }
}
