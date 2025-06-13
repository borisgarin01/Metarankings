using API.Models.RequestsModels.Games.Publishers;
using FluentValidation;

namespace API.ValidationRules.RequestsModels.Publishers;

public class AddPublisherModelValidator : AbstractValidator<AddPublisherModel>
{
    public AddPublisherModelValidator()
    {
        RuleFor(a => a.Name).NotNull().WithMessage("Name should be not null")
            .NotEmpty().WithMessage("Name should be not empty")
            .MaximumLength(511).WithMessage("Name should be shorter 511 symbols");
    }
}
