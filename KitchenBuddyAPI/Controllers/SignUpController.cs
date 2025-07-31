using KitchenBuddyAPI.Data;
using KitchenBuddyAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.AspNetCore.Identity;

namespace KitchenBuddyAPI.Controllers;

[ApiController]
[Route("signUp")]
public class SignUpController : ControllerBase
{
    private readonly KitchenBuddyDbContext _context;
    public SignUpController(KitchenBuddyDbContext context)
    {
        _context = context;
    }
    [HttpPost("signin")]
    public async Task<IActionResult> SignUp([FromBody] SignUpRequest request) 
    {
        if (string.IsNullOrEmpty(request.Name) || string.IsNullOrEmpty(request.Surname) ||
            string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password)) {
                return BadRequest("Invalid parameters");
            }

        // Check if user already exists (by email)
        if (_context.Users.Any(u => u.Email == request.Email))
        {
            return Conflict("User with this email already exists.");
        }

        var user = new User
        {
            Name = request.Name,
            Surname = request.Surname,
            Email = request.Email,
            Usertype = "customer" // or set as needed
        };

        var hasher = new PasswordHasher<User>();
        user.PasswordHash = hasher.HashPassword(user, request.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok("User registered successfully.");
    }
}

public class SignUpRequest
{
    public string Name { set; get; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { set; get; } = string.Empty;
}