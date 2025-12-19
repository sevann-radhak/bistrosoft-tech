using Bistrosoft.Application.DTOs;
using MediatR;

namespace Bistrosoft.Application.Queries;

public record GetOrderByIdQuery(Guid Id) : IRequest<OrderDto?>;

