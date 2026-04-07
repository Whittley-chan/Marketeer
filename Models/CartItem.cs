namespace Marketeer.Models;

public class CartItem
{
    public int ProductId { get; set; }
    public Product Product { get; set; } = new();
    public int Quantity { get; set; }
    public decimal LineTotal => Product.Price * Quantity;
}
