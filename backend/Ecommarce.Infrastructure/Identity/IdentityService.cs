namespace Ecommarce.Infrastructure.Identity;

using Ecommarce.Application.Common.Interfaces;
using Ecommarce.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<(bool Success, string UserId, string Email, string? FirstName, string? LastName, IList<string> Roles)> 
        AuthenticateAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        
        if (user == null)
        {
            return (false, string.Empty, string.Empty, null, null, Array.Empty<string>());
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: false);

        if (!result.Succeeded)
        {
            return (false, string.Empty, string.Empty, null, null, Array.Empty<string>());
        }

        user.LastLoginAt = DateTime.UtcNow;
       await _userManager.UpdateAsync(user);

        var roles = await _userManager.GetRolesAsync(user);

        return (true, user.Id, user.Email!, user.FirstName, user.LastName, roles);
    }

    public async Task<(bool Success, string UserId, string Email, string FirstName, string LastName, IList<string> Roles, string? Error)> 
        RegisterAsync(string email, string password, string firstName, string lastName, string? phoneNumber)
    {
        // Check if user already exists
        var existingUser = await _userManager.FindByEmailAsync(email);
        if (existingUser != null)
        {
            return (false, string.Empty, string.Empty, string.Empty, string.Empty, Array.Empty<string>(), "Email already registered");
        }

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            PhoneNumber = phoneNumber,
            EmailConfirmed = true // Auto-confirm for now
        };

        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return (false, string.Empty, string.Empty, string.Empty, string.Empty, Array.Empty<string>(), errors);
        }

        // Assign Customer role by default
        await _userManager.AddToRoleAsync(user, "Customer");

        var roles = await _userManager.GetRolesAsync(user);

        return (true, user.Id, user.Email!, user.FirstName, user.LastName, roles, null);
    }
}
