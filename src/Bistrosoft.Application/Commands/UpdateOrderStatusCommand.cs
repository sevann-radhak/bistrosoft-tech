using Bistrosoft.Application.DTOs;
using Bistrosoft.Domain.Enums;
using MediatR;

namespace Bistrosoft.Application.Commands;

public record UpdateOrderStatusCommand(
    Guid OrderId,
    OrderStatus Status
) : IRequest<OrderDto>;

