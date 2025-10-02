using BigShotCore.Data.Dtos;
using BigShotCore.Data.Models;

namespace BigShotCore.Data.Services
{
    public interface IOrderService
    {
        // CRUD
        Task<IEnumerable<OrderDto>> ListOrdersAsync(int pageSize, int pageIndex, int? userId = null);
        Task<OrderDto?> GetOrderByIdAsync(int id);
        Task<OrderDto> AddOrderAsync(CreateOrderDto dto, int UserId);
        Task<bool> UpdateOrderAsync(int id, UpdateOrderDto dto);
        Task<bool> DeleteOrderAsync(int id);

        // Optional helpers
        Task<IEnumerable<OrderDto>> SearchOrdersAsync(string keyword, int pageSize, int pageIndex);
    }
}
