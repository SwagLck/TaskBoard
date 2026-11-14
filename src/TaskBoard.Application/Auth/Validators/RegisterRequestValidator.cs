using FluentValidation;
using TaskBoard.Application.Auth.Dtos;

namespace TaskBoard.Application.Auth.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8).MaximumLength(72);
        RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(100);
    }
}