using Ecommarce.Api.Dtos;
using Ecommarce.Api.Models;
using Ecommarce.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Ecommarce.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITokenService _tokenService;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ITokenService tokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public ActionResult<AuthResponse> Register(RegisterRequest request)
    {
        return StatusCode(StatusCodes.Status403Forbidden, new { message = "Customer registration is disabled." });
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
        {
            return Unauthorized();
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded)
        {
            return Unauthorized();
        }

        if (!await _userManager.IsInRoleAsync(user, "admin"))
        {
            return Unauthorized("Admin access required.");
        }

        var (token, expiresAt) = _tokenService.CreateToken(user);
        var response = await BuildResponse(user, token, expiresAt);
        return Ok(response);
    }

    private async Task<AuthResponse> BuildResponse(ApplicationUser user, string token, DateTimeOffset expiresAt)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? "user";
        var name = $"{user.FirstName} {user.LastName}".Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            name = user.Email ?? string.Empty;
        }

        return new AuthResponse
        {
            AccessToken = token,
            ExpiresAt = expiresAt,
            User = new AuthUser
            {
                Id = user.Id,
                Name = name,
                Email = user.Email ?? string.Empty,
                Role = role
            }
        };
    }

}
