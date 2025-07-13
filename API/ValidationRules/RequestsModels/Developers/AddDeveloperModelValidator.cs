using API.Models.RequestsModels.Games.Developers;

namespace API.ValidationRules.RequestsModels.Developers;

public sealed class AddDeveloperModelValidator : AbstractValidator<AddDeveloperModel>
{
    public AddDeveloperModelValidator()
    {
        RuleFor(a => a.Name).NotEmpty().WithMessage("Please enter name");
        RuleFor(a => a.Name).MaximumLength(511).WithMessage("Name should be shorter than 512 symbols");
        RuleFor(a => a.Name).MinimumLength(1).WithMessage("Name should be not empty");
    }
}
