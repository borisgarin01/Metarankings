using Domain.RequestsModels.Games.GamesGamersReviews.Shifts.Frontend;
using System.Net.Http.Json;

namespace WebManagers.Derived.Games;

public sealed class GamesPlayersReviewsShiftsWebManager : WebManager
{
    public GamesPlayersReviewsShiftsWebManager(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
    {
    }

    public Task<HttpResponseMessage> AddAsync(AddGamePlayerReviewShiftModel addGamePlayerReviewShiftModel)
    {
        return Client.PostAsJsonAsync("/api/games/GamesGamersReviews/shift", addGamePlayerReviewShiftModel);
    }
}
