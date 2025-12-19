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

public class GetCustomerOrdersQueryHandlerTests
{
    private readonly Mock<IOrderRepository> _repositoryMock;
    private readonly GetCustomerOrdersQueryHandler _handler;

    public GetCustomerOrdersQueryHandlerTests()
    {
        _repositoryMock = new Mock<IOrderRepository>();
        _handler = new GetCustomerOrdersQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithOrders_ReturnsCustomerOrders()
    {
        var customerId = Guid.NewGuid();
        var orders = new List<Order>
        {
            new()
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                TotalAmount = 100.00m,
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                TotalAmount = 200.00m,
                Status = OrderStatus.Paid,
                CreatedAt = DateTime.UtcNow
            }
        };

        var query = new GetCustomerOrdersQuery(customerId);

        _repositoryMock
            .Setup(r => r.GetByCustomerIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(orders);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().HaveCount(2);
        result.Should().Contain(o => o.TotalAmount == 100.00m);
        result.Should().Contain(o => o.TotalAmount == 200.00m);
    }

    [Fact]
    public async Task Handle_NoOrders_ReturnsEmptyList()
    {
        var customerId = Guid.NewGuid();
        var query = new GetCustomerOrdersQuery(customerId);

        _repositoryMock
            .Setup(r => r.GetByCustomerIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Order>());

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().BeEmpty();
    }
}
