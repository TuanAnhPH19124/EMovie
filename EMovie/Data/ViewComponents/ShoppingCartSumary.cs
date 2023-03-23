using EMovie.Data.Cart;
using Microsoft.AspNetCore.Mvc;

namespace EMovie.Data.ViewComponents
{
    public class ShoppingCartSumary : ViewComponent
    {
        private readonly ShoppingCart _shoppingCart;

        public ShoppingCartSumary(ShoppingCart shoppingCart)
        {
            _shoppingCart=shoppingCart;
        }

        public IViewComponentResult Invoke()
        {
            var items = _shoppingCart.GetShoppingCartItems();
            return View(items.Count);
        }
    }
}
