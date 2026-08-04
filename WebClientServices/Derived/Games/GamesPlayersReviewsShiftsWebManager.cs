using Domain.Games;
using Domain.RequestsModels.Games.GamesGamersReviews.Shifts.Frontend;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;

namespace WebManagers.Derived.Games;

public sealed class GamesPlayersReviewsShiftsWebManager : WebManager, IWebManager<GamePlayerReviewShift, AddGamePlayerReviewShiftModel, UpdateGamePlayerReviewShiftModel>
{
    public GamesPlayersReviewsShiftsWebManager(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
    {
    }

    public async Task<HttpResponseMessage> AddAsync(AddGamePlayerReviewShiftModel addGamePlayerReviewShiftModel)
    {
        HttpResponseMessage response = await HttpClientFactory.CreateClient("AuthorizedClient").PostAsJsonAsync("/api/games/GamesGamersReviews/shift", addGamePlayerReviewShiftModel);
        return response;
    }

    public Task<HttpResponseMessage> AddFromExcelAsync(IFormFile formFile)
    {
        throw new NotImplementedException();
    }

    public Task<HttpResponseMessage> AddFromJsonAsync(IEnumerable<AddGamePlayerReviewShiftModel> addGamePlayerReviewShiftModels)
    {
        throw new NotImplementedException();
    }

    public Task<HttpResponseMessage> DeleteAsync(long id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<GamePlayerReviewShift>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<GamePlayerReviewShift> GetAsync(long id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<GamePlayerReviewShift>> GetFirstAsync(long offset, long limit)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<GamePlayerReviewShift>> GetLastAsync(long offset, long limit)
    {
        throw new NotImplementedException();
    }

    public Task<GamePlayerReviewShift> UpdateAsync(long id, UpdateGamePlayerReviewShiftModel updateGamePlayerReviewShiftModel)
    {
        throw new NotImplementedException();
    }
}
