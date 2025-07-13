using Data.Repositories.Interfaces;
using Domain.Games;

namespace Data.Repositories.Classes.Derived.Games;
public sealed class GamesGenresRepository : Repository, IRepository<Genre>
{
    public GamesGenresRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task<long> AddAsync(Genre genre)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var id = await connection.QueryFirstAsync<long>(@"INSERT INTO Genres
(Name)
OUTPUT inserted.Id
VALUES (@Name);"
 , new
 {
     genre.Name
 });
            return id;
        }
    }

    public async Task AddRangeAsync(IEnumerable<Genre> genres)
    {
        foreach (var genre in genres)
        {
            await AddAsync(genre);
        }
    }

    public async Task<IEnumerable<Genre>> GetAllAsync()
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var genres = await connection.QueryAsync<Genre>(@"SELECT Id, Name 
FROM 
Genres;");
            return genres;
        }
    }

    public async Task<Genre> GetAsync(long id)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var genre = await connection.QueryFirstOrDefaultAsync<Genre>(@"SELECT Id, Name 
FROM 
Genres
WHERE Id=@id", new { id });

            if (genre is null)
                return null;

            var genreGames = await connection.QueryAsync<GameGenre>(@"SELECT Id, GameId, GenreId
	FROM GamesGenres 
WHERE GenreId=@GenreId;", new { GenreId = id });

            var games = new List<Game>();

            foreach (var genreGame in genreGames)
            {
                var game = await connection.QueryFirstOrDefaultAsync<Game>(@"SELECT Id, Name, Image, LocalizationId, PublisherId, ReleaseDate, Description, Trailer
FROM Games WHERE Id=@GameId", new { genreGame.GameId });

                if (game is not null)
                {
                    games.Add(game);
                }
            }

            genre.Games = games;

            return genre;
        }
    }

    public async Task<IEnumerable<Genre>> GetAsync(long offset, long limit)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var genres = await connection.QueryAsync<Genre>(@"SELECT Id, Name 
FROM 
Genres 
OFFSET @offset
LIMIT @limit;", new { offset, limit });

            return genres;
        }
    }

    public async Task RemoveAsync(long id)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            await connection.ExecuteAsync(@"DELETE FROM 
Genres WHERE Id=@id", new { id });
        }
    }

    public async Task RemoveRangeAsync(IEnumerable<long> ids)
    {
        foreach (var id in ids)
        {
            await RemoveAsync(id);
        }
    }

    public async Task<Genre> UpdateAsync(Genre genre, long id)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var updatedGenre = await connection.QueryFirstOrDefaultAsync<Genre>(@"UPDATE Genres set Name=@Name 
output inserted.name, inserted.id
where Id=@id", new
            {
                genre.Name,
                id
            });

            return updatedGenre;
        }
    }
}
