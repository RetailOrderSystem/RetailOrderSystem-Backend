namespace RetailOrderSystem.API.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public string Packaging { get; set; } = string.Empty; // Box, Bottle, Pack
    public int BrandId { get; set; }
    public Brand? Brand { get; set; }
    public int CategoryId { get; set; }
    public Category? Category { get; set; }
    public bool IsAvailable { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Inventory? Inventory { get; set; }
}