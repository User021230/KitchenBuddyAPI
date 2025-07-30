using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using KitchenBuddyAPI.Models;
using Microsoft.EntityFrameworkCore;
using KitchenBuddyAPI.Data;

namespace KitchenBuddyAPI.Controllers;

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

// NOTE: Make sure to seed the Users table with at least one admin and one customer user for login to work.
[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly KitchenBuddyDbContext _db;
    public AuthController(IConfiguration config, KitchenBuddyDbContext db)
    {
        _config = config;
        _db = db;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest("Username and password are required.");

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == request.Username && u.Password == request.Password);
        if (user == null)
            return Unauthorized("Invalid credentials.");

        var tokenString = GenerateJwtToken(user.Username, user.UserType);
        return Ok(new { token = tokenString });
    }

    private string GenerateJwtToken(string username, string usertype)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(ClaimTypes.Role, usertype)
        };
        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_config["Jwt:Secret"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(3),
            signingCredentials: creds
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}