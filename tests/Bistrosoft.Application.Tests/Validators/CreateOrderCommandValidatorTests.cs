using Bistrosoft.Application.Commands;
using Bistrosoft.Application.Validators;
using FluentAssertions;
using Xunit;

namespace Bistrosoft.Application.Tests.Validators;

public class CreateOrderCommandValidatorTests
{
    private readonly CreateOrderCommandValidator _validator;

    public CreateOrderCommandValidatorTests()
    {
        _validator = new CreateOrderCommandValidator();
    }

    [Fact]
    public void Validate_ValidCommand_ReturnsValid()
    {
        var command = new CreateOrderCommand(
            Guid.NewGuid(),
            new List<OrderItemRequest>
            {
                new(Guid.NewGuid(), 2)
            }
        );

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_EmptyCustomerId_ReturnsInvalid()
    {
        var command = new CreateOrderCommand(
            Guid.Empty,
            new List<OrderItemRequest>
            {
                new(Guid.NewGuid(), 2)
            }
        );

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "CustomerId" && e.ErrorMessage == "Customer ID is required.");
    }

    [Fact]
    public void Validate_NullItems_ReturnsInvalid()
    {
        var command = new CreateOrderCommand(
            Guid.NewGuid(),
            null!
        );

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Items" && e.ErrorMessage == "Order items are required.");
    }

    [Fact]
    public void Validate_EmptyItems_ReturnsInvalid()
    {
        var command = new CreateOrderCommand(
            Guid.NewGuid(),
            new List<OrderItemRequest>()
        );

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Items" && e.ErrorMessage == "Order must contain at least one item.");
    }

    [Fact]
    public void Validate_ItemWithEmptyProductId_ReturnsInvalid()
    {
        var command = new CreateOrderCommand(
            Guid.NewGuid(),
            new List<OrderItemRequest>
            {
                new(Guid.Empty, 2)
            }
        );

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Items[0].ProductId" && e.ErrorMessage == "Product ID is required.");
    }

    [Fact]
    public void Validate_ItemWithZeroQuantity_ReturnsInvalid()
    {
        var command = new CreateOrderCommand(
            Guid.NewGuid(),
            new List<OrderItemRequest>
            {
                new(Guid.NewGuid(), 0)
            }
        );

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Items[0].Quantity" && e.ErrorMessage == "Quantity must be greater than zero.");
    }

    [Fact]
    public void Validate_ItemWithNegativeQuantity_ReturnsInvalid()
    {
        var command = new CreateOrderCommand(
            Guid.NewGuid(),
            new List<OrderItemRequest>
            {
                new(Guid.NewGuid(), -1)
            }
        );

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Items[0].Quantity");
    }

    [Fact]
    public void Validate_MultipleItemsWithInvalidData_ReturnsMultipleErrors()
    {
        var command = new CreateOrderCommand(
            Guid.NewGuid(),
            new List<OrderItemRequest>
            {
                new(Guid.Empty, 2),
                new(Guid.NewGuid(), 0)
            }
        );

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Items[0].ProductId");
        result.Errors.Should().Contain(e => e.PropertyName == "Items[1].Quantity");
    }
}
