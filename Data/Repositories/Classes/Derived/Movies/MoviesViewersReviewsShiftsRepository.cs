using Data.Repositories.Interfaces;
using Domain.Movies;
using Domain.RequestsModels.Games.GamesGamersReviews.Shifts.Backend;

namespace Data.Repositories.Classes.Derived.Movies;

public sealed class MoviesViewersReviewsShiftsRepository : Repository<MovieViewerReviewShift, AddMovieViewerReviewShiftModel, UpdateMovieViewerReviewShiftModel>
{
    private const string SelectColumns = "Id, ViewerMovieReviewId AS MovieViewerReviewId, ShifterId, Direction";

    public MoviesViewersReviewsShiftsRepository(string connectionString) : base(connectionString)
    {
    }

    public override async Task<long> AddAsync(AddMovieViewerReviewShiftModel entity)
    {
        using (var connection = CreateConnection())
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

    public override async Task<IEnumerable<MovieViewerReviewShift>> GetAllAsync()
    {
        using (var connection = CreateConnection())
        {
            IEnumerable<MovieViewerReviewShift> movieViewerReviewShifts = await connection.QueryAsync<MovieViewerReviewShift>($@"SELECT {SelectColumns}
FROM ViewersMoviesReviewsShifts;");

            return movieViewerReviewShifts;
        }
    }

    public override async Task<MovieViewerReviewShift> GetAsync(long id)
    {
        using (var connection = CreateConnection())
        {
            MovieViewerReviewShift movieViewerReviewShift = await connection.QuerySingleAsync<MovieViewerReviewShift>($@"SELECT {SelectColumns}
FROM ViewersMoviesReviewsShifts
WHERE Id=@Id;", new { Id = id });

            return movieViewerReviewShift;
        }
    }

    public async Task<MovieViewerReviewShift> GetByShifterIdAsync(long shifterId, long movieViewerReviewId)
    {
        using (var connection = CreateConnection())
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

    public override async Task<IEnumerable<MovieViewerReviewShift>> GetAsync(long offset, long limit)
    {
        using (var connection = CreateConnection())
        {
            IEnumerable<MovieViewerReviewShift> movieViewerReviewShifts = await connection.QueryAsync<MovieViewerReviewShift>($@"SELECT {SelectColumns}
FROM ViewersMoviesReviewsShifts
OFFSET @Offset LIMIT @Limit;", new { Offset = offset, Limit = limit });

            return movieViewerReviewShifts;
        }
    }

    public override async Task RemoveAsync(long id)
    {
        using (var connection = CreateConnection())
        {
            await connection.ExecuteAsync("DELETE FROM ViewersMoviesReviewsShifts WHERE Id=@Id", new { Id = id });
        }
    }

    public override async Task RemoveRangeAsync(IEnumerable<long> ids)
    {
        using (var connection = CreateConnection())
        {
            await connection.ExecuteAsync("DELETE FROM ViewersMoviesReviewsShifts WHERE Id = ANY(@Ids)", new { Ids = ids.ToArray() });
        }
    }

    public override async Task<MovieViewerReviewShift> UpdateAsync(UpdateMovieViewerReviewShiftModel entity, long id)
    {
        using (var connection = CreateConnection())
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
