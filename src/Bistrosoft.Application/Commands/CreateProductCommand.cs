using Bistrosoft.Application.DTOs;
using MediatR;

namespace Bistrosoft.Application.Commands;

public record CreateProductCommand(
    string Name,
    decimal Price,
    int StockQuantity
) : IRequest<ProductDto>;
