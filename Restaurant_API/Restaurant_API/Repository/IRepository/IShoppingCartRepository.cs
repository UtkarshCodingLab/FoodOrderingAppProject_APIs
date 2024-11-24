using Restaurant_API.Models;
using System.Threading.Tasks;

namespace Restaurant_API.Repository.IRepository
{
    public interface IShoppingCartRepository : IRepository<ShoppingCart>
    {
        Task<ShoppingCart> GetCartByUserIdAsync(string userId);
        Task AddOrUpdateItemAsync(string userId, int menuItemId, int quantityChange);
        Task<double> CalculateCartTotalAsync(ShoppingCart shoppingCart);
        Task RemoveCartItemAsync(CartItem cartItem);
    }
}

