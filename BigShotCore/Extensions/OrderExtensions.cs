using BigShotCore.Extensions;
using BigShotCore.Data.Dtos;
using BigShotCore.Data.Models;

namespace BigShotCore.Extensions
{
    public static class OrderExtensions
    {
        // Map CreateOrderDto to Order
        public static Order ToOrder(this CreateOrderDto dto)
        {
            var order = new Order
            {
                CustomerName = dto.CustomerName,
                OrderDate = DateTime.UtcNow,
                Items = dto.Items.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    PriceAtPurchase = i.PriceAtPurchase
                }).ToList()
            };

            order.Total = order.Items.Sum(i => i.Quantity * i.PriceAtPurchase);

            return order;
        }

        // Map Order to OrderDto
        public static OrderDto ToDto(this Order order)
        {
            return new OrderDto(
                order.OrderId,
                order.CustomerName,
                order.OrderDate,
                order.Total,
                order.Items.Select(i => new OrderItemDto(
                    i.OrderItemId,
                    i.ProductId,
                    i.Quantity,
                    i.PriceAtPurchase
                )).ToList()
            );
        }

        // Update Order from UpdateOrderDto
        public static void UpdateFromDto(this Order order, UpdateOrderDto dto)
        {
            if (!string.IsNullOrEmpty(dto.CustomerName))
                order.CustomerName = dto.CustomerName;

            if (dto.Items != null && dto.Items.Any())
            {
                for (int i = 0; i < dto.Items.Count; i++)
                {
                    if (i < order.Items.Count)
                    {
                        var itemDto = dto.Items[i];
                        var item = order.Items[i];

                        if (itemDto.ProductId.HasValue) item.ProductId = itemDto.ProductId.Value;
                        if (itemDto.Quantity.HasValue) item.Quantity = itemDto.Quantity.Value;
                        if (itemDto.PriceAtPurchase.HasValue) item.PriceAtPurchase = itemDto.PriceAtPurchase.Value;
                    }
                    else
                    {
                        // Add new item if more items in DTO than existing order items
                        var newItemDto = dto.Items[i];
                        order.Items.Add(new OrderItem
                        {
                            ProductId = newItemDto.ProductId ?? 0,
                            Quantity = newItemDto.Quantity ?? 1,
                            PriceAtPurchase = newItemDto.PriceAtPurchase ?? 0
                        });
                    }
                }
            }

            // Recalculate total
            order.Total = order.Items.Sum(i => i.Quantity * i.PriceAtPurchase);
        }
    }
}
