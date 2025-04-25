using API.Models.RequestsModels.Publishers;
using FluentValidation;

namespace API.ValidationRules.RequestsModels.Publishers;

public class UpdatePublisherModelValidator : AbstractValidator<UpdatePublisherModel>
{
    public UpdatePublisherModelValidator()
    {
        RuleFor(a => a.Name).NotNull().WithMessage("Name should be not null")
            .NotEmpty().WithMessage("Name should be not empty")
            .MaximumLength(511).WithMessage("Name should be shorter 511 symbols");

        RuleFor(a => a.Url).NotNull().WithMessage("URL should be not null")
            .NotEmpty().WithMessage("URL should be not empty")
            .MaximumLength(1023).WithMessage("URL should be shorter 511 symbols");
    }
}
