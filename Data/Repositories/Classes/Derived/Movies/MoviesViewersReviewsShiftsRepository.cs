using Data.Repositories.Interfaces;
using Domain.Movies;
using Domain.RequestsModels.Games.GamesGamersReviews.Shifts.Backend;

namespace Data.Repositories.Classes.Derived.Movies;

public sealed class MoviesViewersReviewsShiftsRepository : Repository, IRepository<MovieViewerReviewShift, AddMovieViewerReviewShiftModel, UpdateMovieViewerReviewShiftModel>
{
    private const string SelectColumns = "Id, ViewerMovieReviewId AS MovieViewerReviewId, ShifterId, Direction";

    public MoviesViewersReviewsShiftsRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task<long> AddAsync(AddMovieViewerReviewShiftModel entity)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            long id = await connection.QuerySingleOrDefaultAsync<long>(@"INSERT INTO ViewersMoviesReviewsShifts(ViewerMovieReviewId, ShifterId, Direction) VALUES(@MovieViewerReviewId, @ShifterId, (@Direction::int)::bit) RETURNING Id;", new
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
            IEnumerable<MovieViewerReviewShift> movieViewerReviewShifts = await connection.QueryAsync<MovieViewerReviewShift>($@"SELECT {SelectColumns}
FROM ViewersMoviesReviewsShifts;");

            return movieViewerReviewShifts;
        }
    }

    public async Task<MovieViewerReviewShift> GetAsync(long id)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            MovieViewerReviewShift movieViewerReviewShift = await connection.QuerySingleAsync<MovieViewerReviewShift>($@"SELECT {SelectColumns}
FROM ViewersMoviesReviewsShifts
WHERE Id=@Id;", new { Id = id });

            return movieViewerReviewShift;
        }
    }

    public async Task<MovieViewerReviewShift> GetByShifterIdAsync(long shifterId, long movieViewerReviewId)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            MovieViewerReviewShift movieViewerReviewShift = await connection.QuerySingleOrDefaultAsync<MovieViewerReviewShift>($@"SELECT {SelectColumns}
FROM ViewersMoviesReviewsShifts
WHERE ShifterId=@ShifterId
AND ViewerMovieReviewId=@MovieViewerReviewId;", new
            {
                ShifterId = shifterId,
                MovieViewerReviewId = movieViewerReviewId
            });

            return movieViewerReviewShift;
        }
    }

    public async Task<IEnumerable<MovieViewerReviewShift>> GetAsync(long offset, long limit)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            IEnumerable<MovieViewerReviewShift> movieViewerReviewShifts = await connection.QueryAsync<MovieViewerReviewShift>($@"SELECT {SelectColumns}
FROM ViewersMoviesReviewsShifts
OFFSET @Offset LIMIT @Limit;", new { Offset = offset, Limit = limit });

            return movieViewerReviewShifts;
        }
    }

    public async Task RemoveAsync(long id)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            await connection.ExecuteAsync("DELETE FROM ViewersMoviesReviewsShifts WHERE Id=@Id", new { Id = id });
        }
    }

    public async Task RemoveRangeAsync(IEnumerable<long> ids)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            await connection.ExecuteAsync("DELETE FROM ViewersMoviesReviewsShifts WHERE Id = ANY(@Ids)", new { Ids = ids.ToArray() });
        }
    }

    public async Task<MovieViewerReviewShift> UpdateAsync(UpdateMovieViewerReviewShiftModel entity, long id)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            MovieViewerReviewShift movieViewerReviewShift = await connection.QuerySingleOrDefaultAsync<MovieViewerReviewShift>($@"UPDATE ViewersMoviesReviewsShifts
SET ViewerMovieReviewId=@MovieViewerReviewId,
    ShifterId=@ShifterId,
    Direction=(@Direction::int)::bit
WHERE Id=@Id
RETURNING {SelectColumns};", new
            {
                entity.MovieViewerReviewId,
                entity.ShifterId,
                entity.Direction,
                Id = id
            });

            return movieViewerReviewShift;
        }
    }
}
