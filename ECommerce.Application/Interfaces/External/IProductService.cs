﻿using ECommerce.Application.DTOs;

namespace ECommerce.Application.Interfaces.External
{
    public interface IProductService
    {
        Task<List<ProductDto>> GetProductsAsync();
    }
}
