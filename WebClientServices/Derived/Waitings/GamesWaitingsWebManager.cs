namespace WebManagers.Derived.Waitings;

public sealed class GamesWaitingsWebManager : WaitingsWebManager
{
    public GamesWaitingsWebManager(IHttpClientFactory httpClientFactory)
        : base(httpClientFactory, "/api/games/GamesWaitings")
    {
    }
}
