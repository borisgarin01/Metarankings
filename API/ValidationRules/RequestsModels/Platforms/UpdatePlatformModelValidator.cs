using API.Models.RequestsModels.Games.Platforms;
using FluentValidation;

namespace API.ValidationRules.RequestsModels.Platforms;

public class UpdatePlatformModelValidator : AbstractValidator<UpdatePlatformModel>
{
    public UpdatePlatformModelValidator()
    {
        RuleFor(a => a.Name)
            .NotNull().WithMessage("Name should be not null")
            .NotEmpty().WithMessage("Name should be not empty")
            .MaximumLength(255).WithMessage("Name should be shorter than 255 symbols");
    }
}
