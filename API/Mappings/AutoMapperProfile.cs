using API.Models.RequestsModels.Developers;
using API.Models.RequestsModels.Genres;
using API.Models.RequestsModels.Localizations;
using API.Models.RequestsModels.Platforms;
using API.Models.RequestsModels.Publishers;
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
        
        CreateMap<AddLocalizationModel, Localization>();
        CreateMap<UpdateLocalizationModel, Localization>();
        
        CreateMap<AddPublisherModel, Publisher>();
        CreateMap<UpdatePublisherModel, Publisher>();
    }
}
