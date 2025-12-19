using System.Net;
using System.Net.Http.Json;
using Bistrosoft.Application.DTOs;
using Bistrosoft.Domain.Enums;
using Bistrosoft.API.Tests;
using FluentAssertions;
using Xunit;

namespace Bistrosoft.API.Tests.Controllers;

public class OrdersControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public OrdersControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateOrder_ValidRequest_ReturnsCreated()
    {
        var customer = await CreateTestCustomer();
        var product = await CreateTestProduct();

        var orderDto = new CreateOrderDto(
            customer.Id,
            new List<OrderItemRequestDto>
            {
                new(product.Id, 2)
            }
        );

        var response = await _client.PostAsJsonAsync("/api/orders", orderDto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var order = await response.Content.ReadFromJsonAsync<OrderDto>();
        order.Should().NotBeNull();
        order!.CustomerId.Should().Be(customer.Id);
        order.Status.Should().Be(OrderStatus.Pending);
        order.TotalAmount.Should().Be(product.Price * 2);
    }

    [Fact]
    public async Task CreateOrder_NonExistentCustomer_ReturnsNotFound()
    {
        var product = await CreateTestProduct();

        var orderDto = new CreateOrderDto(
            Guid.NewGuid(),
            new List<OrderItemRequestDto>
            {
                new(product.Id, 1)
            }
        );

        var response = await _client.PostAsJsonAsync("/api/orders", orderDto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateOrder_InsufficientStock_ReturnsBadRequest()
    {
        var customer = await CreateTestCustomer();
        var product = await CreateTestProduct();

        var orderDto = new CreateOrderDto(
            customer.Id,
            new List<OrderItemRequestDto>
            {
                new(product.Id, 1000)
            }
        );

        var response = await _client.PostAsJsonAsync("/api/orders", orderDto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetOrderById_ExistingOrder_ReturnsOrder()
    {
        var customer = await CreateTestCustomer();
        var product = await CreateTestProduct();

        var orderDto = new CreateOrderDto
        {
            CustomerId = customer.Id,
            Items = new List<CreateOrderItemDto>
            {
                new() { ProductId = product.Id, Quantity = 1 }
            }
        };

        var createResponse = await _client.PostAsJsonAsync("/api/orders", orderDto);
        var createdOrder = await createResponse.Content.ReadFromJsonAsync<OrderDto>();

        var response = await _client.GetAsync($"/api/orders/{createdOrder!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var order = await response.Content.ReadFromJsonAsync<OrderDto>();
        order.Should().NotBeNull();
        order!.Id.Should().Be(createdOrder.Id);
    }

    [Fact]
    public async Task GetOrderById_NonExistentOrder_ReturnsNotFound()
    {
        var response = await _client.GetAsync($"/api/orders/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateOrderStatus_ValidTransition_ReturnsOk()
    {
        var customer = await CreateTestCustomer();
        var product = await CreateTestProduct();

        var orderDto = new CreateOrderDto(
            customer.Id,
            new List<OrderItemRequestDto>
            {
                new(product.Id, 1)
            }
        );

        var createResponse = await _client.PostAsJsonAsync("/api/orders", orderDto);
        var createdOrder = await createResponse.Content.ReadFromJsonAsync<OrderDto>();

        var updateDto = new UpdateOrderStatusDto(
            createdOrder!.Id,
            OrderStatus.Paid
        );

        var response = await _client.PutAsJsonAsync($"/api/orders/{createdOrder.Id}/status", updateDto);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedOrder = await response.Content.ReadFromJsonAsync<OrderDto>();
        updatedOrder.Should().NotBeNull();
        updatedOrder!.Status.Should().Be(OrderStatus.Paid);
    }

    [Fact]
    public async Task UpdateOrderStatus_InvalidTransition_ReturnsBadRequest()
    {
        var customer = await CreateTestCustomer();
        var product = await CreateTestProduct();

        var orderDto = new CreateOrderDto(
            customer.Id,
            new List<OrderItemRequestDto>
            {
                new(product.Id, 1)
            }
        );

        var createResponse = await _client.PostAsJsonAsync("/api/orders", orderDto);
        var createdOrder = await createResponse.Content.ReadFromJsonAsync<OrderDto>();

        var updateDto = new UpdateOrderStatusDto(
            createdOrder!.Id,
            OrderStatus.Shipped
        );

        var response = await _client.PutAsJsonAsync($"/api/orders/{createdOrder.Id}/status", updateDto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateOrderStatus_NonExistentOrder_ReturnsNotFound()
    {
        var orderId = Guid.NewGuid();
        var updateDto = new UpdateOrderStatusDto(
            orderId,
            OrderStatus.Paid
        );

        var response = await _client.PutAsJsonAsync($"/api/orders/{orderId}/status", updateDto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task<CustomerDto> CreateTestCustomer()
    {
        var dto = new CreateCustomerDto(
            "Test Customer",
            $"test{Guid.NewGuid()}@example.com",
            null
        );

        var response = await _client.PostAsJsonAsync("/api/customers", dto);
        return (await response.Content.ReadFromJsonAsync<CustomerDto>())!;
    }

    private async Task<ProductDto> CreateTestProduct()
    {
        var dto = new CreateProductDto(
            "Test Product",
            10.00m,
            100
        );

        var response = await _client.PostAsJsonAsync("/api/products", dto);
        return (await response.Content.ReadFromJsonAsync<ProductDto>())!;
    }
}
