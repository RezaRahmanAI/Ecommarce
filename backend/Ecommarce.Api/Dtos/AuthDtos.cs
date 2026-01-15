using System.ComponentModel.DataAnnotations;

namespace Ecommarce.Api.Dtos;

public sealed class RegisterRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    public string? FullName { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    [Required]
    public string Password { get; set; } = string.Empty;
}

public sealed class LoginRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

public sealed class AuthResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; set; }
    public AuthUser User { get; set; } = new();
}

public sealed class AuthUser
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = "user";
}
