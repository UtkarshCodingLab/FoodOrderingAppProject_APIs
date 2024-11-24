using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Restaurant_API.Models;
using Restaurant_API.Models.Dto;
using Restaurant_API.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Restaurant_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly ApiResponse _response;
        private readonly IPaymentRepository _paymentRepo;
        private readonly IShoppingCartRepository _shoppingCartRepo;

        public PaymentController(IPaymentRepository paymentRepo, IShoppingCartRepository shoppingCartRepo)
        {
            _response = new ApiResponse();
            _paymentRepo = paymentRepo;
            _shoppingCartRepo = shoppingCartRepo;
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse>> MakePayment(string userId)
        {
            try
            {
                // Get the shopping cart for the user
                var shoppingCart = await _shoppingCartRepo.GetCartByUserIdAsync(userId);

                if (shoppingCart == null || shoppingCart.CartItems == null || shoppingCart.CartItems.Count == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    return BadRequest(_response);
                }

                // Create payment intent
                shoppingCart.RazorpayPaymentIntentId = await _paymentRepo.CreatePaymentIntentAsync(shoppingCart);

                _response.Result = shoppingCart;
                _response.StatusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.Message };
                _response.StatusCode = HttpStatusCode.InternalServerError;
            }

            return Ok(_response);
        }
    }
}
