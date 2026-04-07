using Marketeer.Models;

namespace Marketeer.Services.Interfaces;

public interface IProductService
{
    IEnumerable<Product> GetAll(string? search = null, int? categoryId = null);
    Product? GetById(int id);
    IEnumerable<Category> GetCategories();
}
