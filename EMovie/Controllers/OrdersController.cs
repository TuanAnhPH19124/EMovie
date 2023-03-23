using EMovie.Data.Cart;
using EMovie.Data.Services;
using EMovie.Data.ViewData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EMovie.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly IMoviesService _moviesService;
        private readonly IOrdersService _ordersService;
        private readonly ShoppingCart _shoppingCart;



        public OrdersController(IMoviesService moviesService, ShoppingCart shoppingCart, IOrdersService ordersService)
        {
            _moviesService=moviesService;
            _shoppingCart=shoppingCart;
            _ordersService = ordersService;
        }

        public IActionResult Index()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string userRole = User.FindFirstValue(ClaimTypes.Role);

            var orders = _ordersService.GetOrderByUserIdAndRoleAsync(userId, userRole);
            return View(orders);
        }
        [AllowAnonymous]
        public IActionResult ShoppingCart()
        {
            var items = _shoppingCart.GetShoppingCartItems();
            _shoppingCart.ShoppingCartItems = items;

            var response = new ShoppingCartVM()
            {
                ShoppingCart = _shoppingCart,
                ShoppingCartTotal = _shoppingCart.GetShoppingCartTotal()
            };

            return View(response);
        }
        [AllowAnonymous]
        public async Task<IActionResult> AddItemToShoppingCart(int id)
        {
            var movie = await _moviesService.GetMovieByIdAsync(id);
            if (movie != null)
            {
                _shoppingCart.AddItemToCart(movie);
            }
            return RedirectToAction(nameof(ShoppingCart));
        }
        [AllowAnonymous]
        public async Task<IActionResult> RemoveItemFromShoppingCart(int id)
        {
            var movie = await _moviesService.GetMovieByIdAsync(id);
            if (movie != null)
            {
                _shoppingCart.RemoveItemFromCart(movie);
            }
            return RedirectToAction(nameof(ShoppingCart));
        }

        public async Task<IActionResult> Buy()
        {
            var items = _shoppingCart.GetShoppingCartItems();
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string emailAddress = User.FindFirstValue(ClaimTypes.Email);

            await _ordersService.StoreOrderAsync(items, userId, emailAddress);
            await _shoppingCart.ClearShoppingCartAsync();
            return View("OrderSuccessfully");
        }

        public async Task<IActionResult> OrdersCompleted()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string userRole = User.FindFirstValue(ClaimTypes.Role);
            var orders = await _ordersService.GetOrderByUserIdAndRoleAsync(userId, userRole);
            return View(orders);
        }
    }
}
