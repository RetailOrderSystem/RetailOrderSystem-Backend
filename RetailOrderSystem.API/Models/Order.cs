namespace RetailOrderSystem.API.Models;

public class Order
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public string Status { get; set; } = "Pending"; // Pending|Confirmed|Preparing|Delivered|Cancelled
    public decimal TotalAmount { get; set; }
    public string? Address { get; set; }
    public string? Notes { get; set; }
    public string? CouponCode { get; set; }
    public decimal Discount { get; set; } = 0;
    public DateTime OrderedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeliveredAt { get; set; }
    public List<OrderItem> OrderItems { get; set; } = [];
}