namespace BigShotCore.Data.Dtos
{
    public record CreateOrderItemDto(
        int ProductId,
        int Quantity
    );

    public record CreateOrderDto(
        string? CustomerName,
        List<CreateOrderItemDto> Items,
        string Address
    );

    public record UpdateOrderItemDto(
        int? ProductId = null,
        int? Quantity = null
    );

    public record UpdateOrderDto(
        string? CustomerName = null,
        List<UpdateOrderItemDto>? Items = null,
        string? Address = null
    );

    public record OrderItemDto(
        int OrderItemId,
        int ProductId,
        int Quantity,
        string ProductName,     // ✅ new
        decimal priceAtPurchase);

    public record OrderDto(
        int OrderId,
        string CustomerName,
        DateTime OrderDate,
        decimal Total,
        List<OrderItemDto> Items,
        string Address
    );
}
