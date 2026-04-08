using RetailOrderSystem.API.DTOs;

namespace RetailOrderSystem.API.Services.Interfaces
{
    public interface IOrderService
    {
        Task<OrderDto> PlaceOrderAsync(int userId, CreateOrderDto dto);

        Task<List<OrderDto>> GetUserOrdersAsync(int userId);

        Task<OrderDto?> GetOrderByIdAsync(int id, int userId);

        Task<OrderDto> CancelOrderAsync(int id, int userId);

        Task<OrderDto> UpdateStatusAsync(int id, string status);
    }
}