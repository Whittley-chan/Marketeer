namespace Marketeer.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public int AvailableQuantity { get; set; }
    public int ReservedQuantity { get; set; }
    public int CategoryId { get; set; }
    public Category? Category { get; set; }
    public List<ProductReview> Reviews { get; set; } = new();
    public double AverageRating => Reviews.Count == 0 ? 0 : Reviews.Average(r => r.Rating);
}
