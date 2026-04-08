using Marketeer.Models;
using Microsoft.EntityFrameworkCore;

namespace Marketeer.Data;

public static class SeedData
{
    public static void Initialize(ApplicationDbContext context)
    {
        // Ensure identity/app tables coexist even when the DB existed before app entities were added.
        context.Database.ExecuteSqlRaw("""
            CREATE TABLE IF NOT EXISTS "Categories" (
                "Id" INTEGER NOT NULL CONSTRAINT "PK_Categories" PRIMARY KEY AUTOINCREMENT,
                "Name" TEXT NOT NULL
            );
            """);

        context.Database.ExecuteSqlRaw("""
            CREATE TABLE IF NOT EXISTS "Products" (
                "Id" INTEGER NOT NULL CONSTRAINT "PK_Products" PRIMARY KEY AUTOINCREMENT,
                "Name" TEXT NOT NULL,
                "Description" TEXT NOT NULL,
                "Price" TEXT NOT NULL,
                "ImageUrl" TEXT NOT NULL,
                "CategoryId" INTEGER NOT NULL,
                CONSTRAINT "FK_Products_Categories_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES "Categories" ("Id") ON DELETE RESTRICT
            );
            """);

        context.Database.ExecuteSqlRaw("""
            CREATE TABLE IF NOT EXISTS "CartItems" (
                "Id" INTEGER NOT NULL CONSTRAINT "PK_CartItems" PRIMARY KEY AUTOINCREMENT,
                "UserId" TEXT NOT NULL,
                "ProductId" INTEGER NOT NULL,
                "Quantity" INTEGER NOT NULL,
                CONSTRAINT "FK_CartItems_Products_ProductId" FOREIGN KEY ("ProductId") REFERENCES "Products" ("Id") ON DELETE CASCADE
            );
            """);

        context.Database.ExecuteSqlRaw("""
            CREATE TABLE IF NOT EXISTS "Orders" (
                "Id" INTEGER NOT NULL CONSTRAINT "PK_Orders" PRIMARY KEY AUTOINCREMENT,
                "UserId" TEXT NOT NULL,
                "CustomerName" TEXT NOT NULL,
                "ShippingAddress" TEXT NOT NULL,
                "CreatedAtUtc" TEXT NOT NULL
            );
            """);

        context.Database.ExecuteSqlRaw("""
            CREATE TABLE IF NOT EXISTS "OrderItems" (
                "Id" INTEGER NOT NULL CONSTRAINT "PK_OrderItems" PRIMARY KEY AUTOINCREMENT,
                "OrderId" INTEGER NOT NULL,
                "ProductId" INTEGER NOT NULL,
                "ProductName" TEXT NOT NULL,
                "UnitPrice" TEXT NOT NULL,
                "Quantity" INTEGER NOT NULL,
                CONSTRAINT "FK_OrderItems_Orders_OrderId" FOREIGN KEY ("OrderId") REFERENCES "Orders" ("Id") ON DELETE CASCADE
            );
            """);

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
                CategoryId = categories.First(c => c.Name == "Accessories").Id
            },
            new()
            {
                Name = "Mechanical Keyboard",
                Description = "Compact keyboard with tactile switches.",
                Price = 79.99m,
                ImageUrl = "/images/keyboard.png",
                CategoryId = categories.First(c => c.Name == "Electronics").Id
            },
            new()
            {
                Name = "27-inch Monitor",
                Description = "Full HD monitor with vibrant display.",
                Price = 189.99m,
                ImageUrl = "/images/monitor.png",
                CategoryId = categories.First(c => c.Name == "Electronics").Id
            },
            new()
            {
                Name = "Notebook Set",
                Description = "Set of 3 ruled notebooks for notes.",
                Price = 12.50m,
                ImageUrl = "/images/notebook.png",
                CategoryId = categories.First(c => c.Name == "Office").Id
            }
        };

        context.Products.AddRange(products);
        context.SaveChanges();
    }
}
