using API.Models.RequestsModels.Games.Localizations;
using FluentValidation;

namespace API.ValidationRules.RequestsModels.Localizations;

public class UpdateLocalizationModelValidator : AbstractValidator<UpdateLocalizationModel>
{
    public UpdateLocalizationModelValidator()
    {
        RuleFor(a => a.Name)
            .NotNull().WithMessage("Name should be not null")
            .NotEmpty().WithMessage("Name should be not empty")
            .MaximumLength(255).WithMessage("Name should be shorter than 255 symbols");
    }
}
