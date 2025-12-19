using Bistrosoft.Application.DTOs;
using MediatR;

namespace Bistrosoft.Application.Commands;

public record CreateCustomerCommand(
    string Name,
    string Email,
    string? PhoneNumber
) : IRequest<CustomerDto>;



