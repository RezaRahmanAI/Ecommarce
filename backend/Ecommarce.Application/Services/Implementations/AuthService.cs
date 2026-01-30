namespace Ecommarce.Application.Services.Implementations;

using Ecommarce.Application.Common.Exceptions;
using Ecommarce.Application.Common.Interfaces;
using Ecommarce.Application.Features.Authentication.Commands.Login;
using Ecommarce.Application.Features.Authentication.Commands.Register;
using Ecommarce.Application.Features.Authentication.DTOs;
using Ecommarce.Application.Services.Interfaces;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class AuthService : IAuthService
{
    private readonly IIdentityService _identityService;
    private readonly IJwtTokenGenerator _tokenGenerator;

    public AuthService(
        IIdentityService identityService,
        IJwtTokenGenerator tokenGenerator)
    {
        _identityService = identityService;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<AuthenticationResult> RegisterAsync(RegisterCommand request)
    {
        var (success, userId, email, firstName, lastName, roles, error) = await _identityService.RegisterAsync(
            request.Email,
            request.Password,
            request.FirstName,
            request.LastName,
            request.PhoneNumber);

        if (!success)
        {
            throw new ValidationException(
                new[] { new FluentValidation.Results.ValidationFailure("Registration", error ?? "Registration failed") });
        }

        var token = _tokenGenerator.GenerateToken(userId, email, firstName, lastName, roles);

        return new AuthenticationResult
        {
            Token = token,
            UserId = userId,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            Roles = roles.ToList()
        };
    }

    public async Task<AuthenticationResult> LoginAsync(LoginCommand request)
    {
        var (success, userId, email, firstName, lastName, roles) = await _identityService.AuthenticateAsync(request.Email, request.Password);
        
        if (!success)
        {
            throw new ValidationException(
                new[] { new FluentValidation.Results.ValidationFailure("Credentials", "Invalid email or password") });
        }

        var token = _tokenGenerator.GenerateToken(userId, email, firstName, lastName, roles);

        return new AuthenticationResult
        {
            Token = token,
            UserId = userId,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            Roles = roles.ToList()
        };
    }
}
