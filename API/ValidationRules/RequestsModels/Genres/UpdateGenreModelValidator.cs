using API.Models.RequestsModels.Games.Genres;

namespace API.ValidationRules.RequestsModels.Genres;

public class UpdateGenreModelValidator : AbstractValidator<UpdateGenreModel>
{
    public UpdateGenreModelValidator()
    {
        RuleFor(a => a.Name)
            .NotNull().WithMessage("Name should be not null")
            .NotEmpty().WithMessage("Name should be not empty")
            .MaximumLength(255).WithMessage("Name should be shorter than 255 symbols");
    }
}
