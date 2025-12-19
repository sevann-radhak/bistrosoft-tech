namespace Bistrosoft.Application.DTOs;

public record CreateOrderDto(
    Guid CustomerId,
    List<OrderItemRequestDto> Items
);

public record OrderItemRequestDto(
    Guid ProductId,
    int Quantity
);

