using Bistrosoft.Application.Commands;
using Bistrosoft.Application.DTOs;
using Bistrosoft.Application.Handlers;
using Bistrosoft.Domain.Entities;
using Bistrosoft.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace Bistrosoft.Application.Tests.Handlers;

public class CreateProductCommandHandlerTests
{
    private readonly Mock<IProductRepository> _repositoryMock;
    private readonly CreateProductCommandHandler _handler;

    public CreateProductCommandHandlerTests()
    {
        _repositoryMock = new Mock<IProductRepository>();
        _handler = new CreateProductCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsProductDto()
    {
        var command = new CreateProductCommand(
            "Test Product",
            29.99m,
            50
        );

        Product? savedProduct = null;
        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .Callback<Product, CancellationToken>((p, _) => savedProduct = p)
            .ReturnsAsync((Product p, CancellationToken _) => p);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Name.Should().Be("Test Product");
        result.Price.Should().Be(29.99m);
        result.StockQuantity.Should().Be(50);
        savedProduct.Should().NotBeNull();
        savedProduct!.Name.Should().Be("Test Product");
    }
}
