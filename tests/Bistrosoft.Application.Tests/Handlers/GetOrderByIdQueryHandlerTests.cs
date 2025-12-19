using Bistrosoft.Application.DTOs;
using Bistrosoft.Application.Handlers;
using Bistrosoft.Application.Queries;
using Bistrosoft.Domain.Entities;
using Bistrosoft.Domain.Enums;
using Bistrosoft.Domain.Interfaces;
using Bistrosoft.Domain.ValueObjects;
using FluentAssertions;
using Moq;
using Xunit;

namespace Bistrosoft.Application.Tests.Handlers;

public class GetOrderByIdQueryHandlerTests
{
    private readonly Mock<IOrderRepository> _repositoryMock;
    private readonly GetOrderByIdQueryHandler _handler;

    public GetOrderByIdQueryHandlerTests()
    {
        _repositoryMock = new Mock<IOrderRepository>();
        _handler = new GetOrderByIdQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingOrder_ReturnsOrderDto()
    {
        var orderId = Guid.NewGuid();
        var order = new Order
        {
            Id = orderId,
            CustomerId = Guid.NewGuid(),
            TotalAmount = 100.00m,
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        var query = new GetOrderByIdQuery(orderId);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(orderId);
        result.TotalAmount.Should().Be(100.00m);
        result.Status.Should().Be(OrderStatus.Pending);
    }

    [Fact]
    public async Task Handle_NonExistentOrder_ReturnsNull()
    {
        var orderId = Guid.NewGuid();
        var query = new GetOrderByIdQuery(orderId);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order?)null);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().BeNull();
    }
}
