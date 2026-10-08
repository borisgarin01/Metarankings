using Data.Repositories.Interfaces;
using Domain.Games;
using Domain.RequestsModels.Games.GamesGamersReviews.Shifts.Backend;

namespace Data.Repositories.Classes.Derived.Games;

public sealed class GamesPlayersReviewsShiftsRepository : Repository<GamePlayerReviewShift, AddGamePlayerReviewShiftModel, UpdateGamePlayerReviewShiftModel>
{
    public GamesPlayersReviewsShiftsRepository(string connectionString) : base(connectionString)
    {
    }

    public override async Task<long> AddAsync(AddGamePlayerReviewShiftModel entity, CancellationToken cancellationToken = default)
    {
        using (var connection = CreateConnection())
        {
            long id = await connection.QuerySingleOrDefaultAsync<long>(new CommandDefinition(@"INSERT INTO GamesPlayersReviewsShifts(GamePlayerReviewId, ShifterId, Direction) VALUES(@GamePlayerReviewId, @ShifterId, (@Direction::int)::bit) RETURNING Id;", new
            {
                entity.GamePlayerReviewId,
                entity.ShifterId,
                entity.Direction
            }, cancellationToken: cancellationToken));

            return id;
        }
    }

    public override async Task<IEnumerable<GamePlayerReviewShift>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using (var connection = CreateConnection())
        {
            IEnumerable<GamePlayerReviewShift> gamePlayerReviewShifts = await connection.QueryAsync<GamePlayerReviewShift>(new CommandDefinition(@"SELECT Id, GamePlayerReviewId, ShifterId, Direction 
FROM GamesPlayersReviewsShifts;", cancellationToken: cancellationToken));

            return gamePlayerReviewShifts;
        }
    }

    public override async Task<GamePlayerReviewShift> GetAsync(long id, CancellationToken cancellationToken = default)
    {
        using (var connection = CreateConnection())
        {
            GamePlayerReviewShift gamePlayerReviewShifts = await connection.QuerySingleAsync<GamePlayerReviewShift>(new CommandDefinition(@"SELECT Id, GamePlayerReviewId, ShifterId, Direction 
FROM GamesPlayersReviewsShifts
WHERE Id=@Id;", new { Id = id }, cancellationToken: cancellationToken));

            return gamePlayerReviewShifts;
        }
    }

    public async Task<GamePlayerReviewShift> GetByShifterIdAsync(long shifterId, long gamePlayerReviewId, CancellationToken cancellationToken = default)
    {
        using (var connection = CreateConnection())
        {
            GamePlayerReviewShift gamePlayerReviewShift = await connection.QuerySingleOrDefaultAsync<GamePlayerReviewShift>(new CommandDefinition(@"SELECT Id, GamePlayerReviewId, ShifterId, Direction 
FROM GamesPlayersReviewsShifts
WHERE ShifterId=@ShifterId 
AND GamePlayerReviewId=@GamePlayerReviewId;", new
            {
                ShifterId = shifterId,
                GamePlayerReviewId = gamePlayerReviewId
            }, cancellationToken: cancellationToken));

            return gamePlayerReviewShift;
        }
    }

    public override async Task<IEnumerable<GamePlayerReviewShift>> GetAsync(long offset, long limit, CancellationToken cancellationToken = default)
    {
        using (var connection = CreateConnection())
        {
            IEnumerable<GamePlayerReviewShift> gamePlayerReviewShifts = await connection.QueryAsync<GamePlayerReviewShift>(new CommandDefinition(@"SELECT Id, GamePlayerReviewId, ShifterId, Direction 
FROM GamesPlayersReviewsShifts
OFFSET @Offset LIMIT @Limit;", new { Offset = offset, Limit = limit }, cancellationToken: cancellationToken));

            return gamePlayerReviewShifts;
        }
    }

    public override async Task RemoveAsync(long id, CancellationToken cancellationToken = default)
    {
        using (var connection = CreateConnection())
        {
            await connection.ExecuteAsync(new CommandDefinition("DELETE FROM GamesPlayersReviewsShifts WHERE Id=@Id", new { Id = id }, cancellationToken: cancellationToken));
        }
    }

    public override async Task RemoveRangeAsync(IEnumerable<long> ids, CancellationToken cancellationToken = default)
    {
        using (var connection = CreateConnection())
        {
            await connection.ExecuteAsync(new CommandDefinition("DELETE FROM GamesPlayersReviewsShifts WHERE Id in @Ids", new { Ids = ids }, cancellationToken: cancellationToken));
        }
    }

    public override async Task<GamePlayerReviewShift> UpdateAsync(UpdateGamePlayerReviewShiftModel entity, long id, CancellationToken cancellationToken = default)
    {
        using (var connection = CreateConnection())
        {
            GamePlayerReviewShift gamePlayerReviewShift = await connection.QuerySingleOrDefaultAsync<GamePlayerReviewShift>(new CommandDefinition(@"UPDATE GamesPlayersReviewsShifts 
SET GamePlayerReviewId=@GamePlayerReviewId,
    ShifterId=@ShifterId,
    Direction=(@Direction::int)::bit
    WHERE Id=@Id
    RETURNING Id, GamePlayerReviewId, ShifterId, Direction;", new
            {
                GamePlayerReviewId = entity.GamePlayerReviewId,
                ShifterId = entity.ShifterId,
                Direction = entity.Direction,
                Id = id
            }, cancellationToken: cancellationToken));

            return gamePlayerReviewShift;
        }
    }
}
