using Data.Repositories.Interfaces;
using Domain.Movies;
using Domain.RequestsModels.Movies.MoviesGenres;

namespace Data.Repositories.Classes.Derived.Movies;

public sealed class MoviesGenresRepository : Repository<Genre, AddMovieGenreModel, UpdateMovieGenreModel>
{
    public MoviesGenresRepository(string connectionString) : base(connectionString)
    {
    }

    public override async Task<long> AddAsync(AddMovieGenreModel entity, CancellationToken cancellationToken = default)
    {
        using (var connection = CreateConnection())
        {
            var insertedId = await connection.QueryFirstOrDefaultAsync<long>(new CommandDefinition(@"INSERT INTO MoviesGenres(Name) 
VALUES (@Name)
RETURNING Id;", new { entity.Name }, cancellationToken: cancellationToken));

            return insertedId;
        }
    }

    public override async Task<IEnumerable<Genre>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using (var connection = CreateConnection())
        {
            var moviesGenres = await connection.QueryAsync<Genre>(new CommandDefinition(@"SELECT Id, Name 
FROM MoviesGenres;", cancellationToken: cancellationToken));

            return moviesGenres;
        }
    }

    public override async Task<Genre> GetAsync(long id, CancellationToken cancellationToken = default)
    {
        using (var connection = CreateConnection())
        {
            var moviesGenres = await connection.QueryAsync<Genre, Movie,Genre>(new CommandDefinition(@"
SELECT MoviesGenres.Id, MoviesGenres.Name, 
       Movies.Id, Movies.Name, Movies.OriginalName, Movies.ImageSource,
       Movies.PremierDate, Movies.Description,
       COALESCE((SELECT AVG(Score)::float FROM ViewersMoviesReviews WHERE MovieId = Movies.Id), 0) AS UsersScore,
       COALESCE((SELECT COUNT(*) FROM ViewersMoviesReviews WHERE MovieId = Movies.Id), 0) AS UsersReviewsCount,
       COALESCE((SELECT AVG(Score)::float FROM MoviesCriticsReviews WHERE MovieId = Movies.Id), 0) AS CriticsScore,
       COALESCE((SELECT COUNT(*) FROM MoviesCriticsReviews WHERE MovieId = Movies.Id), 0) AS CriticsReviewsCount
FROM MoviesGenres 
LEFT JOIN MoviesMoviesGenres ON MoviesMoviesGenres.MovieGenreId = MoviesGenres.Id
LEFT JOIN Movies ON Movies.Id = MoviesMoviesGenres.MovieId
WHERE MoviesGenres.Id = @Id", new { Id = id }, cancellationToken: cancellationToken), (genre, movie) =>
            {
                genre.Movies.Add(movie);
                return genre;
            });

            var genresResult = moviesGenres
                            .GroupBy(d => d.Id)
                            .Select(g =>
                            {
                                Genre groupedGenre = g.First() with
                                {
                                    Movies = g.SelectMany(d => d.Movies).ToList()
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
            var moviesGenres = await connection.QueryAsync<Genre>(new CommandDefinition(@"SELECT Id, Name 
FROM MoviesGenres
OFFSET @offset
LIMIT @limit;", new { offset, limit }, cancellationToken: cancellationToken));

            return moviesGenres;
        }
    }
    public override async Task RemoveAsync(long id, CancellationToken cancellationToken = default)
    {
        using (var connection = CreateConnection())
            await connection.ExecuteAsync(new CommandDefinition(@"DELETE FROM MoviesGenres WHERE Id=@id", new { id }, cancellationToken: cancellationToken));
    }

    public override async Task<Genre> UpdateAsync(UpdateMovieGenreModel movieGenre, long id, CancellationToken cancellationToken = default)
    {
        using (var connection = CreateConnection())
        {
            var updatedMovieGenre = await connection.QueryFirstOrDefaultAsync<Genre>(new CommandDefinition(@"UPDATE MoviesGenres 
SET Name=@Name
WHERE Id=@Id
RETURNING Name, Id;", new
            {
                movieGenre.Name,
                id
            }, cancellationToken: cancellationToken));

            return updatedMovieGenre;
        }
    }
}
