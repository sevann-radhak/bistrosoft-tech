using Bistrosoft.Application.DTOs;
using Bistrosoft.Domain.Entities;

namespace Bistrosoft.Application.Mappings;

public static class ProductMappingExtensions
{
    public static ProductDto ToDto(this Product product)
    {
        return new ProductDto(
            product.Id,
            product.Name,
            product.Price,
            product.StockQuantity
        );
    }
}



