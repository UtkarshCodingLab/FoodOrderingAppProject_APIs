using Microsoft.EntityFrameworkCore;
using Restaurant_API.Data;
using Restaurant_API.Models;
using Restaurant_API.Models.Dto;
using Restaurant_API.Repository.IRepository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurant_API.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _db;

        public OrderRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<OrderHeader>> GetOrdersByUserIdAsync(string userId)
        {
            return await _db.OrderHeaders.Include(u => u.OrderDetails)
                                          .ThenInclude(u => u.MenuItem)
                                          .Where(u => u.ApplicationUserId == userId)
                                          .OrderByDescending(u => u.OrderHeaderId)
                                          .ToListAsync();
        }

        public async Task<OrderHeader> GetOrderByIdAsync(int orderId)
        {
            return await _db.OrderHeaders.Include(u => u.OrderDetails)
                                          .ThenInclude(u => u.MenuItem)
                                          .FirstOrDefaultAsync(u => u.OrderHeaderId == orderId);
        }

        public async Task<OrderHeader> CreateOrderAsync(OrderHeaderCreateDTO orderHeaderDTO)
        {
            var order = new OrderHeader
            {
                ApplicationUserId = orderHeaderDTO.ApplicationUserId,
                PickupEmail = orderHeaderDTO.PickupEmail,
                PickupName = orderHeaderDTO.PickupName,
                PickupPhoneNumber = orderHeaderDTO.PickupPhoneNumber,
                OrderTotal = orderHeaderDTO.OrderTotal,
                OrderDate = DateTime.Now,
                RazorpayPaymentIntentId = orderHeaderDTO.RazorpayPaymentIntentId,
                TotalItems = orderHeaderDTO.TotalItems,
                Status = string.IsNullOrEmpty(orderHeaderDTO.Status) ? "Pending" : orderHeaderDTO.Status
            };

            _db.OrderHeaders.Add(order);
            await _db.SaveChangesAsync();

            foreach (var orderDetailDTO in orderHeaderDTO.OrderDetailsDTO)
            {
                var orderDetail = new OrderDetails
                {
                    OrderHeaderId = order.OrderHeaderId,
                    ItemName = orderDetailDTO.ItemName,
                    MenuItemId = orderDetailDTO.MenuItemId,
                    Price = orderDetailDTO.Price,
                    Quantity = orderDetailDTO.Quantity
                };
                _db.OrderDetails.Add(orderDetail);
            }

            await _db.SaveChangesAsync();
            return order;
        }

        public async Task<bool> UpdateOrderHeaderAsync(int id, OrderHeaderUpdateDTO orderHeaderUpdateDTO)
        {
            var orderFromDb = await _db.OrderHeaders.FirstOrDefaultAsync(u => u.OrderHeaderId == id);

            if (orderFromDb == null)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(orderHeaderUpdateDTO.PickupName))
                orderFromDb.PickupName = orderHeaderUpdateDTO.PickupName;

            if (!string.IsNullOrEmpty(orderHeaderUpdateDTO.PickupPhoneNumber))
                orderFromDb.PickupPhoneNumber = orderHeaderUpdateDTO.PickupPhoneNumber;

            if (!string.IsNullOrEmpty(orderHeaderUpdateDTO.PickupEmail))
                orderFromDb.PickupEmail = orderHeaderUpdateDTO.PickupEmail;

            if (!string.IsNullOrEmpty(orderHeaderUpdateDTO.Status))
                orderFromDb.Status = orderHeaderUpdateDTO.Status;

            if (!string.IsNullOrEmpty(orderHeaderUpdateDTO.RazorpayPaymentIntentId))
                orderFromDb.RazorpayPaymentIntentId = orderHeaderUpdateDTO.RazorpayPaymentIntentId;

            await _db.SaveChangesAsync();
            return true;
        }
    }
}

