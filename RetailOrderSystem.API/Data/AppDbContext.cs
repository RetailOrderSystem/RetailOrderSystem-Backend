using Microsoft.EntityFrameworkCore;
using RetailOrderSystem.API.Models;

namespace RetailOrderSystem.API.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Brand> Brands { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Inventory> Inventories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>()
            .HasOne(p => p.Brand)
            .WithMany(b => b.Products)
            .HasForeignKey(p => p.BrandId);

        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId);

        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Order)
            .WithMany(o => o.OrderItems)
            .HasForeignKey(oi => oi.OrderId);

        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Product)
            .WithMany()
            .HasForeignKey(oi => oi.ProductId);

        modelBuilder.Entity<CartItem>()
            .HasOne(ci => ci.User)
            .WithMany()
            .HasForeignKey(ci => ci.UserId);

        modelBuilder.Entity<Inventory>()
            .HasOne(i => i.Product)
            .WithOne(p => p.Inventory)
            .HasForeignKey<Inventory>(i => i.ProductId);

        // Seed brands
        modelBuilder.Entity<Brand>().HasData(
            new Brand { Id = 1, Name = "Dominos", Description = "Pizza brand" },
            new Brand { Id = 2, Name = "CocaCola", Description = "Beverages brand" },
            new Brand { Id = 3, Name = "Britannia", Description = "Breads & Bakery" }
        );

        // Seed categories
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Pizza", Description = "All pizza varieties" },
            new Category { Id = 2, Name = "Cold Drinks", Description = "Chilled beverages" },
            new Category { Id = 3, Name = "Breads", Description = "Breads and bakery items" }
        );
    }
}