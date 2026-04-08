using Microsoft.AspNetCore.Mvc;
using RetailOrderSystem.API.DTOs;
using RetailOrderSystem.API.Services.Interfaces;

namespace RetailOrderSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var result = await _authService.RegisterAsync(dto);

        if (result.Success)
            return Ok(result);

        return BadRequest(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);

        if (result.Success)
            return Ok(result);

        return Unauthorized(result);
    }
}