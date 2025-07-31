using KitchenBuddyAPI.Data;
using KitchenBuddyAPI.Models;
using KitchenBuddyAPI.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace KitchenBuddyAPI.Controllers;

[ApiController]
[Route("signUp")]
public class SignUpController : ControllerBase
{
    private readonly KitchenBuddyDbContext _context;
    private readonly IConfiguration _config;
    private readonly ILogger<SignUpController> _logger;

    public SignUpController(KitchenBuddyDbContext context, IConfiguration config, ILogger<SignUpController> logger)
    {
        _context = context;
        _config = config;
        _logger = logger;
    }

    [HttpPost("signUp")]
    public async Task<IActionResult> SignUp([FromBody] SignUpRequest request)
    {
        try
        {
            // Validate request
            var validationResult = ValidateSignUpRequest(request);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Sign-up validation failed: {Errors}", string.Join(", ", validationResult.Errors));
                return BadRequest(ApiResponse<SignUpResponse>.ErrorResult("Validation failed", validationResult.Errors));
            }

            // Check if user already exists
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                _logger.LogWarning("Sign-up attempt with existing email: {Email}", request.Email);
                return Conflict(ApiResponse<SignUpResponse>.ErrorResult("User with this email already exists."));
            }

            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
            {
                _logger.LogWarning("Sign-up attempt with existing username: {Username}", request.Username);
                return Conflict(ApiResponse<SignUpResponse>.ErrorResult("Username is already taken."));
            }

            // Create new user
            var user = new User
            {
                Name = SanitizeInput(request.Name),
                Surname = SanitizeInput(request.Surname),
                Email = request.Email.ToLowerInvariant(),
                Username = request.Username.ToLowerInvariant(),
                Usertype = "customer", // Default user type
                IsEmailVerified = false,
                CreatedAt = DateTime.UtcNow
            };

            // Hash password
            var hasher = new PasswordHasher<User>();
            user.PasswordHash = hasher.HashPassword(user, request.Password);

            // Save to database
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Generate JWT token for auto-login
            var token = JwtHelper.GenerateJwtToken(user.Username, user.Usertype, _config);

            // Create response
            var response = new SignUpResponse
            {
                UserId = user.Id,
                Username = user.Username,
                Email = user.Email,
                Name = user.Name,
                Surname = user.Surname,
                Usertype = user.Usertype,
                Token = token
            };

            _logger.LogInformation("User registered successfully: {Username}", user.Username);
            return Ok(ApiResponse<SignUpResponse>.SuccessResult(response, "User registered successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user registration");
            return StatusCode(500, ApiResponse<SignUpResponse>.ErrorResult("An error occurred during registration. Please try again."));
        }
    }

    private (bool IsValid, List<string> Errors) ValidateSignUpRequest(SignUpRequest request)
    {
        var errors = new List<string>();

        // Basic validation
        if (request == null)
        {
            errors.Add("Request body is required.");
            return (false, errors);
        }

        // Name validation
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            errors.Add("Name is required.");
        }
        else if (request.Name.Length < 2 || request.Name.Length > 50)
        {
            errors.Add("Name must be between 2 and 50 characters.");
        }

        // Surname validation
        if (string.IsNullOrWhiteSpace(request.Surname))
        {
            errors.Add("Surname is required.");
        }
        else if (request.Surname.Length < 2 || request.Surname.Length > 50)
        {
            errors.Add("Surname must be between 2 and 50 characters.");
        }

        // Email validation
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            errors.Add("Email is required.");
        }
        else if (!IsValidEmail(request.Email))
        {
            errors.Add("Please provide a valid email address.");
        }

        // Username validation
        if (string.IsNullOrWhiteSpace(request.Username))
        {
            errors.Add("Username is required.");
        }
        else if (request.Username.Length < 3 || request.Username.Length > 30)
        {
            errors.Add("Username must be between 3 and 30 characters.");
        }
        else if (!Regex.IsMatch(request.Username, @"^[a-zA-Z0-9_-]+$"))
        {
            errors.Add("Username can only contain letters, numbers, hyphens, and underscores.");
        }

        // Password validation
        var passwordValidation = PasswordValidator.ValidatePassword(request.Password);
        if (!passwordValidation.IsValid)
        {
            errors.AddRange(passwordValidation.Errors);
        }

        // Password confirmation
        if (string.IsNullOrWhiteSpace(request.ConfirmPassword))
        {
            errors.Add("Password confirmation is required.");
        }
        else if (request.Password != request.ConfirmPassword)
        {
            errors.Add("Passwords do not match.");
        }

        return (errors.Count == 0, errors);
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    private string SanitizeInput(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        // Remove potentially dangerous characters
        return System.Net.WebUtility.HtmlEncode(input.Trim());
    }
}

public class SignUpRequest
{
    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string Surname { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(30, MinimumLength = 3)]
    [RegularExpression(@"^[a-zA-Z0-9_-]+$", ErrorMessage = "Username can only contain letters, numbers, hyphens, and underscores.")]
    public string Username { get; set; } = string.Empty;

    [Required]
    [StringLength(128, MinimumLength = 8)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}