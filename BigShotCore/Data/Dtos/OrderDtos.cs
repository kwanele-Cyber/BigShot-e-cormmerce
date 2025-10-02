namespace BigShotCore.Data.Dtos
{
    public record CreateOrderItemDto(
        int ProductId,
        int Quantity,
        decimal PriceAtPurchase
    );

    public record CreateOrderDto(
        string? CustomerName,
        List<CreateOrderItemDto> Items
    );

    public record UpdateOrderItemDto(
        int? ProductId = null,
        int? Quantity = null,
        decimal? PriceAtPurchase = null
    );

    public record UpdateOrderDto(
        string? CustomerName = null,
        List<UpdateOrderItemDto>? Items = null
    );

    public record OrderItemDto(
        int OrderItemId,
        int ProductId,
        int Quantity,
        decimal PriceAtPurchase
    );

    public record OrderDto(
        int OrderId,
        string CustomerName,
        DateTime OrderDate,
        decimal Total,
        List<OrderItemDto> Items
    );
}
