
using BigShotCore.Extensions;
using BigShotCore.Data.Dtos;
using BigShotCore.Data.Models;

using BigShotCore.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace BigShotCore.Data.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _db;

        public OrderService(AppDbContext db)
        {
            _db = db;
        }

        // -------------------- CRUD --------------------


        public async Task<OrderDto> AddOrderAsync(CreateOrderDto dto, int userId)
        {
            var order = dto.ToOrder();
            order.UserId = userId;

            // Start a transaction to ensure atomicity
            await using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                foreach (var item in order.Items)
                {
                    var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == item.ProductId);
                    if (product == null)
                        throw new InvalidOperationException($"Product with ID {item.ProductId} not found.");

                    if (product.InStock < item.Quantity)
                        throw new InvalidOperationException($"Not enough stock for product {product.Name}. Available: {product.InStock}, Requested: {item.Quantity}");

                    // Always fetch real DB price
                    item.PriceAtPurchase = (decimal)product.Price;

                    // Reduce stock
                    product.InStock -= item.Quantity;
                    item.Product = product;

                }

                // Recalculate order total
                order.Total = order.Items.Sum(i => i.Quantity * i.PriceAtPurchase);

                await _db.Orders.AddAsync(order);
                await _db.SaveChangesAsync();

                await transaction.CommitAsync();

                return order.ToDto();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw; // Let the controller handle the exception
            }
        }


        public async Task<bool> DeleteOrderAsync(int id)
        {
            var order = await _db.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.OrderId == id);
            
            if (order == null) return false;

            // Optionally, restore stock when deleting an order
            foreach (var item in order.Items)
            {
                var product = await _db.Products
                    .FirstOrDefaultAsync(p => p.Id == item.ProductId);

                if (product != null)
                    product.InStock += item.Quantity;
            }

            _db.Orders.Remove(order);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int id)
        {

            var order = await _db.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.OrderId == id);
            return order?.ToDto();
        }

        public async Task<IEnumerable<OrderDto>> ListOrdersAsync(int pageSize, int pageIndex, int? userId = null)
        {
            IQueryable<Order> query = _db.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product); ;

            if (userId.HasValue)
                query = query.Where(o => o.UserId == userId.Value);

            var orders = await query
                .OrderBy(o => o.OrderDate)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return orders.Select(o => o.ToDto());
        }



        public async Task<bool> UpdateOrderAsync(int id, UpdateOrderDto dto)
        {
            var order = await _db.Orders
                                  .Include(o => o.Items)
                                  .ThenInclude(i => i.Product)
                                  .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null) return false;

            order.UpdateFromDto(dto);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<OrderDto>> SearchOrdersAsync(string keyword, int pageSize, int pageIndex)
        {
            var orders = await _db.Orders
                                  .Include(o => o.Items)
                                  .ThenInclude(i => i.Product)
                                  .Where(o => o.CustomerName.Contains(keyword))
                                  .OrderBy(o => o.OrderDate)
                                  .Skip(pageIndex * pageSize)
                                  .Take(pageSize)
                                  .ToListAsync();

            return orders.Select(o => o.ToDto());
        }
    }
}
