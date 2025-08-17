using Domain.Games;
using Domain.RequestsModels.Games.Developers;
using Domain.RequestsModels.Games.Publishers;
using ExcelProcessors;
using ExcelProcessors.Derived;

namespace API.IServiceCollectionExtensions;

public static class FilesDataReadersRegistrator
{
    public static IServiceCollection RegisterFilesDataReaders(this IServiceCollection services)
    {
        services.AddScoped<IExcelDataReader<AddPublisherModel>>(instance => new PublishersDataReader());
        services.AddScoped<IExcelDataReader<AddDeveloperModel>>(instance => new DevelopersDataReader());

        return services;
    }
}
