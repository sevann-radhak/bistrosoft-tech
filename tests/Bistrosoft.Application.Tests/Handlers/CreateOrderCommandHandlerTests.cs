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

public class CreateOrderCommandHandlerTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<ICustomerRepository> _customerRepositoryMock;
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly CreateOrderCommandHandler _handler;

    public CreateOrderCommandHandlerTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _customerRepositoryMock = new Mock<ICustomerRepository>();
        _productRepositoryMock = new Mock<IProductRepository>();
        _handler = new CreateOrderCommandHandler(
            _orderRepositoryMock.Object,
            _customerRepositoryMock.Object,
            _productRepositoryMock.Object
        );
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsOrderDto()
    {
        var customerId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var customer = new Customer
        {
            Id = customerId,
            Name = "Test Customer",
            Email = Email.Create("test@example.com")
        };

        var product = new Product
        {
            Id = productId,
            Name = "Test Product",
            Price = 10.00m,
            StockQuantity = 100
        };

        var command = new CreateOrderCommand(
            customerId,
            new List<OrderItemRequest>
            {
                new(productId, 2)
            }
        );

        _customerRepositoryMock
            .Setup(r => r.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        _productRepositoryMock
            .Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        Order? savedOrder = null;
        _orderRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .Callback<Order, CancellationToken>((o, _) => savedOrder = o)
            .ReturnsAsync((Order o, CancellationToken _) => o);

        _productRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.CustomerId.Should().Be(customerId);
        result.Status.Should().Be(OrderStatus.Pending);
        result.TotalAmount.Should().Be(20.00m);
        savedOrder.Should().NotBeNull();
        savedOrder!.OrderItems.Should().HaveCount(1);
        savedOrder.OrderItems.First().Quantity.Should().Be(2);
    }

    [Fact]
    public async Task Handle_NonExistentCustomer_ThrowsNotFoundException()
    {
        var customerId = Guid.NewGuid();
        var command = new CreateOrderCommand(
            customerId,
            new List<OrderItemRequest>
            {
                new(Guid.NewGuid(), 1)
            }
        );

        _customerRepositoryMock
            .Setup(r => r.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Entity 'Customer' with key '{customerId}' was not found.");
    }

    [Fact]
    public async Task Handle_EmptyItems_ThrowsBusinessRuleException()
    {
        var customerId = Guid.NewGuid();
        var customer = new Customer
        {
            Id = customerId,
            Name = "Test Customer",
            Email = Email.Create("test@example.com")
        };

        var command = new CreateOrderCommand(customerId, new List<OrderItemRequest>());

        _customerRepositoryMock
            .Setup(r => r.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("Order must contain at least one item.");
    }

    [Fact]
    public async Task Handle_NullItems_ThrowsBusinessRuleException()
    {
        var customerId = Guid.NewGuid();
        var customer = new Customer
        {
            Id = customerId,
            Name = "Test Customer",
            Email = Email.Create("test@example.com")
        };

        var command = new CreateOrderCommand(customerId, null!);

        _customerRepositoryMock
            .Setup(r => r.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("Order must contain at least one item.");
    }

    [Fact]
    public async Task Handle_NonExistentProduct_ThrowsNotFoundException()
    {
        var customerId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var customer = new Customer
        {
            Id = customerId,
            Name = "Test Customer",
            Email = Email.Create("test@example.com")
        };

        var command = new CreateOrderCommand(
            customerId,
            new List<OrderItemRequest>
            {
                new(productId, 1)
            }
        );

        _customerRepositoryMock
            .Setup(r => r.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        _productRepositoryMock
            .Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Entity 'Product' with key '{productId}' was not found.");
    }

    [Fact]
    public async Task Handle_InsufficientStock_ThrowsBusinessRuleException()
    {
        var customerId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var customer = new Customer
        {
            Id = customerId,
            Name = "Test Customer",
            Email = Email.Create("test@example.com")
        };

        var product = new Product
        {
            Id = productId,
            Name = "Test Product",
            Price = 10.00m,
            StockQuantity = 5
        };

        var command = new CreateOrderCommand(
            customerId,
            new List<OrderItemRequest>
            {
                new(productId, 10)
            }
        );

        _customerRepositoryMock
            .Setup(r => r.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        _productRepositoryMock
            .Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("Insufficient stock for product 'Test Product'. Available: 5, Requested: 10");
    }

    [Fact]
    public async Task Handle_MultipleItems_CalculatesTotalCorrectly()
    {
        var customerId = Guid.NewGuid();
        var product1Id = Guid.NewGuid();
        var product2Id = Guid.NewGuid();
        var customer = new Customer
        {
            Id = customerId,
            Name = "Test Customer",
            Email = Email.Create("test@example.com")
        };

        var product1 = new Product
        {
            Id = product1Id,
            Name = "Product 1",
            Price = 10.00m,
            StockQuantity = 100
        };

        var product2 = new Product
        {
            Id = product2Id,
            Name = "Product 2",
            Price = 15.00m,
            StockQuantity = 100
        };

        var command = new CreateOrderCommand(
            customerId,
            new List<OrderItemRequest>
            {
                new(product1Id, 2),
                new(product2Id, 3)
            }
        );

        _customerRepositoryMock
            .Setup(r => r.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        _productRepositoryMock
            .Setup(r => r.GetByIdAsync(product1Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product1);

        _productRepositoryMock
            .Setup(r => r.GetByIdAsync(product2Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product2);

        Order? savedOrder = null;
        _orderRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .Callback<Order, CancellationToken>((o, _) => savedOrder = o)
            .ReturnsAsync((Order o, CancellationToken _) => o);

        _productRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.TotalAmount.Should().Be(65.00m);
        savedOrder.Should().NotBeNull();
        savedOrder!.OrderItems.Should().HaveCount(2);
    }
}
