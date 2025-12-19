namespace Bistrosoft.Application.DTOs;

public record OrderItemDto(
    Guid Id,
    Guid OrderId,
    Guid ProductId,
    int Quantity,
    decimal UnitPrice,
    ProductDto? Product
);

