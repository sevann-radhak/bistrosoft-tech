using Bistrosoft.Application.Commands;
using Bistrosoft.Application.DTOs;
using Bistrosoft.Application.Handlers;
using Bistrosoft.Domain.Entities;
using Bistrosoft.Domain.Exceptions;
using Bistrosoft.Domain.Interfaces;
using Bistrosoft.Domain.ValueObjects;
using FluentAssertions;
using Moq;
using Xunit;

namespace Bistrosoft.Application.Tests.Handlers;

public class CreateCustomerCommandHandlerTests
{
    private readonly Mock<ICustomerRepository> _repositoryMock;
    private readonly CreateCustomerCommandHandler _handler;

    public CreateCustomerCommandHandlerTests()
    {
        _repositoryMock = new Mock<ICustomerRepository>();
        _handler = new CreateCustomerCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsCustomerDto()
    {
        var command = new CreateCustomerCommand(
            "John Doe",
            "john@example.com",
            "1234567890"
        );

        _repositoryMock
            .Setup(r => r.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);

        Customer? savedCustomer = null;
        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()))
            .Callback<Customer, CancellationToken>((c, _) => savedCustomer = c)
            .ReturnsAsync((Customer c, CancellationToken _) => c);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Name.Should().Be("John Doe");
        result.Email.Should().Be("john@example.com");
        result.PhoneNumber.Should().Be("1234567890");
        savedCustomer.Should().NotBeNull();
        savedCustomer!.Name.Should().Be("John Doe");
    }

    [Fact]
    public async Task Handle_DuplicateEmail_ThrowsBusinessRuleException()
    {
        var command = new CreateCustomerCommand(
            "John Doe",
            "existing@example.com",
            null
        );

        var existingCustomer = new Customer
        {
            Id = Guid.NewGuid(),
            Name = "Existing User",
            Email = Email.Create("existing@example.com")
        };

        _repositoryMock
            .Setup(r => r.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCustomer);

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("Customer with email 'existing@example.com' already exists.");
    }

    [Fact]
    public async Task Handle_CommandWithoutPhoneNumber_ReturnsCustomerDto()
    {
        var command = new CreateCustomerCommand(
            "Jane Doe",
            "jane@example.com",
            null
        );

        _repositoryMock
            .Setup(r => r.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer c, CancellationToken _) => c);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.PhoneNumber.Should().BeNull();
    }
}
