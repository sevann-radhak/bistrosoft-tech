using Bistrosoft.Application.DTOs;
using MediatR;

namespace Bistrosoft.Application.Commands;

public record CreateOrderCommand(
    Guid CustomerId,
    List<OrderItemRequest> Items
) : IRequest<OrderDto>;

public record OrderItemRequest(
    Guid ProductId,
    int Quantity
);

