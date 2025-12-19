using Bistrosoft.Application.DTOs;
using Bistrosoft.Domain.Entities;

namespace Bistrosoft.Application.Mappings;

public static class OrderItemMappingExtensions
{
    public static OrderItemDto ToDto(this OrderItem orderItem)
    {
        return new OrderItemDto(
            orderItem.Id,
            orderItem.OrderId,
            orderItem.ProductId,
            orderItem.Quantity,
            orderItem.UnitPrice,
            orderItem.Product?.ToDto()
        );
    }
}



