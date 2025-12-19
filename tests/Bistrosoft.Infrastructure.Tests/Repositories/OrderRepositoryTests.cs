using Bistrosoft.Domain.Entities;
using Bistrosoft.Domain.Enums;
using Bistrosoft.Domain.ValueObjects;
using Bistrosoft.Infrastructure.Data.Repositories;
using FluentAssertions;
using Xunit;

namespace Bistrosoft.Infrastructure.Tests.Repositories;

public class OrderRepositoryTests : RepositoryTestBase
{
    private readonly OrderRepository _repository;
    private readonly CustomerRepository _customerRepository;

    public OrderRepositoryTests()
    {
        _repository = new OrderRepository(Context);
        _customerRepository = new CustomerRepository(Context);
    }

    [Fact]
    public async Task AddAsync_ValidOrder_ReturnsOrder()
    {
        var customer = await CreateTestCustomer();
        var product = await CreateTestProduct();
        var order = CreateTestOrder(customer.Id, product);

        var result = await _repository.AddAsync(order);

        result.Should().NotBeNull();
        result.Id.Should().Be(order.Id);
        result.CustomerId.Should().Be(customer.Id);
        result.Status.Should().Be(OrderStatus.Pending);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingOrder_ReturnsOrderWithRelations()
    {
        var customer = await CreateTestCustomer();
        var product = await CreateTestProduct();
        var order = CreateTestOrder(customer.Id, product);

        await _repository.AddAsync(order);

        var result = await _repository.GetByIdAsync(order.Id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(order.Id);
        result.Customer.Should().NotBeNull();
        result.OrderItems.Should().NotBeEmpty();
        result.OrderItems.First().Product.Should().NotBeNull();
    }

    [Fact]
    public async Task GetByIdAsync_NonExistentOrder_ReturnsNull()
    {
        var result = await _repository.GetByIdAsync(Guid.NewGuid());

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByCustomerIdAsync_WithOrders_ReturnsCustomerOrders()
    {
        var customer = await CreateTestCustomer();
        var product = await CreateTestProduct();

        var order1 = CreateTestOrder(customer.Id, product);
        var order2 = CreateTestOrder(customer.Id, product);

        await _repository.AddAsync(order1);
        await _repository.AddAsync(order2);

        var result = await _repository.GetByCustomerIdAsync(customer.Id);

        result.Should().HaveCount(2);
        result.Should().Contain(o => o.Id == order1.Id);
        result.Should().Contain(o => o.Id == order2.Id);
    }

    [Fact]
    public async Task GetByCustomerIdAsync_NoOrders_ReturnsEmptyList()
    {
        var customer = await CreateTestCustomer();

        var result = await _repository.GetByCustomerIdAsync(customer.Id);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByCustomerIdAsync_OrdersOrderedByCreatedAtDescending()
    {
        var customer = await CreateTestCustomer();
        var product = await CreateTestProduct();

        var order1 = CreateTestOrder(customer.Id, product);
        order1.CreatedAt = DateTime.UtcNow.AddHours(-2);

        var order2 = CreateTestOrder(customer.Id, product);
        order2.CreatedAt = DateTime.UtcNow.AddHours(-1);

        await _repository.AddAsync(order1);
        await _repository.AddAsync(order2);

        var result = (await _repository.GetByCustomerIdAsync(customer.Id)).ToList();

        result.Should().HaveCount(2);
        result[0].Id.Should().Be(order2.Id);
        result[1].Id.Should().Be(order1.Id);
    }

    [Fact]
    public async Task GetAllAsync_WithOrders_ReturnsAllOrders()
    {
        var customer1 = await CreateTestCustomer();
        var customer2 = await CreateTestCustomer();
        var product = await CreateTestProduct();

        var order1 = CreateTestOrder(customer1.Id, product);
        var order2 = CreateTestOrder(customer2.Id, product);

        await _repository.AddAsync(order1);
        await _repository.AddAsync(order2);

        var result = await _repository.GetAllAsync();

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task UpdateAsync_ExistingOrder_UpdatesOrder()
    {
        var customer = await CreateTestCustomer();
        var product = await CreateTestProduct();
        var order = CreateTestOrder(customer.Id, product);

        await _repository.AddAsync(order);

        order.Status = OrderStatus.Paid;
        order.TotalAmount = 200.00m;

        await _repository.UpdateAsync(order);

        var updated = await _repository.GetByIdAsync(order.Id);
        updated.Should().NotBeNull();
        updated!.Status.Should().Be(OrderStatus.Paid);
        updated.TotalAmount.Should().Be(200.00m);
    }

    [Fact]
    public async Task DeleteAsync_ExistingOrder_RemovesOrder()
    {
        var customer = await CreateTestCustomer();
        var product = await CreateTestProduct();
        var order = CreateTestOrder(customer.Id, product);

        await _repository.AddAsync(order);
        await _repository.DeleteAsync(order);

        var result = await _repository.GetByIdAsync(order.Id);
        result.Should().BeNull();
    }

    private async Task<Customer> CreateTestCustomer()
    {
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            Name = "Test Customer",
            Email = Email.Create($"test{Guid.NewGuid()}@example.com")
        };
        return await _customerRepository.AddAsync(customer);
    }

    private async Task<Product> CreateTestProduct()
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Test Product",
            Price = 10.00m,
            StockQuantity = 100
        };
        Context.Set<Product>().Add(product);
        await Context.SaveChangesAsync();
        return product;
    }

    private Order CreateTestOrder(Guid customerId, Product product)
    {
        var orderItem = new OrderItem
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            Quantity = 2,
            UnitPrice = product.Price
        };

        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            TotalAmount = product.Price * 2,
            CreatedAt = DateTime.UtcNow,
            Status = OrderStatus.Pending,
            OrderItems = new List<OrderItem> { orderItem }
        };

        orderItem.OrderId = order.Id;
        return order;
    }
}
