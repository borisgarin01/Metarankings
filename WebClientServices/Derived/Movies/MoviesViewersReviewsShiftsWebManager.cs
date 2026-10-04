using Domain.Movies;
using Domain.RequestsModels.Games.GamesGamersReviews.Shifts.Frontend;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;

namespace WebManagers.Derived.Movies;

public sealed class MoviesViewersReviewsShiftsWebManager : WebManager, IWebManager<MovieViewerReviewShift, AddMovieViewerReviewShiftModel, UpdateMovieViewerReviewShiftModel>
{
    public MoviesViewersReviewsShiftsWebManager(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
    {
    }

    public async Task<HttpResponseMessage> AddAsync(AddMovieViewerReviewShiftModel addMovieViewerReviewShiftModel)
    {
        HttpResponseMessage response = await HttpClientFactory.CreateClient("AuthorizedClient").PostAsJsonAsync("/api/movies/MoviesViewersReviews/shift", addMovieViewerReviewShiftModel);
        return response;
    }

    public Task<HttpResponseMessage> AddFromExcelAsync(IFormFile formFile)
    {
        throw new NotImplementedException();
    }

    public Task<HttpResponseMessage> AddFromJsonAsync(IEnumerable<AddMovieViewerReviewShiftModel> addMovieViewerReviewShiftModels)
    {
        throw new NotImplementedException();
    }

    public Task<HttpResponseMessage> DeleteAsync(long id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<MovieViewerReviewShift>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<MovieViewerReviewShift> GetAsync(long id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<MovieViewerReviewShift>> GetFirstAsync(long offset, long limit)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<MovieViewerReviewShift>> GetLastAsync(long offset, long limit)
    {
        throw new NotImplementedException();
    }

    public Task<MovieViewerReviewShift> UpdateAsync(long id, UpdateMovieViewerReviewShiftModel updateMovieViewerReviewShiftModel)
    {
        throw new NotImplementedException();
    }
}
