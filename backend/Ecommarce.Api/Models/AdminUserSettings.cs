namespace Ecommarce.Api.Models;

public sealed class AdminUserSettings
{
    public const string SectionName = "AdminUser";

    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string FirstName { get; init; } = "Admin";
    public string LastName { get; init; } = "User";
}
