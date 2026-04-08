using Marketeer.Models;

namespace Marketeer.Services.Interfaces;

public interface IProductService
{
    IEnumerable<Product> GetAll(string? search = null, int? categoryId = null);
    Product? GetById(int id);
    void AddReview(int productId, int rating, string comment, string userId);
    IEnumerable<Category> GetCategories();
}
