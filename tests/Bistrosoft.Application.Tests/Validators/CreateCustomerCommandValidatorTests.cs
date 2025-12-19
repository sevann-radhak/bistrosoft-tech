using Bistrosoft.Application.Commands;
using Bistrosoft.Application.Validators;
using FluentAssertions;
using Xunit;

namespace Bistrosoft.Application.Tests.Validators;

public class CreateCustomerCommandValidatorTests
{
    private readonly CreateCustomerCommandValidator _validator;

    public CreateCustomerCommandValidatorTests()
    {
        _validator = new CreateCustomerCommandValidator();
    }

    [Fact]
    public void Validate_ValidCommand_ReturnsValid()
    {
        var command = new CreateCustomerCommand(
            "John Doe",
            "john@example.com",
            "1234567890"
        );

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_EmptyName_ReturnsInvalid()
    {
        var command = new CreateCustomerCommand(
            string.Empty,
            "john@example.com",
            null
        );

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name" && e.ErrorMessage == "Name is required.");
    }

    [Fact]
    public void Validate_NullName_ReturnsInvalid()
    {
        var command = new CreateCustomerCommand(
            null!,
            "john@example.com",
            null
        );

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public void Validate_NameExceedsMaxLength_ReturnsInvalid()
    {
        var longName = new string('A', 201);
        var command = new CreateCustomerCommand(
            longName,
            "john@example.com",
            null
        );

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name" && e.ErrorMessage == "Name must not exceed 200 characters.");
    }

    [Fact]
    public void Validate_EmptyEmail_ReturnsInvalid()
    {
        var command = new CreateCustomerCommand(
            "John Doe",
            string.Empty,
            null
        );

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email" && e.ErrorMessage == "Email is required.");
    }

    [Fact]
    public void Validate_InvalidEmailFormat_ReturnsInvalid()
    {
        var command = new CreateCustomerCommand(
            "John Doe",
            "invalid-email",
            null
        );

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email" && e.ErrorMessage == "Email must be a valid email address.");
    }

    [Fact]
    public void Validate_EmailExceedsMaxLength_ReturnsInvalid()
    {
        var longEmail = new string('a', 321);
        var command = new CreateCustomerCommand(
            "John Doe",
            longEmail,
            null
        );

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
        var maxLengthError = result.Errors.FirstOrDefault(e => e.PropertyName == "Email" && e.ErrorMessage == "Email must not exceed 320 characters.");
        maxLengthError.Should().NotBeNull("MaximumLength validation should catch emails over 320 characters");
    }

    [Fact]
    public void Validate_PhoneNumberExceedsMaxLength_ReturnsInvalid()
    {
        var longPhone = new string('1', 21);
        var command = new CreateCustomerCommand(
            "John Doe",
            "john@example.com",
            longPhone
        );

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "PhoneNumber" && e.ErrorMessage == "Phone number must not exceed 20 characters.");
    }

    [Fact]
    public void Validate_NullPhoneNumber_ReturnsValid()
    {
        var command = new CreateCustomerCommand(
            "John Doe",
            "john@example.com",
            null
        );

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }
}
