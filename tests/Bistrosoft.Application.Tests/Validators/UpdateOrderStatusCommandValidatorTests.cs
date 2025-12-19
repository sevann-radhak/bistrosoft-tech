using Bistrosoft.Application.Commands;
using Bistrosoft.Application.Validators;
using Bistrosoft.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace Bistrosoft.Application.Tests.Validators;

public class UpdateOrderStatusCommandValidatorTests
{
    private readonly UpdateOrderStatusCommandValidator _validator;

    public UpdateOrderStatusCommandValidatorTests()
    {
        _validator = new UpdateOrderStatusCommandValidator();
    }

    [Fact]
    public void Validate_ValidCommand_ReturnsValid()
    {
        var command = new UpdateOrderStatusCommand(
            Guid.NewGuid(),
            OrderStatus.Paid
        );

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_EmptyOrderId_ReturnsInvalid()
    {
        var command = new UpdateOrderStatusCommand(
            Guid.Empty,
            OrderStatus.Paid
        );

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "OrderId" && e.ErrorMessage == "Order ID is required.");
    }

    [Fact]
    public void Validate_InvalidStatus_ReturnsInvalid()
    {
        var command = new UpdateOrderStatusCommand(
            Guid.NewGuid(),
            (OrderStatus)999
        );

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Status" && e.ErrorMessage == "Status must be a valid OrderStatus value.");
    }

    [Fact]
    public void Validate_AllValidStatuses_ReturnsValid()
    {
        var orderId = Guid.NewGuid();

        var statuses = new[]
        {
            OrderStatus.Pending,
            OrderStatus.Paid,
            OrderStatus.Shipped,
            OrderStatus.Delivered,
            OrderStatus.Cancelled
        };

        foreach (var status in statuses)
        {
            var command = new UpdateOrderStatusCommand(orderId, status);
            var result = _validator.Validate(command);
            result.IsValid.Should().BeTrue($"Status {status} should be valid");
        }
    }
}
