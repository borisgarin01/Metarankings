using Data.Repositories.Interfaces;
using Domain.Games;
using Domain.RequestsModels.Games.GamesGamersReviews.Shifts.Backend;

namespace Data.Repositories.Classes.Derived.Games;

public sealed class GamesPlayersReviewsShiftsRepository : Repository, IRepository<GamePlayerReviewShift, AddGamePlayerReviewShiftModel, UpdateGamePlayerReviewShiftModel>
{
    public GamesPlayersReviewsShiftsRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task<long> AddAsync(AddGamePlayerReviewShiftModel entity)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            long id = await connection.QuerySingleOrDefaultAsync<long>(@"INSERT INTO GamesPlayersReviewsShifts(GamePlayerReviewId, ShifterId, Direction) VALUES(@GamePlayerReviewId, @ShifterId, (@Direction::int)::bit) RETURNING Id;", new
            {
                entity.GamePlayerReviewId,
                entity.ShifterId,
                entity.Direction
            });

            return id;
        }
    }

    public async Task AddRangeAsync(IEnumerable<AddGamePlayerReviewShiftModel> entities)
    {
        foreach (AddGamePlayerReviewShiftModel gamePlayerReviewShift in entities)
            await AddAsync(gamePlayerReviewShift);
    }

    public async Task<IEnumerable<GamePlayerReviewShift>> GetAllAsync()
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            IEnumerable<GamePlayerReviewShift> gamePlayerReviewShifts = await connection.QueryAsync<GamePlayerReviewShift>(@"SELECT Id, GamePlayerReviewId, ShifterId, Direction 
FROM GamesPlayersReviewsShifts;");

            return gamePlayerReviewShifts;
        }
    }

    public async Task<GamePlayerReviewShift> GetAsync(long id)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            GamePlayerReviewShift gamePlayerReviewShifts = await connection.QuerySingleAsync<GamePlayerReviewShift>(@"SELECT Id, GamePlayerReviewId, ShifterId, Direction 
FROM GamesPlayersReviewsShifts
WHERE Id=@Id;", new { Id = id });

            return gamePlayerReviewShifts;
        }
    }

    public async Task<GamePlayerReviewShift> GetByShifterIdAsync(long shifterId, long gamePlayerReviewId)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            GamePlayerReviewShift gamePlayerReviewShift = await connection.QuerySingleOrDefaultAsync<GamePlayerReviewShift>(@"SELECT Id, GamePlayerReviewId, ShifterId, Direction 
FROM GamesPlayersReviewsShifts
WHERE ShifterId=@ShifterId 
AND GamePlayerReviewId=@GamePlayerReviewId;", new
            {
                ShifterId = shifterId,
                GamePlayerReviewId = gamePlayerReviewId
            });

            return gamePlayerReviewShift;
        }
    }

    public async Task<IEnumerable<GamePlayerReviewShift>> GetAsync(long offset, long limit)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            IEnumerable<GamePlayerReviewShift> gamePlayerReviewShifts = await connection.QueryAsync<GamePlayerReviewShift>(@"SELECT Id, GamePlayerReviewId, ShifterId, Direction 
FROM GamesPlayersReviewsShifts
OFFSET @Offset LIMIT @Limit;", new { Offset = offset, Limit = limit });

            return gamePlayerReviewShifts;
        }
    }

    public async Task RemoveAsync(long id)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            await connection.ExecuteAsync("DELETE FROM GamesPlayersReviewsShifts WHERE Id=@Id", new { Id = id });
        }
    }

    public async Task RemoveRangeAsync(IEnumerable<long> ids)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            await connection.ExecuteAsync("DELETE FROM GamesPlayersReviewsShifts WHERE Id in @Ids", new { Ids = ids });
        }
    }

    public async Task<GamePlayerReviewShift> UpdateAsync(UpdateGamePlayerReviewShiftModel entity, long id)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            GamePlayerReviewShift gamePlayerReviewShift = await connection.QuerySingleOrDefaultAsync<GamePlayerReviewShift>(@"UPDATE GamesPlayersReviewsShifts 
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
            });

            return gamePlayerReviewShift;
        }
    }
}
