using Data.Repositories.Interfaces;
using Domain.Movies;
using Domain.RequestsModels.Movies.MoviesGenres;

namespace Data.Repositories.Classes.Derived.Movies;

public sealed class MoviesGenresRepository : Repository, IRepository<Genre, AddMovieGenreModel, UpdateMovieGenreModel>
{
    public MoviesGenresRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task<long> AddAsync(AddMovieGenreModel entity)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var insertedId = await connection.QueryFirstOrDefaultAsync<long>(@"INSERT INTO MoviesGenres(Name) 
VALUES (@Name)
RETURNING Id;", new { entity.Name });

            return insertedId;
        }
    }

    public async Task AddRangeAsync(IEnumerable<AddMovieGenreModel> entities)
    {
        foreach (var movieGenre in entities)
        {
            await AddAsync(movieGenre);
        }
    }

    public async Task<IEnumerable<Genre>> GetAllAsync()
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var moviesGenres = await connection.QueryAsync<Genre>(@"SELECT Id, Name 
FROM MoviesGenres;");

            return moviesGenres;
        }
    }

    public async Task<Genre> GetAsync(long id)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var moviesGenres = await connection.QueryAsync<Genre, Movie,Genre>(@"
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
WHERE MoviesGenres.Id = @Id", (genre, movie) =>
            {
                genre.Movies.Add(movie);
                return genre;
            }, new { Id = id });

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

    public async Task<IEnumerable<Genre>> GetAsync(long offset, long limit)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var moviesGenres = await connection.QueryAsync<Genre>(@"SELECT Id, Name 
FROM MoviesGenres
OFFSET @offset
LIMIT @limit;", new { offset, limit });

            return moviesGenres;
        }
    }
    public async Task RemoveAsync(long id)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
            await connection.ExecuteAsync(@"DELETE FROM MoviesGenres WHERE Id=@id", new { id });
    }

    public async Task RemoveRangeAsync(IEnumerable<long> ids)
    {
        foreach (var id in ids)
        {
            await RemoveAsync(id);
        }
    }

    public async Task<Genre> UpdateAsync(UpdateMovieGenreModel movieGenre, long id)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var updatedMovieGenre = await connection.QueryFirstOrDefaultAsync<Genre>(@"UPDATE MoviesGenres 
SET Name=@Name
WHERE Id=@Id
RETURNING Name, Id;", new
            {
                movieGenre.Name,
                id
            });

            return updatedMovieGenre;
        }
    }
}
