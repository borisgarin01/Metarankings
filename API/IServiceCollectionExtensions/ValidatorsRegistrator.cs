using API.Models.RequestsModels.Games.Developers;
using API.Models.RequestsModels.Games.Genres;
using API.Models.RequestsModels.Games.Localizations;
using API.Models.RequestsModels.Games.Platforms;
using API.Models.RequestsModels.Games.Publishers;
using API.ValidationRules.RequestsModels.Developers;
using API.ValidationRules.RequestsModels.Genres;
using API.ValidationRules.RequestsModels.Localizations;
using API.ValidationRules.RequestsModels.Platforms;
using API.ValidationRules.RequestsModels.Publishers;

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

        services.AddScoped<IValidator<AddPublisherModel>, AddPublisherModelValidator>();
        services.AddScoped<IValidator<UpdatePublisherModel>, UpdatePublisherModelValidator>();

        return services;
    }
}
