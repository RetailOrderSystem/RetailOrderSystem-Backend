using RetailOrderSystem.API.DTOs;

namespace RetailOrderSystem.API.Services.Interfaces
{
    public interface ICartService
    {
        Task<CartDto> GetCartAsync(int userId);

        Task<CartDto> AddToCartAsync(int userId, AddToCartDto dto);

        Task<CartDto> UpdateQuantityAsync(
            int userId,
            int productId,
            int quantity);

        Task RemoveFromCartAsync(int userId, int productId);

        Task ClearCartAsync(int userId);
    }
}