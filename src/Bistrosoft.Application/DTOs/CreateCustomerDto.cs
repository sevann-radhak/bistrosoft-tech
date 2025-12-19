namespace Bistrosoft.Application.DTOs;

public record CreateCustomerDto(
    string Name,
    string Email,
    string? PhoneNumber
);

