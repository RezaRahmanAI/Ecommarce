namespace Ecommarce.Api.Controllers;

using Ecommarce.Application.Features.Authentication.Commands.Login;
using Ecommarce.Application.Features.Authentication.Commands.Register;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly Application.Services.Interfaces.IAuthService _authService;

    public AuthController(Application.Services.Interfaces.IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Register a new customer account
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command)
    {
        var result = await _authService.RegisterAsync(command);
        return Ok(result);
    }

    /// <summary>
    /// Login with email and password
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await _authService.LoginAsync(command);
        return Ok(result);
    }

    /// <summary>
    /// Get current user information (requires authentication)
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    public IActionResult GetCurrentUser()
    {
        return Ok(new
        {
            UserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
            Email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value,
            FirstName = User.FindFirst(System.Security.Claims.ClaimTypes.GivenName)?.Value,
            LastName = User.FindFirst(System.Security.Claims.ClaimTypes.Surname)?.Value,
            Roles = User.FindAll(System.Security.Claims.ClaimTypes.Role).Select(c => c.Value).ToList()
        });
    }
}
