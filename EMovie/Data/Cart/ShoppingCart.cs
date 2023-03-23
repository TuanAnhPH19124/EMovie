using EMovie.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMovie.Data.Cart
{
    public class ShoppingCart
    {
        public AppDbContext _context { get; set; }

        public string ShoppingCartId { get; set; }
        public List<ShoppingCartItem> ShoppingCartItems { get; set; }

        public ShoppingCart(AppDbContext context)
        {
            _context=context;
        }

        public static ShoppingCart GetShoppingCart(IServiceProvider services)
        {
            ISession session = services.GetRequiredService<IHttpContextAccessor>()?.HttpContext.Session;
            var context = services.GetService<AppDbContext>();

            string cardId = session.GetString("CardId") ?? Guid.NewGuid().ToString();
            session.SetString("CardId", cardId);

            return new ShoppingCart(context) { ShoppingCartId = cardId };
        }

        public void AddItemToCart(Movie movie)
        {
            // tim cai minh click vao
            var shoppingCartItem = _context.ShoppingCartItems.FirstOrDefault(p => p.Movie.Id == movie.Id && p.ShoppingCartId == ShoppingCartId);
            if (shoppingCartItem == null)
            {
                shoppingCartItem = new ShoppingCartItem()
                {
                    ShoppingCartId = ShoppingCartId,
                    Movie = movie,
                    Amount = 1
                };
                _context.ShoppingCartItems.Add(shoppingCartItem);
            }
            else
            {
                shoppingCartItem.Amount++;
            }
            _context.SaveChanges();
        }

        public void RemoveItemFromCart(Movie movie)
        {
            var shoppingCartItem = _context.ShoppingCartItems.FirstOrDefault(p => p.Movie.Id == movie.Id && p.ShoppingCartId == ShoppingCartId);
            if (shoppingCartItem != null)
            {
                if (shoppingCartItem.Amount > 1) { shoppingCartItem.Amount--; }
                else { _context.ShoppingCartItems.Remove(shoppingCartItem); }
                _context.SaveChanges();
            }
        }

        public List<ShoppingCartItem> GetShoppingCartItems()
        {
            return ShoppingCartItems ?? (ShoppingCartItems = _context.ShoppingCartItems.Where(p => p.ShoppingCartId == ShoppingCartId).Include(p => p.Movie).ToList());
        }

        public double GetShoppingCartTotal()
        {
            var total = _context.ShoppingCartItems.Where(p => p.ShoppingCartId == ShoppingCartId).Select(p => p.Movie.Price * p.Amount).Sum();
            return total;
        }

        public async Task ClearShoppingCartAsync()
        {
            var items = await _context.ShoppingCartItems.Where(p => p.ShoppingCartId == ShoppingCartId).ToListAsync();
            _context.ShoppingCartItems.RemoveRange(items);
            await _context.SaveChangesAsync();
        }

    }
}
