using Bistrosoft.Application.DTOs;
using Bistrosoft.Application.Handlers;
using Bistrosoft.Application.Queries;
using Bistrosoft.Domain.Entities;
using Bistrosoft.Domain.Interfaces;
using Bistrosoft.Domain.ValueObjects;
using FluentAssertions;
using Moq;
using Xunit;

namespace Bistrosoft.Application.Tests.Handlers;

public class GetCustomerByIdQueryHandlerTests
{
    private readonly Mock<ICustomerRepository> _repositoryMock;
    private readonly GetCustomerByIdQueryHandler _handler;

    public GetCustomerByIdQueryHandlerTests()
    {
        _repositoryMock = new Mock<ICustomerRepository>();
        _handler = new GetCustomerByIdQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingCustomer_ReturnsCustomerDto()
    {
        var customerId = Guid.NewGuid();
        var customer = new Customer
        {
            Id = customerId,
            Name = "John Doe",
            Email = Email.Create("john@example.com"),
            PhoneNumber = "1234567890"
        };

        var query = new GetCustomerByIdQuery(customerId);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(customerId);
        result.Name.Should().Be("John Doe");
        result.Email.Should().Be("john@example.com");
    }

    [Fact]
    public async Task Handle_NonExistentCustomer_ReturnsNull()
    {
        var customerId = Guid.NewGuid();
        var query = new GetCustomerByIdQuery(customerId);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().BeNull();
    }
}
