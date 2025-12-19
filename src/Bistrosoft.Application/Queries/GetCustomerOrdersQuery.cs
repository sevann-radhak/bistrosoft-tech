using Bistrosoft.Application.DTOs;
using MediatR;

namespace Bistrosoft.Application.Queries;

public record GetCustomerOrdersQuery(Guid CustomerId) : IRequest<IEnumerable<OrderDto>>;



