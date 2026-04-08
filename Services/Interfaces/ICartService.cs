using Marketeer.Models;

namespace Marketeer.Services.Interfaces;

public interface ICartService
{
    Cart GetCart();
    Cart GetSelectedCart();
    void AddToCart(int productId, int quantity = 1);
    void SetSelection(int productId, bool isSelected);
    void RemoveFromCart(int productId);
    void ClearCart(bool selectedOnly = false);
}
