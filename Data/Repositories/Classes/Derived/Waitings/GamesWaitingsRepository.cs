namespace Data.Repositories.Classes.Derived.Waitings;

public sealed class GamesWaitingsRepository : WaitingsRepository
{
    public GamesWaitingsRepository(string connectionString)
        : base(connectionString, "GamesWaitings", "GameId", "Games", "ReleaseDate")
    {
    }
}
