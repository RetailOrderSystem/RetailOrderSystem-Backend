namespace RetailOrderSystem.API.Models;

public class Inventory
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public Product? Product { get; set; }
    public int StockQuantity { get; set; }
    public int ReorderLevel { get; set; } = 10;
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}
