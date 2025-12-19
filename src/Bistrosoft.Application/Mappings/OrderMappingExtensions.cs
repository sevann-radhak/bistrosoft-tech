using Bistrosoft.Application.DTOs;
using Bistrosoft.Domain.Entities;

namespace Bistrosoft.Application.Mappings;

public static class OrderMappingExtensions
{
    public static OrderDto ToDto(this Order order)
    {
        return new OrderDto(
            order.Id,
            order.CustomerId,
            order.TotalAmount,
            order.CreatedAt,
            order.Status,
            order.OrderItems.Select(oi => oi.ToDto()).ToList()
        );
    }
}

