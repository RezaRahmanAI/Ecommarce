namespace Ecommarce.Application.Features.Authentication.Commands.Register;

using Ecommarce.Application.Common.Interfaces;
using Ecommarce.Application.Features.Authentication.DTOs;
using FluentValidation;
using MediatR;

public record RegisterCommand : IRequest<AuthenticationResult>
{
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public string? PhoneNumber { get; init; }
}

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(50).WithMessage("First name cannot exceed 50 characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters");
    }
}

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthenticationResult>
{
    private readonly IIdentityService _identityService;
    private readonly IJwtTokenGenerator _tokenGenerator;

    public RegisterCommandHandler(
        IIdentityService identityService,
        IJwtTokenGenerator tokenGenerator)
    {
        _identityService = identityService;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<AuthenticationResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var (success, userId, email, firstName, lastName, roles, error) = await _identityService.RegisterAsync(
            request.Email,
            request.Password,
            request.FirstName,
            request.LastName,
            request.PhoneNumber);

        if (!success)
        {
            throw new Common.Exceptions.ValidationException(
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
}
