using API.Models.RequestsModels.Developers;
using API.Models.RequestsModels.Genres;
using API.Models.RequestsModels.Platforms;
using AutoMapper;
using Domain;

namespace API.Mappings;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<AddDeveloperModel, Developer>();
        CreateMap<UpdateDeveloperModel, Developer>();
        CreateMap<AddGenreModel, Genre>();
        CreateMap<UpdateGenreModel, Genre>();
        CreateMap<AddPlatformModel, Platform>();
        CreateMap<UpdatePlatformModel, Platform>();
    }
}
