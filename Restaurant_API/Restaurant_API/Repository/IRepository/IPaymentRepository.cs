using Restaurant_API.Models;
using System.Threading.Tasks;

namespace Restaurant_API.Repository.IRepository
{
    public interface IPaymentRepository
    {
        Task<string> CreatePaymentIntentAsync(ShoppingCart shoppingCart);
    }
}

