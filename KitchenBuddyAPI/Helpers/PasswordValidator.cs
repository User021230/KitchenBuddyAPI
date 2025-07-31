using System.Text.RegularExpressions;

namespace KitchenBuddyAPI.Helpers;

public static class PasswordValidator
{
    public static (bool IsValid, List<string> Errors) ValidatePassword(string password)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(password))
        {
            errors.Add("Password is required.");
            return (false, errors);
        }

        if (password.Length < 8)
        {
            errors.Add("Password must be at least 8 characters long.");
        }

        if (password.Length > 128)
        {
            errors.Add("Password must not exceed 128 characters.");
        }

        if (!Regex.IsMatch(password, @"[A-Z]"))
        {
            errors.Add("Password must contain at least one uppercase letter.");
        }

        if (!Regex.IsMatch(password, @"[a-z]"))
        {
            errors.Add("Password must contain at least one lowercase letter.");
        }

        if (!Regex.IsMatch(password, @"\d"))
        {
            errors.Add("Password must contain at least one number.");
        }

        if (!Regex.IsMatch(password, @"[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]"))
        {
            errors.Add("Password must contain at least one special character.");
        }

        return (errors.Count == 0, errors);
    }
} 