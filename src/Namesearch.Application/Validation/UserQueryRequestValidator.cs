using FluentValidation;
using Namesearch.Application.Contracts;

namespace Namesearch.Application.Validation;

public sealed class UserQueryRequestValidator : AbstractValidator<UserQueryRequest>
{
    public UserQueryRequestValidator()
    {
        RuleFor(x => x.Query)
            .NotEmpty()
            .MinimumLength(2)
            .WithMessage("Query must be at least 2 characters.");

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100);
    }
}
