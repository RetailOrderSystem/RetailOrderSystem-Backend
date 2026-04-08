using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RetailOrderSystem.API.Data;
using RetailOrderSystem.API.DTOs;
using RetailOrderSystem.API.Models;
using RetailOrderSystem.API.Services.Interfaces;

namespace RetailOrderSystem.API.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;

    public AuthService(AppDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        if (await _db.Users.AnyAsync(u => u.Email == dto.Email))
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "Email already registered."
            };
        }

        var user = new User
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Phone = dto.Phone,
            Address = dto.Address,
            Role = "Customer",
            IsActive = true
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return new AuthResponseDto
        {
            Success = true,
            Token = GenerateToken(user),
            Message = "Registered successfully."
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Email == dto.Email && u.IsActive);

        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "Invalid credentials."
            };
        }

        return new AuthResponseDto
        {
            Success = true,
            Token = GenerateToken(user),
            Message = "Login successful.",
            UserId = user.Id,
            Role = user.Role,
            FullName = $"{user.FirstName} {user.LastName}"
        };
    }

    private string GenerateToken(User user)
    {
        var jwtSettings = _config.GetSection("JwtSettings");

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!)
        );

        var creds = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256
        );

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}")
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(
                double.Parse(jwtSettings["ExpirationHours"]!)
            ),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}