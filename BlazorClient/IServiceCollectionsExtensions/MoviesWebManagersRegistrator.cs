using Domain.Movies;
using Domain.Movies.Collections;
using Domain.RequestsModels.Movies.Collections;
using Domain.RequestsModels.Movies.MoviesDirectors;
using Domain.RequestsModels.Movies.MoviesGenres;
using Domain.RequestsModels.Movies.MoviesStudios;
using WebManagers;
using WebManagers.Derived.Movies;

namespace BlazorClient.IServiceCollectionsExtensions;

public static class MoviesWebManagersRegistrator
{
    public static IServiceCollection AddMoviesWebManagers(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IWebManager<MovieDirector, AddMovieDirectorModel, UpdateMovieDirectorModel>, MoviesDirectorsWebManager>()
            .AddSingleton<IWebManager<MovieGenre, AddMovieGenreModel, UpdateMovieGenreModel>, MoviesGenresWebManager>()
            .AddSingleton<IWebManager<MovieStudio, AddMovieStudioModel, UpdateMovieStudioModel>, MoviesStudiosWebManager>()
            .AddSingleton<MoviesWebManager>()
            .AddSingleton<IWebManager<MoviesCollection, AddMoviesCollectionModel, UpdateMoviesCollectionModel>, MoviesCollectionsWebManager>()
            .AddSingleton<IWebManager<MoviesCollectionItem, AddMoviesCollectionItemModel, UpdateMoviesCollectionItemModel>, MoviesCollectionsItemsWebManager>();

        return serviceCollection;
    }
}
