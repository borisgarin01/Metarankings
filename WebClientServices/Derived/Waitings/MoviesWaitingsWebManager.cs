namespace WebManagers.Derived.Waitings;

public sealed class MoviesWaitingsWebManager : WaitingsWebManager
{
    public MoviesWaitingsWebManager(IHttpClientFactory httpClientFactory)
        : base(httpClientFactory, "/api/movies/MoviesWaitings")
    {
    }
}
