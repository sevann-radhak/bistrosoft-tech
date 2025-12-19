using Bistrosoft.Domain.Entities;
using Bistrosoft.Domain.ValueObjects;
using Bistrosoft.Infrastructure.Data.Repositories;
using FluentAssertions;
using Xunit;

namespace Bistrosoft.Infrastructure.Tests.Repositories;

public class CustomerRepositoryTests : RepositoryTestBase
{
    private readonly CustomerRepository _repository;

    public CustomerRepositoryTests()
    {
        _repository = new CustomerRepository(Context);
    }

    [Fact]
    public async Task AddAsync_ValidCustomer_ReturnsCustomer()
    {
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            Name = "John Doe",
            Email = Email.Create("john@example.com"),
            PhoneNumber = "1234567890"
        };

        var result = await _repository.AddAsync(customer);

        result.Should().NotBeNull();
        result.Id.Should().Be(customer.Id);
        result.Name.Should().Be("John Doe");
        result.Email.Value.Should().Be("john@example.com");
    }

    [Fact]
    public async Task GetByIdAsync_ExistingCustomer_ReturnsCustomerWithOrders()
    {
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            Name = "Jane Doe",
            Email = Email.Create("jane@example.com")
        };

        await _repository.AddAsync(customer);

        var result = await _repository.GetByIdAsync(customer.Id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(customer.Id);
        result.Name.Should().Be("Jane Doe");
        result.Orders.Should().NotBeNull();
    }

    [Fact]
    public async Task GetByIdAsync_NonExistentCustomer_ReturnsNull()
    {
        var result = await _repository.GetByIdAsync(Guid.NewGuid());

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByEmailAsync_ExistingEmail_ReturnsCustomer()
    {
        var email = "test@example.com";
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = Email.Create(email)
        };

        await _repository.AddAsync(customer);

        var result = await _repository.GetByEmailAsync(email);

        result.Should().NotBeNull();
        result!.Email.Value.Should().Be(email);
    }

    [Fact]
    public async Task GetByEmailAsync_NonExistentEmail_ReturnsNull()
    {
        var result = await _repository.GetByEmailAsync("nonexistent@example.com");

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_WithCustomers_ReturnsAllCustomers()
    {
        var customer1 = new Customer
        {
            Id = Guid.NewGuid(),
            Name = "Customer 1",
            Email = Email.Create("customer1@example.com")
        };

        var customer2 = new Customer
        {
            Id = Guid.NewGuid(),
            Name = "Customer 2",
            Email = Email.Create("customer2@example.com")
        };

        await _repository.AddAsync(customer1);
        await _repository.AddAsync(customer2);

        var result = await _repository.GetAllAsync();

        result.Should().HaveCount(2);
        result.Should().Contain(c => c.Id == customer1.Id);
        result.Should().Contain(c => c.Id == customer2.Id);
    }

    [Fact]
    public async Task GetAllAsync_EmptyDatabase_ReturnsEmptyList()
    {
        var result = await _repository.GetAllAsync();

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task UpdateAsync_ExistingCustomer_UpdatesCustomer()
    {
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            Name = "Original Name",
            Email = Email.Create("original@example.com"),
            PhoneNumber = "1111111111"
        };

        await _repository.AddAsync(customer);

        customer.Name = "Updated Name";
        customer.PhoneNumber = "2222222222";

        await _repository.UpdateAsync(customer);

        var updated = await _repository.GetByIdAsync(customer.Id);
        updated.Should().NotBeNull();
        updated!.Name.Should().Be("Updated Name");
        updated.PhoneNumber.Should().Be("2222222222");
    }

    [Fact]
    public async Task DeleteAsync_ExistingCustomer_RemovesCustomer()
    {
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            Name = "To Delete",
            Email = Email.Create("delete@example.com")
        };

        await _repository.AddAsync(customer);
        await _repository.DeleteAsync(customer);

        var result = await _repository.GetByIdAsync(customer.Id);
        result.Should().BeNull();
    }
}
