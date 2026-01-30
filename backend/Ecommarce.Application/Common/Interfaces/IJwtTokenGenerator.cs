namespace Ecommarce.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(string userId, string email, string? firstName, string? lastName, IList<string> roles);
}
