using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailOrderSystem.API.DTOs;
using RetailOrderSystem.API.Services.Interfaces;

namespace RetailOrderSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController(IOrderService orderService) : ControllerBase
{
    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
    public async Task<IActionResult> PlaceOrder([FromBody] CreateOrderDto dto)
        => Ok(await orderService.PlaceOrderAsync(UserId, dto));

    [HttpGet]
    public async Task<IActionResult> GetMyOrders()
        => Ok(await orderService.GetUserOrdersAsync(UserId));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrder(int id)
    {
        var order = await orderService.GetOrderByIdAsync(id, UserId);
        return order is null ? NotFound() : Ok(order);
    }

    [HttpPut("{id}/cancel")]
    public async Task<IActionResult> Cancel(int id)
        => Ok(await orderService.CancelOrderAsync(id, UserId));

    [HttpPut("{id}/status"), Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
        => Ok(await orderService.UpdateStatusAsync(id, status));
}