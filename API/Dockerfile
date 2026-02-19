# См. статью по ссылке https://aka.ms/customizecontainer, чтобы узнать как настроить контейнер отладки и как Visual Studio использует этот Dockerfile для создания образов для ускорения отладки.

# Этот этап используется при запуске из VS в быстром режиме (по умолчанию для конфигурации отладки)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8081
EXPOSE 8080


# Этот этап используется для сборки проекта службы
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["API/API.csproj", "API/"]
COPY ["BlazorClient/BlazorClient.csproj", "BlazorClient/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["IdentityLibrary/IdentityLibrary.csproj", "IdentityLibrary/"]
COPY ["Shared/Shared.csproj", "Shared/"]
COPY ["WebClientServices/WebManagers.csproj", "WebClientServices/"]
COPY ["Data/Data.csproj", "Data/"]
COPY ["ExcelProcessors/ExcelProcessors.csproj", "ExcelProcessors/"]
COPY ["Settings/Settings.csproj", "Settings/"]
RUN dotnet restore "./API/API.csproj"
COPY . .
WORKDIR "/src/API"
RUN dotnet build "./API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Этот этап используется для публикации проекта службы, который будет скопирован на последний этап
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Этот этап используется в рабочей среде или при запуске из VS в обычном режиме (по умолчанию, когда конфигурация отладки не используется)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS="https://+:8081;http://+:8080"
ENTRYPOINT ["dotnet", "API.dll"]