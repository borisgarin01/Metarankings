﻿using Domain.Games;
using ExcelProcessors;
using ExcelProcessors.Derived;

namespace API.IServiceCollectionExtensions;

public static class FilesDataReadersRegistrator
{
    public static IServiceCollection RegisterFilesDataReaders(this IServiceCollection services)
    {
        services.AddScoped<IExcelDataReader<Publisher>>(instance => new PublishersDataReader());
        services.AddScoped<IExcelDataReader<Developer>>(instance => new DevelopersDataReader());

        return services;
    }
}
