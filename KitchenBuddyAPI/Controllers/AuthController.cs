using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using KitchenBuddyAPI.Models;
using KitchenBuddyAPI.Helpers;
using Microsoft.EntityFrameworkCore;
using KitchenBuddyAPI.Data;
using Microsoft.AspNetCore.Identity;

namespace KitchenBuddyAPI.Controllers;

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResponse
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string Usertype { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}

// NOTE: Make sure to seed the Users table with at least one admin and one customer user for login to work.
[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly KitchenBuddyDbContext _db;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IConfiguration config, KitchenBuddyDbContext db, ILogger<AuthController> logger)
    {
        _config = config;
        _db = db;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                _logger.LogWarning("Login attempt with missing credentials");
                return BadRequest(ApiResponse<LoginResponse>.ErrorResult("Username and password are required."));
            }

            // Find user by username or email
            var user = await _db.Users.FirstOrDefaultAsync(u => 
                u.Username == request.Username || u.Email == request.Username);

            if (user == null)
            {
                _logger.LogWarning("Login attempt with invalid username/email: {Username}", request.Username);
                return Unauthorized(ApiResponse<LoginResponse>.ErrorResult("Invalid credentials."));
            }

            // Verify password
            var hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
            
            if (result == PasswordVerificationResult.Failed)
            {
                _logger.LogWarning("Login attempt with invalid password for user: {Username}", user.Username);
                return Unauthorized(ApiResponse<LoginResponse>.ErrorResult("Invalid credentials."));
            }

            // Update last login time
            user.LastLoginAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            // Generate JWT token
            var token = JwtHelper.GenerateJwtToken(user.Username, user.Usertype, _config);

            // Create response
            var response = new LoginResponse
            {
                UserId = user.Id,
                Username = user.Username,
                Email = user.Email,
                Name = user.Name,
                Surname = user.Surname,
                Usertype = user.Usertype,
                Token = token
            };

            _logger.LogInformation("User logged in successfully: {Username}", user.Username);
            return Ok(ApiResponse<LoginResponse>.SuccessResult(response, "Login successful"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            return StatusCode(500, ApiResponse<LoginResponse>.ErrorResult("An error occurred during login. Please try again."));
        }
    }
}