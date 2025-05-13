using Dapper;
using Data.Repositories.Interfaces;
using Domain;
using Npgsql;

namespace Data.Repositories.Classes.Derived;
public sealed class GenresRepository : Repository, IRepository<Genre>
{
    public GenresRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task<long> AddAsync(Genre genre)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var id = await connection.QueryFirstAsync<long>(@"INSERT INTO Genres
(Name, Url)
VALUES (@Name, @Url)
RETURNING Id;"
 , new
 {
     genre.Name,
     genre.Url
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
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var genres = await connection.QueryAsync<Genre>(@"SELECT Id, Name, Url 
FROM 
Genres;");
            return genres;
        }
    }

    public async Task<Genre> GetAsync(long id)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var genre = await connection.QueryFirstOrDefaultAsync<Genre>(@"SELECT Id, Name, Url 
FROM 
Genres
WHERE Id=@id", new { id });

            if (genre is null)
                return null;

            var genreGames = await connection.QueryAsync<GameGenre>(@"SELECT Id, GameId, GenreId
	FROM GamesGenres WHERE GenreId=@GenreId;", new { GenreId = id });

            var games = new List<Game>();

            foreach (var genreGame in genreGames)
            {
                var game = await connection.QueryFirstOrDefaultAsync<Game>(@"SELECT Id, Href, Name, Image, LocalizationId, PublisherId, ReleaseDate, Description, Trailer
FROM Games WHERE Id=@GameId", new { GameId = genreGame.GameId });

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
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var genres = await connection.QueryAsync<Genre>(@"SELECT Id, Name, Url 
FROM 
Genres 
OFFSET @offset
LIMIT @limit;", new { offset, limit });

            return genres;
        }
    }

    public async Task RemoveAsync(long id)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
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
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var updatedGenre = await connection.QueryFirstOrDefaultAsync<Genre>(@"UPDATE Genres set Name=@Name, Url=@Url 
where Id=@id
returning Name, Url, Id", new
            {
                genre.Name,
                genre.Url,
                id
            });

            return updatedGenre;
        }
    }
}
