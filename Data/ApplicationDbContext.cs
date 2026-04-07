using Marketeer.Models;

namespace Marketeer.Data;

public class ApplicationDbContext
{
    public List<Category> Categories { get; }
    public List<Product> Products { get; }
    public Cart Cart { get; }
    public List<Order> Orders { get; }

    public ApplicationDbContext()
    {
        Categories = SeedData.GetCategories();
        Products = SeedData.GetProducts(Categories);
        Cart = new Cart();
        Orders = new List<Order>();
    }
}
