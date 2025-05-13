using API.Models.RequestsModels.Localizations;
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

        RuleFor(a => a.Href)
            .NotNull().WithMessage("Href should be not null")
            .NotEmpty().WithMessage("Please enter Href")
            .MaximumLength(1023).WithMessage("Href should be shorter than 1023 symbols");
    }
}
