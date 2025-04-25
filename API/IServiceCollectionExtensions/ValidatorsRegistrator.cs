using API.Models.RequestsModels.Developers;
using API.Models.RequestsModels.Genres;
using API.Models.RequestsModels.Localizations;
using API.Models.RequestsModels.Platforms;
using API.ValidationRules.RequestsModels.Developers;
using API.ValidationRules.RequestsModels.Genres;
using API.ValidationRules.RequestsModels.Localizations;
using API.ValidationRules.RequestsModels.Platforms;
using FluentValidation;

namespace API.IServiceCollectionExtensions;

public static class ValidatorsRegistrator
{
    public static IServiceCollection RegisterValidators(this IServiceCollection services)
    {
        services.AddScoped<IValidator<AddDeveloperModel>, AddDeveloperModelValidator>();
        services.AddScoped<IValidator<UpdateDeveloperModel>, UpdateDeveloperModelValidator>();

        services.AddScoped<IValidator<AddLocalizationModel>, AddLocalizationModelValidator>();
        services.AddScoped<IValidator<UpdateLocalizationModel>, UpdateLocalizationModelValidator>();

        services.AddScoped<IValidator<AddGenreModel>, AddGenreModelValidator>();
        services.AddScoped<IValidator<UpdateGenreModel>, UpdateGenreModelValidator>();
        
        services.AddScoped<IValidator<AddPlatformModel>, AddPlatformModelValidator>();
        services.AddScoped<IValidator<UpdatePlatformModel>, UpdatePlatformModelValidator>();

        return services;
    }
}
