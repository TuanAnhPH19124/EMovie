using EMovie.Data.Static;
using EMovie.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMovie.Data.Services
{
    public class OrdersService : IOrdersService
    {
        private readonly AppDbContext _context;

        public OrdersService(AppDbContext context)
        {
            _context=context;
        }

        public async Task<List<Order>> GetOrderByUserIdAndRoleAsync(string userId, string userRole)
        {
            var orders = await _context.Orders.Include(p => p.User).Include(p => p.OrderItems).ThenInclude(p => p.Movie).ToListAsync();

            if (userRole != UserRoles.Admin)
            {
                orders = orders.Where(p => p.UserId == userId).ToList();
            }

            return orders;
        }

        public async Task<List<Order>> GetOrderByUserIdAsync(string userId)
        {
            var orders = await _context.Orders.Include(p => p.OrderItems).ThenInclude(p => p.Movie).Where(p => p.UserId == userId).ToListAsync();
            return orders;

        }

        public async Task StoreOrderAsync(List<ShoppingCartItem> items, string userId, string emailAddress)
        {
            var order = new Order()
            {
                UserId = userId,
                Email = emailAddress,

            };
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            foreach (var item in items)
            {
                var orderItem = new OrderItem()
                {
                    Amount = item.Amount,
                    MovieId = item.Movie.Id,
                    OrderId = order.Id,
                    Price = item.Movie.Price
                };

                await _context.OrderItems.AddAsync(orderItem);
            }
            await _context.SaveChangesAsync();
        }
    }
}
