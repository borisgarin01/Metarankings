using Domain.Movies;
using Domain.Movies.Collections;
using Domain.RequestsModels.Movies.Collections;
using Domain.RequestsModels.Movies.Movies;
using Domain.RequestsModels.Movies.MoviesDirectors;
using Domain.RequestsModels.Movies.MoviesGenres;
using Domain.RequestsModels.Movies.MoviesStudios;
using WebManagers;
using WebManagers.Derived.CriticsReviews;
using WebManagers.Derived.Movies;
using WebManagers.Derived.Waitings;

namespace BlazorClient.IServiceCollectionsExtensions;

public static class MoviesWebManagersRegistrator
{
    public static IServiceCollection AddMoviesWebManagers(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IWebManager<MovieDirector, AddMovieDirectorModel, UpdateMovieDirectorModel>, MoviesDirectorsWebManager>()
            .AddSingleton<IWebManager<Genre, AddMovieGenreModel, UpdateMovieGenreModel>, MoviesGenresWebManager>()
            .AddSingleton<IWebManager<MovieStudio, AddMovieStudioModel, UpdateMovieStudioModel>, MoviesStudiosWebManager>()
            .AddSingleton<MoviesWebManager>()
            .AddSingleton<IWebManager<Movie, AddMovieModel, UpdateMovieModel>>(serviceProvider => serviceProvider.GetRequiredService<MoviesWebManager>())
            .AddSingleton<IWebManager<MoviesCollection, AddMoviesCollectionModel, UpdateMoviesCollectionModel>, MoviesCollectionsWebManager>()
            .AddSingleton<IWebManager<MoviesCollectionItem, AddMoviesCollectionItemModel, UpdateMoviesCollectionItemModel>, MoviesCollectionsItemsWebManager>()
            .AddSingleton<MoviesViewersReviewsShiftsWebManager>()
            .AddSingleton<MoviesWaitingsWebManager>()
            .AddSingleton<MoviesCriticsReviewsWebManager>();

        return serviceCollection;
    }
}
