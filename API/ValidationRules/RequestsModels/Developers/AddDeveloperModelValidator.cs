using API.Models.RequestsModels.Games.Developers;
using FluentValidation;

namespace API.ValidationRules.RequestsModels.Developers;

public sealed class AddDeveloperModelValidator : AbstractValidator<AddDeveloperModel>
{
    public AddDeveloperModelValidator()
    {
        RuleFor(a => a.Name).NotEmpty().WithMessage("Please enter name");
        RuleFor(a => a.Name).MaximumLength(511).WithMessage("Name should be shorter than 512 symbols");
        RuleFor(a => a.Name).MinimumLength(1).WithMessage("Name should be not empty");

        RuleFor(a => a.Url).NotEmpty().WithMessage("Please enter URL");
        RuleFor(a => a.Url).MaximumLength(1023).WithMessage("URL should be shorter than 1023 symbols");
        RuleFor(a => a.Url).MinimumLength(1).WithMessage("URL should be not empty");
    }
}
