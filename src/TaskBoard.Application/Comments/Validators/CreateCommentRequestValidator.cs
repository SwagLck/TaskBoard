using FluentValidation;
using TaskBoard.Application.Comments.Dtos;

namespace TaskBoard.Application.Comments.Validators;

public class CreateCommentRequestValidator : AbstractValidator<CreateCommentRequest>
{
    public CreateCommentRequestValidator()
    {
        RuleFor(x => x.Text).NotEmpty().MaximumLength(2000);
    }
}