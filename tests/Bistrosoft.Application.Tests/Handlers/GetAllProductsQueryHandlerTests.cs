using Bistrosoft.Application.DTOs;
using Bistrosoft.Application.Handlers;
using Bistrosoft.Application.Queries;
using Bistrosoft.Domain.Entities;
using Bistrosoft.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace Bistrosoft.Application.Tests.Handlers;

public class GetAllProductsQueryHandlerTests
{
    private readonly Mock<IProductRepository> _repositoryMock;
    private readonly GetAllProductsQueryHandler _handler;

    public GetAllProductsQueryHandlerTests()
    {
        _repositoryMock = new Mock<IProductRepository>();
        _handler = new GetAllProductsQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithProducts_ReturnsAllProducts()
    {
        var products = new List<Product>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Product 1",
                Price = 10.00m,
                StockQuantity = 100
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Product 2",
                Price = 20.00m,
                StockQuantity = 50
            }
        };

        var query = new GetAllProductsQuery();

        _repositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().HaveCount(2);
        result.Should().Contain(p => p.Name == "Product 1");
        result.Should().Contain(p => p.Name == "Product 2");
    }

    [Fact]
    public async Task Handle_EmptyList_ReturnsEmptyList()
    {
        var query = new GetAllProductsQuery();

        _repositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Product>());

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().BeEmpty();
    }
}
