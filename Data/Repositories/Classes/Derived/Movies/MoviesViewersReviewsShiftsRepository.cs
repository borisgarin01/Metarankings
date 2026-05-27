using Data.Repositories.Interfaces;
using Domain.Games;
using Domain.Movies;
using Domain.RequestsModels.Games.GamesGamersReviews.Shifts.Backend;

namespace Data.Repositories.Classes.Derived.Movies;

public sealed class MoviesViewersReviewsShiftsRepository : Repository, IRepository<MovieViewerReviewShift, AddMovieViewerReviewShiftModel, UpdateMovieViewerReviewShiftModel>
{
    public MoviesViewersReviewsShiftsRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task<long> AddAsync(AddMovieViewerReviewShiftModel entity)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            long id = await connection.QuerySingleOrDefaultAsync<long>(@"INSERT INTO MoviesViewersReviewsShifts(MovieViewerReviewId, ShifterId, Direction) VALUES(@MovieViewerReviewId, @ShifterId, (@Direction::int)::bit) RETURNING Id;", new
            {
                entity.MovieViewerReviewId,
                entity.ShifterId,
                entity.Direction
            });

            return id;
        }
    }

    public async Task AddRangeAsync(IEnumerable<AddMovieViewerReviewShiftModel> entities)
    {
        foreach (AddMovieViewerReviewShiftModel movieViewerReviewShift in entities)
            await AddAsync(movieViewerReviewShift);
    }

    public async Task<IEnumerable<MovieViewerReviewShift>> GetAllAsync()
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            IEnumerable<MovieViewerReviewShift> movieViewerReviewShift = await connection.QueryAsync<MovieViewerReviewShift>(@"SELECT Id, MovieViewerReviewId, ShifterId, Direction 
FROM MoviesViewersReviewsShifts;");

            return movieViewerReviewShift;
        }
    }

    public async Task<MovieViewerReviewShift> GetAsync(long id)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            MovieViewerReviewShift movieViewerReviewShift = await connection.QuerySingleAsync<MovieViewerReviewShift>(@"SELECT Id, MovieViewerReviewId, ShifterId, Direction 
FROM MoviesViewersReviewsShifts
WHERE Id=@Id;", new { Id = id });

            return movieViewerReviewShift;
        }
    }

    public async Task<IEnumerable<MovieViewerReviewShift>> GetAsync(long offset, long limit)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            IEnumerable<MovieViewerReviewShift> movieViewerReviewShifts = await connection.QueryAsync<MovieViewerReviewShift>(@"SELECT Id, MovieViewerReviewId, ShifterId, Direction 
FROM MoviesViewersReviewsShifts
OFFSET @Offset LIMIT @Limit;", new { Offset = offset, Limit = limit });

            return movieViewerReviewShifts;
        }
    }

    public Task RemoveAsync(long id)
    {
        throw new NotImplementedException();
    }

    public Task RemoveRangeAsync(IEnumerable<long> ids)
    {
        throw new NotImplementedException();
    }

    public Task<MovieViewerReviewShift> UpdateAsync(UpdateMovieViewerReviewShiftModel entity, long id)
    {
        throw new NotImplementedException();
    }
}
