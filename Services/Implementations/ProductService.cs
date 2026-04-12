using Marketeer.Data;
using Marketeer.Models;
using Marketeer.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Marketeer.Services.Implementations;

public class ProductService : IProductService
{
    private readonly ApplicationDbContext _context;

    public ProductService(ApplicationDbContext context)
    {
        _context = context;
    }
    public IEnumerable<Product> GetAll(string? search, int? categoryId)
    {
        var query = _context.Products.AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            var keyword = search.ToLower();

            query = query.Where(p =>
                p.Name.ToLower().Contains(keyword) ||
                (p.Description != null && p.Description.ToLower().Contains(keyword)));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        return query; 
    }
    public Product? GetById(int id) => _context.Products
        .Include(p => p.Category)
        .Include(p => p.Reviews.OrderByDescending(r => r.CreatedAtUtc))
        .AsNoTracking()
        .FirstOrDefault(p => p.Id == id);

    public void AddReview(int productId, int rating, string comment, string userId)
    {
        if (rating is < 1 or > 5 || string.IsNullOrWhiteSpace(comment))
        {
            return;
        }

        var productExists = _context.Products.Any(p => p.Id == productId);
        if (!productExists)
        {
            return;
        }

        _context.ProductReviews.Add(new ProductReview
        {
            ProductId = productId,
            Rating = rating,
            Comment = comment.Trim(),
            UserId = string.IsNullOrWhiteSpace(userId) ? "guest" : userId
        });

        _context.SaveChanges();
    }

    public IEnumerable<Category> GetCategories() => _context.Categories
        .AsNoTracking()
        .OrderBy(c => c.Name)
        .ToList();
}
