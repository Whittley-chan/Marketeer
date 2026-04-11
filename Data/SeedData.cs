using Marketeer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Marketeer.Data;

public static class SeedData
{
    public static void Initialize(ApplicationDbContext context)
    {       
        if (context.Categories.Any() || context.Products.Any())
        {
            return;
        }

        var categories = new List<Category>
        {
            new() { Id = 1, Name = "Electronics" },
            new() { Id = 2, Name = "Accessories" },
            new() { Id = 3, Name = "Office" }
        };

        context.Categories.AddRange(categories);
        context.SaveChanges();

        var products = new List<Product>
        {
            new()
            {
                Name = "Wireless Mouse",
                Description = "Ergonomic wireless mouse for daily work.",
                Price = 24.99m,
                ImageUrl = "/images/mouse.png",
                AvailableQuantity = 45,
                ReservedQuantity = 0,
                CategoryId = categories.First(c => c.Name == "Accessories").Id
            },
            new()
            {
                Name = "Mechanical Keyboard",
                Description = "Compact keyboard with tactile switches.",
                Price = 79.99m,
                ImageUrl = "/images/keyboard.png",
                AvailableQuantity = 30,
                ReservedQuantity = 0,
                CategoryId = categories.First(c => c.Name == "Electronics").Id
            },
            new()
            {
                Name = "27-inch Monitor",
                Description = "Full HD monitor with vibrant display.",
                Price = 189.99m,
                ImageUrl = "/images/monitor.png",
                AvailableQuantity = 20,
                ReservedQuantity = 0,
                CategoryId = categories.First(c => c.Name == "Electronics").Id
            },
            new()
            {
                Name = "Notebook Set",
                Description = "Set of 3 ruled notebooks for notes.",
                Price = 12.50m,
                ImageUrl = "/images/notebook.png",
                AvailableQuantity = 100,
                ReservedQuantity = 0,
                CategoryId = categories.First(c => c.Name == "Office").Id
            }
        };

        context.Products.AddRange(products);
        context.SaveChanges();
    }
}
