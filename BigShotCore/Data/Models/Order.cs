namespace BigShotCore.Data.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; } = null!;
        public string Address { get; set; } = null!;
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public decimal Total { get; set; }

        // Link order to customer
        public int UserId { get; set; }
        public AppUser User { get; set; } = null!;

        public List<OrderItem> Items { get; set; } = new();
    }

}
