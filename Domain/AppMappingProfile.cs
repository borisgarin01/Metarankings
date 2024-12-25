using AutoMapper;
using Domain.ViewModels;

namespace Domain;

public class AppMappingProfile : Profile
{
    public AppMappingProfile()
    {
        CreateMap<AddGameViewModel, Game>();
        CreateMap<UpdateGameViewModel, Game>();
        CreateMap<GameLocalizationViewModel, GameLocalization>();
        CreateMap<AddGameDeveloperViewModel, GameDeveloper>();
    }
}