using API.Models.RequestsModels.Games.Genres;
using FluentValidation;

namespace API.ValidationRules.RequestsModels.Genres;

public class AddGenreModelValidator : AbstractValidator<AddGenreModel>
{
    public AddGenreModelValidator()
    {
        RuleFor(a => a.Name)
            .NotNull().WithMessage("Name should be not null")
            .NotEmpty().WithMessage("Name should be not empty")
            .MaximumLength(255).WithMessage("Name should be shorter than 255 symbols");
    }
}
