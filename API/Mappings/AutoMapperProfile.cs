using API.Models.RequestsModels.Games;
using API.Models.RequestsModels.Games.Developers;
using API.Models.RequestsModels.Games.Genres;
using API.Models.RequestsModels.Games.Localizations;
using API.Models.RequestsModels.Games.Platforms;
using API.Models.RequestsModels.Games.Publishers;
using API.Models.RequestsModels.Movies.MoviesDirectors;
using Domain.Games;
using Domain.Movies;

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
    }
}
