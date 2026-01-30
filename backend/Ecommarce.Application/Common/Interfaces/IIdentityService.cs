namespace Ecommarce.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<(bool Success, string UserId, string Email, string? FirstName, string? LastName, IList<string> Roles)> 
        AuthenticateAsync(string email, string password);
    
    Task<(bool Success, string UserId, string Email, string FirstName, string LastName, IList<string> Roles, string? Error)> 
        RegisterAsync(string email, string password, string firstName, string lastName, string? phoneNumber);
}
