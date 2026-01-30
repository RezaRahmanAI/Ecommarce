namespace Ecommarce.Application.Features.Authentication.Commands.Login;

using Ecommarce.Application.Common.Interfaces;
using Ecommarce.Application.Features.Authentication.DTOs;
using FluentValidation;
using MediatR;

public record LoginCommand : IRequest<AuthenticationResult>
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required");
    }
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthenticationResult>
{
    private readonly IIdentityService _identityService;
    private readonly IJwtTokenGenerator _tokenGenerator;

    public LoginCommandHandler(
        IIdentityService identityService,
        IJwtTokenGenerator tokenGenerator)
    {
        _identityService = identityService;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<AuthenticationResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var (success, userId, email, firstName, lastName, roles) = await _identityService.AuthenticateAsync(request.Email, request.Password);
        
        if (!success)
        {
            throw new Common.Exceptions.ValidationException(
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
