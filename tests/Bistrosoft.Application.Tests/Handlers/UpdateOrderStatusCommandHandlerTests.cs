using Bistrosoft.Application.Commands;
using Bistrosoft.Application.DTOs;
using Bistrosoft.Application.Handlers;
using Bistrosoft.Domain.Entities;
using Bistrosoft.Domain.Enums;
using Bistrosoft.Domain.Exceptions;
using Bistrosoft.Domain.Interfaces;
using Bistrosoft.Domain.ValueObjects;
using FluentAssertions;
using Moq;
using Xunit;

namespace Bistrosoft.Application.Tests.Handlers;

public class UpdateOrderStatusCommandHandlerTests
{
    private readonly Mock<IOrderRepository> _repositoryMock;
    private readonly UpdateOrderStatusCommandHandler _handler;

    public UpdateOrderStatusCommandHandlerTests()
    {
        _repositoryMock = new Mock<IOrderRepository>();
        _handler = new UpdateOrderStatusCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ValidTransitionFromPendingToPaid_ReturnsUpdatedOrder()
    {
        var orderId = Guid.NewGuid();
        var order = new Order
        {
            Id = orderId,
            CustomerId = Guid.NewGuid(),
            Status = OrderStatus.Pending,
            TotalAmount = 100.00m,
            CreatedAt = DateTime.UtcNow
        };

        var command = new UpdateOrderStatusCommand(orderId, OrderStatus.Paid);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        _repositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Status.Should().Be(OrderStatus.Paid);
        order.Status.Should().Be(OrderStatus.Paid);
    }

    [Fact]
    public async Task Handle_ValidTransitionFromPaidToShipped_ReturnsUpdatedOrder()
    {
        var orderId = Guid.NewGuid();
        var order = new Order
        {
            Id = orderId,
            CustomerId = Guid.NewGuid(),
            Status = OrderStatus.Paid,
            TotalAmount = 100.00m,
            CreatedAt = DateTime.UtcNow
        };

        var command = new UpdateOrderStatusCommand(orderId, OrderStatus.Shipped);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        _repositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Status.Should().Be(OrderStatus.Shipped);
    }

    [Fact]
    public async Task Handle_ValidTransitionFromShippedToDelivered_ReturnsUpdatedOrder()
    {
        var orderId = Guid.NewGuid();
        var order = new Order
        {
            Id = orderId,
            CustomerId = Guid.NewGuid(),
            Status = OrderStatus.Shipped,
            TotalAmount = 100.00m,
            CreatedAt = DateTime.UtcNow
        };

        var command = new UpdateOrderStatusCommand(orderId, OrderStatus.Delivered);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        _repositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Status.Should().Be(OrderStatus.Delivered);
    }

    [Fact]
    public async Task Handle_NonExistentOrder_ThrowsNotFoundException()
    {
        var orderId = Guid.NewGuid();
        var command = new UpdateOrderStatusCommand(orderId, OrderStatus.Paid);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order?)null);

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Entity 'Order' with key '{orderId}' was not found.");
    }

    [Fact]
    public async Task Handle_InvalidTransitionFromPendingToShipped_ThrowsBusinessRuleException()
    {
        var orderId = Guid.NewGuid();
        var order = new Order
        {
            Id = orderId,
            CustomerId = Guid.NewGuid(),
            Status = OrderStatus.Pending,
            TotalAmount = 100.00m,
            CreatedAt = DateTime.UtcNow
        };

        var command = new UpdateOrderStatusCommand(orderId, OrderStatus.Shipped);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("Invalid status transition from 'Pending' to 'Shipped'.");
    }

    [Fact]
    public async Task Handle_InvalidTransitionFromDelivered_ThrowsBusinessRuleException()
    {
        var orderId = Guid.NewGuid();
        var order = new Order
        {
            Id = orderId,
            CustomerId = Guid.NewGuid(),
            Status = OrderStatus.Delivered,
            TotalAmount = 100.00m,
            CreatedAt = DateTime.UtcNow
        };

        var command = new UpdateOrderStatusCommand(orderId, OrderStatus.Paid);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("Invalid status transition from 'Delivered' to 'Paid'.");
    }

    [Fact]
    public async Task Handle_InvalidTransitionFromCancelled_ThrowsBusinessRuleException()
    {
        var orderId = Guid.NewGuid();
        var order = new Order
        {
            Id = orderId,
            CustomerId = Guid.NewGuid(),
            Status = OrderStatus.Cancelled,
            TotalAmount = 100.00m,
            CreatedAt = DateTime.UtcNow
        };

        var command = new UpdateOrderStatusCommand(orderId, OrderStatus.Paid);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("Invalid status transition from 'Cancelled' to 'Paid'.");
    }

    [Fact]
    public async Task Handle_ValidTransitionFromPendingToCancelled_ReturnsUpdatedOrder()
    {
        var orderId = Guid.NewGuid();
        var order = new Order
        {
            Id = orderId,
            CustomerId = Guid.NewGuid(),
            Status = OrderStatus.Pending,
            TotalAmount = 100.00m,
            CreatedAt = DateTime.UtcNow
        };

        var command = new UpdateOrderStatusCommand(orderId, OrderStatus.Cancelled);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        _repositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Status.Should().Be(OrderStatus.Cancelled);
    }
}
