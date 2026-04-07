using Marketeer.Models;

namespace Marketeer.Services.Interfaces;

public interface ICartService
{
    Cart GetCart();
    void AddToCart(int productId, int quantity = 1);
    void RemoveFromCart(int productId);
    void ClearCart();
}
