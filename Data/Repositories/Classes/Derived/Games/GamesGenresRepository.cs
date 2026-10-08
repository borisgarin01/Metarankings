using Data.Repositories.Interfaces;
using Domain.Games;
using Domain.RequestsModels.Games.Genres;

namespace Data.Repositories.Classes.Derived.Games;

public sealed class GamesGenresRepository : Repository<Genre, AddGameGenreModel, UpdateGameGenreModel>
{
    public GamesGenresRepository(string connectionString) : base(connectionString)
    {
    }

    public override async Task<long> AddAsync(AddGameGenreModel genre, CancellationToken cancellationToken = default)
    {
        using (var connection = CreateConnection())
        {
            var id = await connection.QueryFirstAsync<long>(new CommandDefinition(@"
INSERT INTO Genres
    (Name)
VALUES (@Name)
RETURNING Id;", new
    {
        genre.Name
    }, cancellationToken: cancellationToken));
            return id;
        }
    }

    public override async Task<IEnumerable<Genre>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using (var connection = CreateConnection())
        {
            var genres = await connection.QueryAsync<Genre, Game, Genre>(new CommandDefinition(@"
SELECT Genres.Id, Genres.Name, 
	Games.Id, Games.Name, Games.Image, 
	Games.LocalizationId, Games.ReleaseDate, 
	Games.Description, Games.Trailer
FROM 
Genres 
	left join GamesGenres 
		on GamesGenres.GenreId=Genres.Id
	left join Games
		on Games.Id=GamesGenres.GameId", cancellationToken: cancellationToken), (genre, game) =>
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

    public override async Task<Genre> GetAsync(long id, CancellationToken cancellationToken = default)
    {
        using (var connection = CreateConnection())
        {
            var genres = await connection.QueryAsync<Genre, Game, Genre>(new CommandDefinition(@"
SELECT Genres.Id, Genres.Name, 
       Games.Id, Games.Name, Games.Image, 
       Games.LocalizationId, Games.ReleaseDate, 
       Games.Description, Games.Trailer,
       COALESCE((SELECT AVG(Score)::float FROM GamesPlayersReviews WHERE GameId = Games.Id), 0) AS UsersScore,
       COALESCE((SELECT COUNT(*) FROM GamesPlayersReviews WHERE GameId = Games.Id), 0) AS UsersReviewsCount,
       COALESCE((SELECT AVG(Score)::float FROM GamesCriticsReviews WHERE GameId = Games.Id), 0) AS CriticsScore,
       COALESCE((SELECT COUNT(*) FROM GamesCriticsReviews WHERE GameId = Games.Id), 0) AS CriticsReviewsCount
FROM Genres 
LEFT JOIN GamesGenres ON GamesGenres.GenreId = Genres.Id
LEFT JOIN Games ON Games.Id = GamesGenres.GameId
WHERE Genres.Id = @Id", new { Id = id }, cancellationToken: cancellationToken), (genre, game) =>
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

            return genresResult.FirstOrDefault();
        }
    }

    public override async Task<IEnumerable<Genre>> GetAsync(long offset, long limit, CancellationToken cancellationToken = default)
    {
        using (var connection = CreateConnection())
        {
            var genres = await connection.QueryAsync<Genre>(new CommandDefinition(@"SELECT Id, Name 
FROM 
Genres 
order by Id asc
OFFSET @offset
LIMIT @limit;", new { offset, limit }, cancellationToken: cancellationToken));

            return genres;
        }
    }

    public override async Task RemoveAsync(long id, CancellationToken cancellationToken = default)
    {
        using (var connection = CreateConnection())
        {
            await connection.ExecuteAsync(new CommandDefinition(@"DELETE FROM 
Genres WHERE Id=@id", new { id }, cancellationToken: cancellationToken));
        }
    }

    public override async Task<Genre> UpdateAsync(UpdateGameGenreModel genre, long id, CancellationToken cancellationToken = default)
    {
        using (var connection = CreateConnection())
        {
            var updatedGenre = await connection.QueryFirstOrDefaultAsync<Genre>(new CommandDefinition(@"UPDATE Genres SET Name=@Name 
where Id=@id
RETURNING Name, Id;", new
            {
                genre.Name,
                id
            }, cancellationToken: cancellationToken));

            return updatedGenre;
        }
    }
}
