using API.Models.RequestsModels.Games.Genres;
using FluentValidation;

namespace API.ValidationRules.RequestsModels.Genres;

public class UpdateGenreModelValidator : AbstractValidator<UpdateGenreModel>
{
    public UpdateGenreModelValidator()
    {
        RuleFor(a => a.Name)
            .NotNull().WithMessage("Name should be not null")
            .NotEmpty().WithMessage("Name should be not empty")
            .MaximumLength(255).WithMessage("Name should be shorter than 255 symbols");

        RuleFor(a => a.Url)
            .NotNull().WithMessage("URL should be not null")
            .NotEmpty().WithMessage("Please enter URL")
            .MaximumLength(1023).WithMessage("URL should be shorter than 1023 symbols");
    }
}
