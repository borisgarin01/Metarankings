using API.Models.RequestsModels;
using AutoMapper;
using Domain;

namespace API.Mappings;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<AddDeveloperModel, Developer>();
        CreateMap<UpdateDeveloperModel, Developer>();
    }
}
