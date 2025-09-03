using Data.Repositories.Interfaces;
using Domain.Games;
using Domain.RequestsModels.Games.Genres;

namespace Data.Repositories.Classes.Derived.Games;
public sealed class GamesGenresRepository : Repository, IRepository<Genre, AddGenreModel, UpdateGenreModel>
{
    public GamesGenresRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task<long> AddAsync(AddGenreModel genre)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var id = await connection.QueryFirstAsync<long>(@"
INSERT INTO Genres
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

    public async Task AddRangeAsync(IEnumerable<AddGenreModel> genres)
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
            var genres = await connection.QueryAsync<Genre, Game, Genre>(@"
SELECT Genres.Id, Genres.Name, 
	Games.Id, Games.Name, Games.Image, 
	Games.LocalizationId, Games.PublisherId, Games.ReleaseDate, 
	Games.Description, Games.Trailer
FROM 
Genres 
	left join GamesGenres 
		on GamesGenres.GenreId=Genres.Id
	left join Games
		on Games.Id=GamesGenres.GameId", (genre, game) =>
            {
                genre.Games.Add(game);
                return genre;
            });

            var genresResult = genres
                            .GroupBy(d => d.Id)
                            .Select(g =>
                            {
                                Genre groupedGenre = g.First() with
                                {
                                    Games = g.SelectMany(d => d.Games).ToList()
                                };

                                return groupedGenre;
                            });

            return genresResult;
        }
    }

    public async Task<Genre> GetAsync(long id)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var genres = await connection.QueryAsync<Genre, Game, Genre>(@"
SELECT Genres.Id, Genres.Name, 
	Games.Id, Games.Name, Games.Image, 
	Games.LocalizationId, Games.PublisherId, Games.ReleaseDate, 
	Games.Description, Games.Trailer
FROM 
Genres 
	left join GamesGenres 
		on GamesGenres.GenreId=Genres.Id
	left join Games
		on Games.Id=GamesGenres.GameId
WHERE Genres.Id=@id", (genre, game) =>
            {
                genre.Games.Add(game);
                return genre;
            }, new { id });

            var genresResult = genres
                            .GroupBy(d => d.Id)
                            .Select(g =>
                            {
                                Genre groupedGenre = g.First() with
                                {
                                    Games = g.SelectMany(d => d.Games).ToList()
                                };

                                return groupedGenre;
                            });

            return genresResult.FirstOrDefault();
        }
    }

    public async Task<IEnumerable<Genre>> GetAsync(long offset, long limit)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var genres = await connection.QueryAsync<Genre>(@"SELECT Id, Name 
FROM 
Genres 
order by Id asc
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

    public async Task<Genre> UpdateAsync(UpdateGenreModel genre, long id)
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
