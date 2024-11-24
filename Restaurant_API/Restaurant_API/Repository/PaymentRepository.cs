using Microsoft.Extensions.Configuration;
using Razorpay.Api;
using Restaurant_API.Data;
using Restaurant_API.Models;
using Restaurant_API.Repository.IRepository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurant_API.Repository
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _configuration;

        public PaymentRepository(ApplicationDbContext db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        public async Task<string> CreatePaymentIntentAsync(ShoppingCart shoppingCart)
        {
            shoppingCart.CartTotal = shoppingCart.CartItems.Sum(item => item.Quantity * item.MenuItem.Price);

            // Set up payment details for Razorpay
            var paymentDetails = new Dictionary<string, object>
            {
                { "amount", (int)(shoppingCart.CartTotal * 100) },
                { "currency", "INR" },
                { "receipt", "12121" }
            };

            // Retrieve Razorpay credentials from configuration
            var key = _configuration["RazorpaySettings:KeyId"];
            var secret = _configuration["RazorpaySettings:SecretKey"];

            // Initialize Razorpay client
            var client = new RazorpayClient(key, secret);

            // Create payment order
            var order = client.Order.Create(paymentDetails);
            return order["id"].ToString();
        }
    }
}
