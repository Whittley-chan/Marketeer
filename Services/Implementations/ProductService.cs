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

    public IEnumerable<Product> GetAll(string? search = null, int? categoryId = null)
    {
        IQueryable<Product> query = _context.Products
            .Include(p => p.Category)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p =>
                p.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                p.Description.Contains(search, StringComparison.OrdinalIgnoreCase));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        return query.OrderBy(p => p.Name).ToList();
    }

    public Product? GetById(int id) => _context.Products
        .Include(p => p.Category)
        .AsNoTracking()
        .FirstOrDefault(p => p.Id == id);

    public IEnumerable<Category> GetCategories() => _context.Categories
        .AsNoTracking()
        .OrderBy(c => c.Name)
        .ToList();
}
