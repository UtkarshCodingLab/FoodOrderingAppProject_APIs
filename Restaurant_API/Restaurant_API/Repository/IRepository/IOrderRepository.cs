using Restaurant_API.Models;
using Restaurant_API.Models.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Restaurant_API.Repository.IRepository
{
    public interface IOrderRepository
    {
        Task<IEnumerable<OrderHeader>> GetOrdersByUserIdAsync(string userId);
        Task<OrderHeader> GetOrderByIdAsync(int orderId);
        Task<OrderHeader> CreateOrderAsync(OrderHeaderCreateDTO orderHeaderDTO);
        Task<bool> UpdateOrderHeaderAsync(int id, OrderHeaderUpdateDTO orderHeaderUpdateDTO);
    }
}

