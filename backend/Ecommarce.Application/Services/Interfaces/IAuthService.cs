namespace Ecommarce.Application.Services.Interfaces;

using Ecommarce.Application.Features.Authentication.Commands.Login;
using Ecommarce.Application.Features.Authentication.Commands.Register;
using Ecommarce.Application.Features.Authentication.DTOs;

public interface IAuthService
{
    Task<AuthenticationResult> RegisterAsync(RegisterCommand command);
    Task<AuthenticationResult> LoginAsync(LoginCommand command);
}
