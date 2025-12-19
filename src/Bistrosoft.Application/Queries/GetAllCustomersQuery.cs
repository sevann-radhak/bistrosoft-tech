using Bistrosoft.Application.DTOs;
using MediatR;

namespace Bistrosoft.Application.Queries;

public record GetAllCustomersQuery : IRequest<IEnumerable<CustomerDto>>;
