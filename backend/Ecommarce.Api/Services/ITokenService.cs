using Ecommarce.Api.Models;

namespace Ecommarce.Api.Services;

public interface ITokenService
{
    (string Token, DateTimeOffset ExpiresAt) CreateToken(ApplicationUser user);
}
