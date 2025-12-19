using Bistrosoft.Application.DTOs;
using MediatR;

namespace Bistrosoft.Application.Queries;

public record GetAllProductsQuery : IRequest<IEnumerable<ProductDto>>;
