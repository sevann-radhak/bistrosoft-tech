namespace Bistrosoft.Application.DTOs;

public record CustomerDto(
    Guid Id,
    string Name,
    string Email,
    string? PhoneNumber,
    List<OrderDto> Orders
);

