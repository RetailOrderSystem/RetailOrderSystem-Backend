namespace RetailOrderSystem.API.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Status { get; set; } = "Pending";
        public decimal TotalAmount { get; set; }

        public string Address { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public string CouponCode { get; set; } = string.Empty;
        public decimal Discount { get; set; }

        public DateTime OrderedAt { get; set; }
        public DateTime? DeliveredAt { get; set; }

        public List<OrderItemDto> Items { get; set; } = new();
    }

    public class CreateOrderDto
    {
        public string Address { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public string CouponCode { get; set; } = string.Empty;
    }

    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}