using API.Models.RequestsModels.Games.Developers;
using FluentValidation;

namespace API.ValidationRules.RequestsModels.Developers;

public sealed class UpdateDeveloperModelValidator : AbstractValidator<UpdateDeveloperModel>
{
    public UpdateDeveloperModelValidator()
    {
        RuleFor(a => a.Name).NotEmpty().WithMessage("Please enter name");
        RuleFor(a => a.Name).MaximumLength(511).WithMessage("Name should be shorter than 512 symbols");
        RuleFor(a => a.Name).MinimumLength(1).WithMessage("Name should be not empty");
    }
}
