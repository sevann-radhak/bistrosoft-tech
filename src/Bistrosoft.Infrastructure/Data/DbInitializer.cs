using Bistrosoft.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bistrosoft.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task SeedProductsAsync(ApplicationDbContext context)
    {
        if (await context.Products.AnyAsync())
        {
            return;
        }

        var products = new List<Product>
        {
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Margherita Pizza",
                Price = 12.99m,
                StockQuantity = 50
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Pepperoni Pizza",
                Price = 14.99m,
                StockQuantity = 45
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Caesar Salad",
                Price = 8.99m,
                StockQuantity = 30
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Grilled Chicken",
                Price = 16.99m,
                StockQuantity = 25
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Pasta Carbonara",
                Price = 13.99m,
                StockQuantity = 40
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Tiramisu",
                Price = 6.99m,
                StockQuantity = 20
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Coca Cola",
                Price = 2.99m,
                StockQuantity = 100
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Orange Juice",
                Price = 3.49m,
                StockQuantity = 60
            }
        };

        await context.Products.AddRangeAsync(products);
        await context.SaveChangesAsync();
    }
}
