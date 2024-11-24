using Microsoft.EntityFrameworkCore;
using Restaurant_API.Data;
using Restaurant_API.Models;
using Restaurant_API.Repository.IRepository;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurant_API.Repository
{
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
    {
        private readonly ApplicationDbContext _db;

        public ShoppingCartRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<ShoppingCart> GetCartByUserIdAsync(string userId)
        {
            return await _db.ShoppingCarts
                .Include(cart => cart.CartItems)
                .ThenInclude(item => item.MenuItem)
                .FirstOrDefaultAsync(cart => cart.UserId == userId);
        }

        public async Task AddOrUpdateItemAsync(string userId, int menuItemId, int quantityChange)
        {
            var shoppingCart = await GetCartByUserIdAsync(userId);
            var menuItem = await _db.MenuItems.FindAsync(menuItemId);

            if (menuItem == null) return;

            if (shoppingCart == null && quantityChange > 0)
            {
                shoppingCart = new ShoppingCart { UserId = userId };
                await _db.ShoppingCarts.AddAsync(shoppingCart);
                await _db.SaveChangesAsync();
            }

            var cartItem = shoppingCart?.CartItems.FirstOrDefault(item => item.MenuItemId == menuItemId);

            if (cartItem == null && quantityChange > 0)
            {
                cartItem = new CartItem
                {
                    MenuItemId = menuItemId,
                    Quantity = quantityChange,
                    ShoppingCartId = shoppingCart.Id
                };
                await _db.CartItems.AddAsync(cartItem);
            }
            else if (cartItem != null)
            {
                cartItem.Quantity += quantityChange;

                if (cartItem.Quantity <= 0)
                {
                    _db.CartItems.Remove(cartItem);

                    if (shoppingCart.CartItems.Count == 1)
                        _db.ShoppingCarts.Remove(shoppingCart);
                }
            }

            await _db.SaveChangesAsync();
        }

        public async Task<double> CalculateCartTotalAsync(ShoppingCart shoppingCart)
        {   

            return shoppingCart.CartItems.Sum(item => item.Quantity * item.MenuItem.Price);
        }

        public async Task RemoveCartItemAsync(CartItem cartItem)
        {
            _db.CartItems.Remove(cartItem);
            await _db.SaveChangesAsync();
        }
    }
}
