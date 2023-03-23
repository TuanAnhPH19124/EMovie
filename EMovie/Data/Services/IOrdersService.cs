using EMovie.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EMovie.Data.Services
{
    public interface IOrdersService
    {
        Task StoreOrderAsync(List<ShoppingCartItem> items, string userId, string emailAddress);
        Task<List<Order>> GetOrderByUserIdAsync(string userId);

        Task<List<Order>> GetOrderByUserIdAndRoleAsync(string userId, string userRole);

    }
}
