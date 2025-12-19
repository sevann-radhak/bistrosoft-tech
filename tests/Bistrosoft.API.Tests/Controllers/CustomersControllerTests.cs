using System.Net;
using System.Net.Http.Json;
using Bistrosoft.Application.DTOs;
using Bistrosoft.API.Tests;
using FluentAssertions;
using Xunit;

namespace Bistrosoft.API.Tests.Controllers;

public class CustomersControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public CustomersControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateCustomer_ValidRequest_ReturnsCreated()
    {
        var dto = new CreateCustomerDto(
            "John Doe",
            "john@example.com",
            "1234567890"
        );

        var response = await _client.PostAsJsonAsync("/api/customers", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var customer = await response.Content.ReadFromJsonAsync<CustomerDto>();
        customer.Should().NotBeNull();
        customer!.Name.Should().Be("John Doe");
        customer.Email.Should().Be("john@example.com");
    }

    [Fact]
    public async Task CreateCustomer_InvalidEmail_ReturnsBadRequest()
    {
        var dto = new CreateCustomerDto(
            "John Doe",
            "invalid-email",
            "1234567890"
        );

        var response = await _client.PostAsJsonAsync("/api/customers", dto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetCustomerById_ExistingCustomer_ReturnsCustomer()
    {
        var createDto = new CreateCustomerDto(
            "Jane Doe",
            "jane@example.com",
            "0987654321"
        );

        var createResponse = await _client.PostAsJsonAsync("/api/customers", createDto);
        var createdCustomer = await createResponse.Content.ReadFromJsonAsync<CustomerDto>();

        var response = await _client.GetAsync($"/api/customers/{createdCustomer!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var customer = await response.Content.ReadFromJsonAsync<CustomerDto>();
        customer.Should().NotBeNull();
        customer!.Id.Should().Be(createdCustomer.Id);
        customer.Name.Should().Be("Jane Doe");
    }

    [Fact]
    public async Task GetCustomerById_NonExistentCustomer_ReturnsNotFound()
    {
        var response = await _client.GetAsync($"/api/customers/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAllCustomers_WithCustomers_ReturnsList()
    {
        var dto1 = new CreateCustomerDto(
            "Customer 1",
            "customer1@example.com",
            null
        );

        var dto2 = new CreateCustomerDto(
            "Customer 2",
            "customer2@example.com",
            null
        );

        await _client.PostAsJsonAsync("/api/customers", dto1);
        await _client.PostAsJsonAsync("/api/customers", dto2);

        var response = await _client.GetAsync("/api/customers");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var customers = await response.Content.ReadFromJsonAsync<List<CustomerDto>>();
        customers.Should().NotBeNull();
        customers!.Count.Should().BeGreaterOrEqual(2);
    }

    [Fact]
    public async Task GetCustomerOrders_WithOrders_ReturnsOrders()
    {
        var customerDto = new CreateCustomerDto(
            "Order Customer",
            "order@example.com",
            null
        );

        var customerResponse = await _client.PostAsJsonAsync("/api/customers", customerDto);
        var customer = await customerResponse.Content.ReadFromJsonAsync<CustomerDto>();

        var productDto = new CreateProductDto(
            "Test Product",
            10.00m,
            100
        );

        var productResponse = await _client.PostAsJsonAsync("/api/products", productDto);
        var product = await productResponse.Content.ReadFromJsonAsync<ProductDto>();

        var orderDto = new CreateOrderDto(
            customer!.Id,
            new List<OrderItemRequestDto>
            {
                new(product!.Id, 2)
            }
        );

        await _client.PostAsJsonAsync("/api/orders", orderDto);

        var response = await _client.GetAsync($"/api/customers/{customer.Id}/orders");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var orders = await response.Content.ReadFromJsonAsync<List<OrderDto>>();
        orders.Should().NotBeNull();
        orders!.Count.Should().BeGreaterOrEqual(1);
    }
}
