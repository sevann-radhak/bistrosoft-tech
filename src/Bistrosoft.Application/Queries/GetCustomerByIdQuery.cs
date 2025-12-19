using Bistrosoft.Application.DTOs;
using MediatR;

namespace Bistrosoft.Application.Queries;

public record GetCustomerByIdQuery(Guid Id) : IRequest<CustomerDto?>;

