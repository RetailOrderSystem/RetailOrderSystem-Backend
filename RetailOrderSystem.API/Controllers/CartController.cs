using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailOrderSystem.API.DTOs;
using RetailOrderSystem.API.Services.Interfaces;

namespace RetailOrderSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    private int UserId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var cart = await _cartService.GetCartAsync(UserId);
        return Ok(cart);
    }

    [HttpPost]
    public async Task<IActionResult> AddToCart([FromBody] AddToCartDto dto)
    {
        var result = await _cartService.AddToCartAsync(UserId, dto);
        return Ok(result);
    }

    [HttpPut("{productId}")]
    public async Task<IActionResult> UpdateQuantity(
        int productId,
        [FromBody] int quantity)
    {
        var result = await _cartService.UpdateQuantityAsync(
            UserId,
            productId,
            quantity);

        return Ok(result);
    }

    [HttpDelete("{productId}")]
    public async Task<IActionResult> RemoveItem(int productId)
    {
        await _cartService.RemoveFromCartAsync(UserId, productId);
        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> ClearCart()
    {
        await _cartService.ClearCartAsync(UserId);
        return NoContent();
    }
}