using Bistrosoft.Domain.Entities;
using Bistrosoft.Infrastructure.Data.Repositories;
using FluentAssertions;
using Xunit;

namespace Bistrosoft.Infrastructure.Tests.Repositories;

public class ProductRepositoryTests : RepositoryTestBase
{
    private readonly ProductRepository _repository;

    public ProductRepositoryTests()
    {
        _repository = new ProductRepository(Context);
    }

    [Fact]
    public async Task AddAsync_ValidProduct_ReturnsProduct()
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Test Product",
            Price = 29.99m,
            StockQuantity = 50
        };

        var result = await _repository.AddAsync(product);

        result.Should().NotBeNull();
        result.Id.Should().Be(product.Id);
        result.Name.Should().Be("Test Product");
        result.Price.Should().Be(29.99m);
        result.StockQuantity.Should().Be(50);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingProduct_ReturnsProduct()
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Existing Product",
            Price = 15.00m,
            StockQuantity = 25
        };

        await _repository.AddAsync(product);

        var result = await _repository.GetByIdAsync(product.Id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(product.Id);
        result.Name.Should().Be("Existing Product");
    }

    [Fact]
    public async Task GetByIdAsync_NonExistentProduct_ReturnsNull()
    {
        var result = await _repository.GetByIdAsync(Guid.NewGuid());

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_WithProducts_ReturnsAllProductsOrderedByName()
    {
        var product1 = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Zebra Product",
            Price = 10.00m,
            StockQuantity = 10
        };

        var product2 = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Apple Product",
            Price = 20.00m,
            StockQuantity = 20
        };

        await _repository.AddAsync(product1);
        await _repository.AddAsync(product2);

        var result = (await _repository.GetAllAsync()).ToList();

        result.Should().HaveCount(2);
        result[0].Name.Should().Be("Apple Product");
        result[1].Name.Should().Be("Zebra Product");
    }

    [Fact]
    public async Task GetAllAsync_EmptyDatabase_ReturnsEmptyList()
    {
        var result = await _repository.GetAllAsync();

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task UpdateAsync_ExistingProduct_UpdatesProduct()
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Original Name",
            Price = 10.00m,
            StockQuantity = 100
        };

        await _repository.AddAsync(product);

        product.Name = "Updated Name";
        product.Price = 20.00m;
        product.StockQuantity = 50;

        await _repository.UpdateAsync(product);

        var updated = await _repository.GetByIdAsync(product.Id);
        updated.Should().NotBeNull();
        updated!.Name.Should().Be("Updated Name");
        updated.Price.Should().Be(20.00m);
        updated.StockQuantity.Should().Be(50);
    }

    [Fact]
    public async Task DeleteAsync_ExistingProduct_RemovesProduct()
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = "To Delete",
            Price = 5.00m,
            StockQuantity = 1
        };

        await _repository.AddAsync(product);
        await _repository.DeleteAsync(product);

        var result = await _repository.GetByIdAsync(product.Id);
        result.Should().BeNull();
    }
}
