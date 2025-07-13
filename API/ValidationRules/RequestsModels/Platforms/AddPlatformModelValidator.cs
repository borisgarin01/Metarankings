using API.Models.RequestsModels.Games.Platforms;

namespace API.ValidationRules.RequestsModels.Platforms;

public class AddPlatformModelValidator : AbstractValidator<AddPlatformModel>
{
    public AddPlatformModelValidator()
    {
        RuleFor(a => a.Name)
            .NotNull().WithMessage("Name should be not null")
            .NotEmpty().WithMessage("Name should be not empty")
            .MaximumLength(255).WithMessage("Name should be shorter than 255 symbols");
    }
}
