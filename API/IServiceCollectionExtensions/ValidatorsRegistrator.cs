using API.Models.RequestsModels.Developers;
using API.ValidationRules.RequestsModels.Developers;
using FluentValidation;

namespace API.IServiceCollectionExtensions;

public static class ValidatorsRegistrator
{
    public static IServiceCollection RegisterValidators(this IServiceCollection services)
    {
        services.AddScoped<IValidator<AddDeveloperModel>, AddDeveloperModelValidator>();
        services.AddScoped<IValidator<UpdateDeveloperModel>, UpdateDeveloperModelValidator>();
        return services;
    }
}
