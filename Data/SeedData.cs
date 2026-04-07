using Marketeer.Models;

namespace Marketeer.Data;

public static class SeedData
{
    public static List<Category> GetCategories()
    {
        return new List<Category>
        {
            new() { Id = 1, Name = "Electronics" },
            new() { Id = 2, Name = "Accessories" },
            new() { Id = 3, Name = "Office" }
        };
    }

    public static List<Product> GetProducts(List<Category> categories)
    {
        return new List<Product>
        {
            new()
            {
                Id = 1,
                Name = "Wireless Mouse",
                Description = "Ergonomic wireless mouse for daily work.",
                Price = 24.99m,
                ImageUrl = "/images/mouse.png",
                CategoryId = 2,
                Category = categories.First(c => c.Id == 2)
            },
            new()
            {
                Id = 2,
                Name = "Mechanical Keyboard",
                Description = "Compact keyboard with tactile switches.",
                Price = 79.99m,
                ImageUrl = "/images/keyboard.png",
                CategoryId = 1,
                Category = categories.First(c => c.Id == 1)
            },
            new()
            {
                Id = 3,
                Name = "27-inch Monitor",
                Description = "Full HD monitor with vibrant display.",
                Price = 189.99m,
                ImageUrl = "/images/monitor.png",
                CategoryId = 1,
                Category = categories.First(c => c.Id == 1)
            },
            new()
            {
                Id = 4,
                Name = "Notebook Set",
                Description = "Set of 3 ruled notebooks for notes.",
                Price = 12.50m,
                ImageUrl = "/images/notebook.png",
                CategoryId = 3,
                Category = categories.First(c => c.Id == 3)
            }
        };
    }
}
