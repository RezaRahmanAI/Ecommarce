using System.ComponentModel.DataAnnotations;

namespace ECommerce.Core.DTOs;

public class LoginDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
}

public class RegisterDto
{
    [Required]
    public string FullName { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [MinLength(6)]
    public string Password { get; set; }
}

public class AuthResponseDto
{
    public string Token { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
}
