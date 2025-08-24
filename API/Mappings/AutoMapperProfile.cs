using Domain.Games;
using Domain.Movies;
using Domain.RequestsModels.Games;
using Domain.RequestsModels.Games.Developers;
using Domain.RequestsModels.Games.GamesGamersReviews;
using Domain.RequestsModels.Games.Genres;
using Domain.RequestsModels.Games.Localizations;
using Domain.RequestsModels.Games.Platforms;
using Domain.RequestsModels.Games.Publishers;
using Domain.RequestsModels.Movies.MoviesDirectors;
using Domain.Reviews;

namespace API.Mappings;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<AddGameModel, Game>();

        CreateMap<AddDeveloperModel, Developer>();
        CreateMap<UpdateDeveloperModel, Developer>();

        CreateMap<AddGenreModel, Genre>();
        CreateMap<UpdateGenreModel, Genre>();

        CreateMap<AddPlatformModel, Platform>();
        CreateMap<UpdatePlatformModel, Platform>();

        CreateMap<AddLocalizationModel, Localization>();
        CreateMap<UpdateLocalizationModel, Localization>();

        CreateMap<AddPublisherModel, Publisher>();
        CreateMap<UpdatePublisherModel, Publisher>();

        CreateMap<AddMovieDirectorModel, MovieDirector>();
        CreateMap<UpdateMovieDirectorModel, MovieDirector>();

        CreateMap<AddGamePlayerReviewModel, GameReview>();
    }
}
