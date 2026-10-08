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

    public override async Task<long> AddAsync(AddMovieViewerReviewShiftModel entity, CancellationToken cancellationToken = default)
    {
        using (var connection = CreateConnection())
        {
            long id = await connection.QuerySingleOrDefaultAsync<long>(new CommandDefinition(@"INSERT INTO ViewersMoviesReviewsShifts(ViewerMovieReviewId, ShifterId, Direction) VALUES(@MovieViewerReviewId, @ShifterId, (@Direction::int)::bit) RETURNING Id;", new
            {
                entity.MovieViewerReviewId,
                entity.ShifterId,
                entity.Direction
            }, cancellationToken: cancellationToken));

            return id;
        }
    }

    public override async Task<IEnumerable<MovieViewerReviewShift>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using (var connection = CreateConnection())
        {
            IEnumerable<MovieViewerReviewShift> movieViewerReviewShifts = await connection.QueryAsync<MovieViewerReviewShift>(new CommandDefinition($@"SELECT {SelectColumns}
FROM ViewersMoviesReviewsShifts;", cancellationToken: cancellationToken));

            return movieViewerReviewShifts;
        }
    }

    public override async Task<MovieViewerReviewShift> GetAsync(long id, CancellationToken cancellationToken = default)
    {
        using (var connection = CreateConnection())
        {
            MovieViewerReviewShift movieViewerReviewShift = await connection.QuerySingleAsync<MovieViewerReviewShift>(new CommandDefinition($@"SELECT {SelectColumns}
FROM ViewersMoviesReviewsShifts
WHERE Id=@Id;", new { Id = id }, cancellationToken: cancellationToken));

            return movieViewerReviewShift;
        }
    }

    public async Task<MovieViewerReviewShift> GetByShifterIdAsync(long shifterId, long movieViewerReviewId, CancellationToken cancellationToken = default)
    {
        using (var connection = CreateConnection())
        {
            MovieViewerReviewShift movieViewerReviewShift = await connection.QuerySingleOrDefaultAsync<MovieViewerReviewShift>(new CommandDefinition($@"SELECT {SelectColumns}
FROM ViewersMoviesReviewsShifts
WHERE ShifterId=@ShifterId
AND ViewerMovieReviewId=@MovieViewerReviewId;", new
            {
                ShifterId = shifterId,
                MovieViewerReviewId = movieViewerReviewId
            }, cancellationToken: cancellationToken));

            return movieViewerReviewShift;
        }
    }

    public override async Task<IEnumerable<MovieViewerReviewShift>> GetAsync(long offset, long limit, CancellationToken cancellationToken = default)
    {
        using (var connection = CreateConnection())
        {
            IEnumerable<MovieViewerReviewShift> movieViewerReviewShifts = await connection.QueryAsync<MovieViewerReviewShift>(new CommandDefinition($@"SELECT {SelectColumns}
FROM ViewersMoviesReviewsShifts
OFFSET @Offset LIMIT @Limit;", new { Offset = offset, Limit = limit }, cancellationToken: cancellationToken));

            return movieViewerReviewShifts;
        }
    }

    public override async Task RemoveAsync(long id, CancellationToken cancellationToken = default)
    {
        using (var connection = CreateConnection())
        {
            await connection.ExecuteAsync(new CommandDefinition("DELETE FROM ViewersMoviesReviewsShifts WHERE Id=@Id", new { Id = id }, cancellationToken: cancellationToken));
        }
    }

    public override async Task RemoveRangeAsync(IEnumerable<long> ids, CancellationToken cancellationToken = default)
    {
        using (var connection = CreateConnection())
        {
            await connection.ExecuteAsync(new CommandDefinition("DELETE FROM ViewersMoviesReviewsShifts WHERE Id = ANY(@Ids)", new { Ids = ids.ToArray() }, cancellationToken: cancellationToken));
        }
    }

    public override async Task<MovieViewerReviewShift> UpdateAsync(UpdateMovieViewerReviewShiftModel entity, long id, CancellationToken cancellationToken = default)
    {
        using (var connection = CreateConnection())
        {
            MovieViewerReviewShift movieViewerReviewShift = await connection.QuerySingleOrDefaultAsync<MovieViewerReviewShift>(new CommandDefinition($@"UPDATE ViewersMoviesReviewsShifts
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
            }, cancellationToken: cancellationToken));

            return movieViewerReviewShift;
        }
    }
}
