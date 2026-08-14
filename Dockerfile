FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source

COPY *.slnx .

COPY API/*.csproj ./API/
COPY BlazorClient/*.csproj ./BlazorClient/
COPY Data/*.csproj ./Data/
COPY Domain/*.csproj ./Domain/
COPY ExcelProcessors/*.csproj ./ExcelProcessors/
COPY IdentityLibrary/*.csproj ./IdentityLibrary/
COPY Shared/*.csproj ./Shared/
COPY WebClientServices/*.csproj ./WebClientServices/
COPY Settings/*.csproj ./Settings/
COPY ViewModels/*.csproj ./ViewModels/

RUN dotnet restore

COPY API/. ./API/
COPY BlazorClient/. ./BlazorClient/
COPY Data/. ./Data/
COPY Domain/. ./Domain/
COPY ExcelProcessors/. ./ExcelProcessors/
COPY IdentityLibrary/. ./IdentityLibrary/
COPY Shared/. ./Shared/
COPY WebClientServices/. ./WebClientServices/
COPY Settings/. ./Settings/
COPY ViewModels/. ./ViewModels/

WORKDIR /source/API
RUN dotnet publish -c release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "API.dll"]
