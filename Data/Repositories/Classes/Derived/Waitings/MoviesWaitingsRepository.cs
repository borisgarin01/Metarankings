namespace Data.Repositories.Classes.Derived.Waitings;

public sealed class MoviesWaitingsRepository : WaitingsRepository
{
    public MoviesWaitingsRepository(string connectionString)
        : base(connectionString, "MoviesWaitings", "MovieId", "Movies", "PremierDate")
    {
    }
}
