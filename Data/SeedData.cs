using Marketeer.Models;
using Microsoft.AspNetCore.Identity;
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
                "AvailableQuantity" INTEGER NOT NULL DEFAULT 0,
                "ReservedQuantity" INTEGER NOT NULL DEFAULT 0,
                "CategoryId" INTEGER NOT NULL,
                CONSTRAINT "FK_Products_Categories_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES "Categories" ("Id") ON DELETE RESTRICT
            );
            """);

        TryExecute(context, """ALTER TABLE "Products" ADD COLUMN "AvailableQuantity" INTEGER NOT NULL DEFAULT 0;""");
        TryExecute(context, """ALTER TABLE "Products" ADD COLUMN "ReservedQuantity" INTEGER NOT NULL DEFAULT 0;""");

        context.Database.ExecuteSqlRaw("""
            CREATE TABLE IF NOT EXISTS "CartItems" (
                "Id" INTEGER NOT NULL CONSTRAINT "PK_CartItems" PRIMARY KEY AUTOINCREMENT,
                "UserId" TEXT NOT NULL,
                "ProductId" INTEGER NOT NULL,
                "Quantity" INTEGER NOT NULL,
                "IsSelectedForCheckout" INTEGER NOT NULL DEFAULT 1,
                CONSTRAINT "FK_CartItems_Products_ProductId" FOREIGN KEY ("ProductId") REFERENCES "Products" ("Id") ON DELETE CASCADE
            );
            """);
        TryExecute(context, """ALTER TABLE "CartItems" ADD COLUMN "IsSelectedForCheckout" INTEGER NOT NULL DEFAULT 1;""");

        context.Database.ExecuteSqlRaw("""
            CREATE TABLE IF NOT EXISTS "Orders" (
                "Id" INTEGER NOT NULL CONSTRAINT "PK_Orders" PRIMARY KEY AUTOINCREMENT,
                "UserId" TEXT NOT NULL,
                "CustomerName" TEXT NOT NULL,
                "ShippingAddress" TEXT NOT NULL,
                "DeliveryStatus" TEXT NOT NULL DEFAULT 'Pending',
                "CreatedAtUtc" TEXT NOT NULL
            );
            """);
        TryExecute(context, """ALTER TABLE "Orders" ADD COLUMN "DeliveryStatus" TEXT NOT NULL DEFAULT 'Pending';""");

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

        context.Database.ExecuteSqlRaw("""
            CREATE TABLE IF NOT EXISTS "ProductReviews" (
                "Id" INTEGER NOT NULL CONSTRAINT "PK_ProductReviews" PRIMARY KEY AUTOINCREMENT,
                "ProductId" INTEGER NOT NULL,
                "UserId" TEXT NOT NULL,
                "Rating" INTEGER NOT NULL,
                "Comment" TEXT NOT NULL,
                "CreatedAtUtc" TEXT NOT NULL,
                CONSTRAINT "FK_ProductReviews_Products_ProductId" FOREIGN KEY ("ProductId") REFERENCES "Products" ("Id") ON DELETE CASCADE
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

    public static async Task SeedAdminAsync(
        RoleManager<IdentityRole> roleManager,
        UserManager<IdentityUser> userManager)
    {
        const string adminRole = "Admin";
        const string adminEmail = "admin@marketeer.local";
        const string adminPassword = "Admin123!";

        if (!await roleManager.RoleExistsAsync(adminRole))
        {
            await roleManager.CreateAsync(new IdentityRole(adminRole));
        }

        var admin = await userManager.FindByEmailAsync(adminEmail);
        if (admin is null)
        {
            admin = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var createResult = await userManager.CreateAsync(admin, adminPassword);
            if (!createResult.Succeeded)
            {
                return;
            }
        }

        if (!await userManager.IsInRoleAsync(admin, adminRole))
        {
            await userManager.AddToRoleAsync(admin, adminRole);
        }
    }

    private static void TryExecute(ApplicationDbContext context, string sql)
    {
        try
        {
            context.Database.ExecuteSqlRaw(sql);
        }
        catch
        {
            // Ignore duplicate-column and similar migration-compatibility errors.
        }
    }
}
