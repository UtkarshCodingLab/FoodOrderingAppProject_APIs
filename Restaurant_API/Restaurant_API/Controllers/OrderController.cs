using Microsoft.AspNetCore.Mvc;
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
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepo;
        private ApiResponse _response;

        public OrderController(IOrderRepository orderRepo)
        {
            _orderRepo = orderRepo;
            _response = new ApiResponse();
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse>> GetOrders(string? userId)
        {
            try
            {
                var orderHeaders = string.IsNullOrEmpty(userId)
                    ? await _orderRepo.GetOrdersByUserIdAsync(userId)
                    : new List<OrderHeader>();

                _response.Result = orderHeaders;
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
                _response.StatusCode = HttpStatusCode.InternalServerError;
                return BadRequest(_response);
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse>> GetOrder(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var order = await _orderRepo.GetOrderByIdAsync(id);

                if (order == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                _response.Result = order;
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
                _response.StatusCode = HttpStatusCode.InternalServerError;
                return BadRequest(_response);
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse>> CreateOrder([FromBody] OrderHeaderCreateDTO orderHeaderDTO)
        {
            try
            {
                var order = await _orderRepo.CreateOrderAsync(orderHeaderDTO);

                _response.Result = order;
                _response.StatusCode = HttpStatusCode.Created;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
                _response.StatusCode = HttpStatusCode.InternalServerError;
                return BadRequest(_response);
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ApiResponse>> UpdateOrderHeader(int id, [FromBody] OrderHeaderUpdateDTO orderHeaderUpdateDTO)
        {
            try
            {
                var success = await _orderRepo.UpdateOrderHeaderAsync(id, orderHeaderUpdateDTO);

                if (!success)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
                _response.StatusCode = HttpStatusCode.InternalServerError;
                return BadRequest(_response);
            }
        }
    }
}
