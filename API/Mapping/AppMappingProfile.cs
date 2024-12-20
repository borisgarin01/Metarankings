using AutoMapper;
using Domain;
using Domain.ViewModels;

namespace API.Mapping;

public class AppMappingProfile : Profile
{
    public AppMappingProfile()
    {
        CreateMap<AddGameViewModel, Game>();
        CreateMap<UpdateGameViewModel, Game>();
    }
}