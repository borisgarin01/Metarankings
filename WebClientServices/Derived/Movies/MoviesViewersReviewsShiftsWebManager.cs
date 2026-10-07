using Domain.RequestsModels.Games.GamesGamersReviews.Shifts.Frontend;
using System.Net.Http.Json;

namespace WebManagers.Derived.Movies;

public sealed class MoviesViewersReviewsShiftsWebManager : WebManager
{
    public MoviesViewersReviewsShiftsWebManager(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
    {
    }

    public Task<HttpResponseMessage> AddAsync(AddMovieViewerReviewShiftModel addMovieViewerReviewShiftModel)
    {
        return Client.PostAsJsonAsync("/api/movies/MoviesViewersReviews/shift", addMovieViewerReviewShiftModel);
    }
}
