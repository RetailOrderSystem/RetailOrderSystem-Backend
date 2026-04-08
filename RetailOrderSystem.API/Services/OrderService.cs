using Microsoft.EntityFrameworkCore;
using RetailOrderSystem.API.Data;
using RetailOrderSystem.API.DTOs;
using RetailOrderSystem.API.Models;
using RetailOrderSystem.API.Services.Interfaces;

namespace RetailOrderSystem.API.Services;

public class OrderService : IOrderService
{
    private readonly AppDbContext _db;
    private readonly IEmailService _emailService;

    public OrderService(AppDbContext db, IEmailService emailService)
    {
        _db = db;
        _emailService = emailService;
    }

    public async Task<OrderDto> PlaceOrderAsync(int userId, CreateOrderDto dto)
    {
        var cartItems = await _db.CartItems
            .Include(c => c.Product)
            .Where(c => c.UserId == userId)
            .ToListAsync();

        if (cartItems.Count == 0)
            throw new InvalidOperationException("Cart is empty.");

        foreach (var item in cartItems)
        {
            var inventory = await _db.Inventories
                .FirstOrDefaultAsync(i => i.ProductId == item.ProductId);

            if (inventory == null || inventory.StockQuantity < item.Quantity)
                throw new InvalidOperationException(
                    $"Insufficient stock for {item.Product!.Name}."
                );

            inventory.StockQuantity -= item.Quantity;
            inventory.LastUpdated = DateTime.UtcNow;
        }

        var order = new Order
        {
            UserId = userId,
            Address = dto.Address,
            Notes = dto.Notes,
            CouponCode = dto.CouponCode,
            TotalAmount = cartItems.Sum(c => c.Quantity * c.Product!.Price),
            OrderItems = cartItems.Select(c => new OrderItem
            {
                ProductId = c.ProductId,
                Quantity = c.Quantity,
                UnitPrice = c.Product!.Price
            }).ToList()
        };

        _db.Orders.Add(order);
        _db.CartItems.RemoveRange(cartItems);

        await _db.SaveChangesAsync();

        var user = await _db.Users.FindAsync(userId);
        if (user != null)
        {
            await _emailService.SendOrderConfirmationAsync(
                user.Email,
                order.Id,
                order.TotalAmount
            );
        }

        return MapToDto(order);
    }

    public async Task<List<OrderDto>> GetUserOrdersAsync(int userId)
    {
        return await _db.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.OrderedAt)
            .Select(o => MapToDto(o))
            .ToListAsync();
    }

    public async Task<OrderDto?> GetOrderByIdAsync(int id, int userId)
    {
        var order = await _db.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

        return order == null ? null : MapToDto(order);
    }

    public async Task<OrderDto> CancelOrderAsync(int id, int userId)
    {
        var order = await _db.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId)
            ?? throw new InvalidOperationException("Order not found.");

        if (order.Status != "Pending")
            throw new InvalidOperationException(
                "Only pending orders can be cancelled."
            );

        order.Status = "Cancelled";

        foreach (var item in order.OrderItems)
        {
            var inventory = await _db.Inventories
                .FirstOrDefaultAsync(i => i.ProductId == item.ProductId);

            if (inventory != null)
                inventory.StockQuantity += item.Quantity;
        }

        await _db.SaveChangesAsync();

        return MapToDto(order);
    }

    public async Task<OrderDto> UpdateStatusAsync(int id, string status)
    {
        var order = await _db.Orders.FindAsync(id)
            ?? throw new InvalidOperationException("Order not found.");

        order.Status = status;

        if (status == "Delivered")
            order.DeliveredAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return MapToDto(order);
    }

    private static OrderDto MapToDto(Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            UserId = order.UserId,
            Status = order.Status,
            TotalAmount = order.TotalAmount,
            Address = order.Address,
            Notes = order.Notes,
            CouponCode = order.CouponCode,
            Discount = order.Discount,
            OrderedAt = order.OrderedAt,
            DeliveredAt = order.DeliveredAt,
            Items = order.OrderItems.Select(oi => new OrderItemDto
            {
                ProductId = oi.ProductId,
                ProductName = oi.Product?.Name ?? "",
                Quantity = oi.Quantity,
                UnitPrice = oi.UnitPrice
            }).ToList()
        };
    }
}