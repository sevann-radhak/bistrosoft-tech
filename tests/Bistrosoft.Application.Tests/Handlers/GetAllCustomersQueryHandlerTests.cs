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

public class GetAllCustomersQueryHandlerTests
{
    private readonly Mock<ICustomerRepository> _repositoryMock;
    private readonly GetAllCustomersQueryHandler _handler;

    public GetAllCustomersQueryHandlerTests()
    {
        _repositoryMock = new Mock<ICustomerRepository>();
        _handler = new GetAllCustomersQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithCustomers_ReturnsAllCustomers()
    {
        var customers = new List<Customer>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Customer 1",
                Email = Email.Create("customer1@example.com")
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Customer 2",
                Email = Email.Create("customer2@example.com")
            }
        };

        var query = new GetAllCustomersQuery();

        _repositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(customers);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().HaveCount(2);
        result.Should().Contain(c => c.Name == "Customer 1");
        result.Should().Contain(c => c.Name == "Customer 2");
    }

    [Fact]
    public async Task Handle_EmptyList_ReturnsEmptyList()
    {
        var query = new GetAllCustomersQuery();

        _repositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Customer>());

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().BeEmpty();
    }
}
