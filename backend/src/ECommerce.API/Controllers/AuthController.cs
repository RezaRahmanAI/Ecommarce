using ECommerce.Core.DTOs;
using ECommerce.Core.Interfaces;
using ECommerce.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITokenService _tokenService;

    public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ITokenService tokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);

        if (user == null) return Unauthorized("Invalid email or password");

        var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

        if (!result.Succeeded) return Unauthorized("Invalid email or password");

        return new AuthResponseDto
        {
            Token = _tokenService.CreateToken(user),
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email!,
                Name = user.FullName ?? user.UserName!,
                Role = "admin" // TODO: Get actual role from user manager if needed
            }
        };
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto registerDto)
    {
        if (await _userManager.FindByEmailAsync(registerDto.Email) != null)
        {
            return BadRequest("Email is already in use");
        }

        var user = new ApplicationUser
        {
            FullName = registerDto.FullName,
            Email = registerDto.Email,
            UserName = registerDto.Email
        };

        var result = await _userManager.CreateAsync(user, registerDto.Password);

        if (!result.Succeeded) return BadRequest(result.Errors);

        return new AuthResponseDto
        {
            Token = _tokenService.CreateToken(user),
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.FullName,
                Role = "user" // Default role for new users
            }
        };
    }
}
