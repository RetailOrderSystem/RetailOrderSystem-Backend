using Microsoft.EntityFrameworkCore;
using RetailOrderSystem.API.Data;
using RetailOrderSystem.API.DTOs;
using RetailOrderSystem.API.Models;
using RetailOrderSystem.API.Services.Interfaces;

namespace RetailOrderSystem.API.Services
{
    public class CartService : ICartService
    {
        private readonly AppDbContext _db;

        public CartService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<CartDto> GetCartAsync(int userId)
        {
            var cartItems = await _db.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            return MapToCartDto(userId, cartItems);
        }

        public async Task<CartDto> AddToCartAsync(int userId, AddToCartDto dto)
        {
            var existingItem = await _db.CartItems
                .FirstOrDefaultAsync(c =>
                    c.UserId == userId &&
                    c.ProductId == dto.ProductId);

            if (existingItem != null)
            {
                existingItem.Quantity += dto.Quantity;
            }
            else
            {
                var cartItem = new CartItem
                {
                    UserId = userId,
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity
                };

                _db.CartItems.Add(cartItem);
            }

            await _db.SaveChangesAsync();

            return await GetCartAsync(userId);
        }

        public async Task<CartDto> UpdateQuantityAsync(
            int userId,
            int productId,
            int quantity)
        {
            var item = await _db.CartItems
                .FirstOrDefaultAsync(c =>
                    c.UserId == userId &&
                    c.ProductId == productId)
                ?? throw new InvalidOperationException("Cart item not found.");

            item.Quantity = quantity;

            await _db.SaveChangesAsync();

            return await GetCartAsync(userId);
        }

        public async Task RemoveFromCartAsync(int userId, int productId)
        {
            var item = await _db.CartItems
                .FirstOrDefaultAsync(c =>
                    c.UserId == userId &&
                    c.ProductId == productId);

            if (item != null)
            {
                _db.CartItems.Remove(item);
                await _db.SaveChangesAsync();
            }
        }

        public async Task ClearCartAsync(int userId)
        {
            var items = await _db.CartItems
                .Where(c => c.UserId == userId)
                .ToListAsync();

            _db.CartItems.RemoveRange(items);
            await _db.SaveChangesAsync();
        }

        private CartDto MapToCartDto(int userId, List<CartItem> cartItems)
        {
            return new CartDto
            {
                UserId = userId,
                Items = cartItems.Select(c => new CartItemDto
                {
                    ProductId = c.ProductId,
                    ProductName = c.Product?.Name ?? string.Empty,
                    Quantity = c.Quantity,
                    Price = c.Product?.Price ?? 0
                }).ToList(),
                TotalAmount = cartItems.Sum(c =>
                    c.Quantity * (c.Product?.Price ?? 0))
            };
        }
    }
}