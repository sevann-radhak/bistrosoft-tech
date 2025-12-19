using Bistrosoft.Domain.Enums;

namespace Bistrosoft.Application.DTOs;

public record OrderDto(
    Guid Id,
    Guid CustomerId,
    decimal TotalAmount,
    DateTime CreatedAt,
    OrderStatus Status,
    List<OrderItemDto> OrderItems
);



