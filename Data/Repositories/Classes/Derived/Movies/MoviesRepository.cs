using Dapper;
using Data.Repositories.Interfaces;
using Domain.Movies;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data.Repositories.Classes.Derived.Movies;
public sealed class MoviesRepository : Repository, IRepository<MovieModel>
{
    public MoviesRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task<long> AddAsync(MovieModel entity)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var insertedMovieGenres = new List<MovieGenre>();
            var insertedMovieStudios = new List<MovieStudio>();
            var insertedMovieDirectors = new List<MovieDirector>();

            foreach (var movieGenre in entity.MovieGenres)
            {
                var movieGenreToFind = await connection.QueryFirstOrDefaultAsync<MovieGenre>(@"SELECT Id, Name
FROM MoviesGenres
WHERE Name=@Name;", new { movieGenre.Name });

                if (movieGenreToFind is null)
                {
                    var insertedMovieGenre = await connection.QueryFirstAsync<MovieGenre>(@"INSERT INTO MoviesGenres 
(Name)
output inserted.id, inserted.name
VALUES (@Name);", new { movieGenre.Name });
                    insertedMovieGenres.Add(insertedMovieGenre);
                }
                else
                    insertedMovieGenres.Add(movieGenreToFind);
            }
            foreach (var movieStudio in entity.MoviesStudios)
            {
                var moviesStudioToFind = await connection.QueryFirstOrDefaultAsync<MovieStudio>(@"SELECT Id, Name
FROM MoviesStudios
WHERE Name=@Name;", new { movieStudio.Name });

                if (moviesStudioToFind is null)
                {
                    var insertedMovieStudio = await connection.QueryFirstAsync<MovieStudio>(@"INSERT INTO MoviesStudios 
(Name)
output inserted.id, inserted.name
VALUES (@Name);", new { movieStudio.Name });
                    insertedMovieStudios.Add(insertedMovieStudio);
                }
                else
                {
                    insertedMovieStudios.Add(moviesStudioToFind);
                }
            }

            foreach (var movieDirector in entity.MoviesDirectors)
            {
                var moviesDirectorToFind = await connection.QueryFirstOrDefaultAsync<MovieDirector>(@"SELECT Id, Name
FROM MoviesDirectors
WHERE Name=@Name;", new { movieDirector.Name });

                if (moviesDirectorToFind is null)
                {
                    var insertedMovieDirector = await connection.QueryFirstAsync<MovieDirector>(@"INSERT INTO MoviesDirectors 
(Name)
output inserted.id, inserted.name
VALUES (@Name);", new { movieDirector.Name });
                    insertedMovieDirectors.Add(insertedMovieDirector);
                }
                else
                {
                    insertedMovieDirectors.Add(moviesDirectorToFind);
                }
            }

            var insertedMovie = await connection.QueryFirstAsync<Movie>(@"INSERT INTO Movies 
(Name, OriginalName, ImageSource, PremierDate, Description) 
output inserted.id, inserted.name, inserted.originalname, inserted.imagesource, inserted.premierdate, inserted.description
VALUES
(@Name, @OriginalName, @ImageSource, @PremierDate, @Description);", new
            {
                entity.Name,
                entity.OriginalName,
                entity.ImageSource,
                entity.PremierDate,
                entity.Description
            });

            foreach (var movieGenre in insertedMovieGenres)
            {
                await connection.ExecuteAsync(@"INSERT INTO MoviesMoviesGenres (MovieId, MovieGenreId)
VALUES (@MovieId, @MovieGenreId);",
new { MovieId = insertedMovie.Id, MovieGenreId = movieGenre.Id });
            }

            foreach (var movieStudio in insertedMovieStudios)
            {
                await connection.ExecuteAsync(@"INSERT INTO MoviesMoviesStudios (MovieId, MovieStudioId)
VALUES (@MovieId, @MovieStudioId);",
new { MovieId = insertedMovie.Id, MovieStudioId = movieStudio.Id });
            }

            foreach (var insertedMovieDirector in insertedMovieDirectors)
            {
                await connection.ExecuteAsync(@"INSERT INTO MoviesMoviesDirectors (MovieId, MovieDirectorId)
VALUES (@MovieId, @MovieDirectorId);",
new { MovieId = insertedMovie.Id, MovieDirectorId = insertedMovieDirector.Id });
            }

            return insertedMovie.Id;
        }
    }

    public async Task AddRangeAsync(IEnumerable<MovieModel> entities)
    {
        foreach (var movieModel in entities)
        {
            await AddAsync(movieModel);
        }
    }

    public async Task<IEnumerable<MovieModel>> GetAllAsync()
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var sql = @"SELECT         
m.id, m.name, m.imageSource, m.originalname, m.premierdate, m.description,
ms.id, ms.name,
mg.id, mg.name,
md.id, md.name
    FROM movies m
    LEFT JOIN moviesMoviesGenres mmg ON mmg.movieId = m.Id
    LEFT JOIN moviesGenres mg ON mg.id = mmg.moviegenreid
    LEFT JOIN moviesMoviesStudios mms ON mms.movieId = m.Id
    LEFT JOIN moviesStudios ms ON mms.movieStudioId = ms.id
    LEFT JOIN moviesMoviesDirectors mmd ON mmd.movieId = m.id
    LEFT JOIN moviesDirectors md ON md.id = mmd.movieDirectorId";

            var moviesDictionary = new Dictionary<long, MovieModel>();

            var query = await connection.QueryAsync<MovieModel, MovieGenre, MovieStudio, MovieDirector, MovieModel>(
                sql,
                (movie, movieGenre, movieStudio, movieDirector) =>
                {
                    if (!moviesDictionary.TryGetValue(movie.Id, out var movieEntry))
                    {
                        movieEntry = movie;
                        movieEntry.MovieGenres = new List<MovieGenre>();
                        movieEntry.MoviesStudios = new List<MovieStudio>();
                        movieEntry.MoviesDirectors = new List<MovieDirector>();
                        moviesDictionary.Add(movieEntry.Id, movieEntry);
                    }

                    if (movieGenre is not null && !movieEntry.MovieGenres.Any(d => d.Id == movieGenre.Id))
                        movieEntry.MovieGenres.Add(movieGenre);

                    if (movieStudio is not null && !movieEntry.MoviesStudios.Any(g => g.Id == movieStudio.Id))
                        movieEntry.MoviesStudios.Add(movieStudio);

                    if (movieDirector is not null && !movieEntry.MoviesDirectors.Any(p => p.Id == movieDirector.Id))
                        movieEntry.MoviesDirectors.Add(movieDirector);

                    return movieEntry;
                },
                splitOn: "Id,Id,Id,Id,Id,Id" // The columns where each new entity starts
            );

            var result = moviesDictionary.Values.ToList();

            return result;
        }
    }

    public async Task<MovieModel> GetAsync(long id)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var sql = @"SELECT         
m.id, m.name, m.imageSource, m.originalname, m.premierdate, m.description,
ms.id, ms.name,
mg.id, mg.name,
md.id, md.name
    FROM movies m
    LEFT JOIN moviesMoviesGenres mmg ON mmg.movieId = m.Id
    LEFT JOIN moviesGenres mg ON mg.id = mmg.moviegenreid
    LEFT JOIN moviesMoviesStudios mms ON mms.movieId = m.Id
    LEFT JOIN moviesStudios ms ON mms.movieStudioId = ms.id
    LEFT JOIN moviesMoviesDirectors mmd ON mmd.movieId = m.id
    LEFT JOIN moviesDirectors md ON md.id = mmd.movieDirectorId
WHERE m.id=@id";

            var moviesDictionary = new Dictionary<long, MovieModel>();

            var query = await connection.QueryAsync<MovieModel, MovieGenre, MovieStudio, MovieDirector, MovieModel>(
                sql,
                (movie, movieGenre, movieStudio, movieDirector) =>
                {
                    if (!moviesDictionary.TryGetValue(movie.Id, out var movieEntry))
                    {
                        movieEntry = movie;
                        movieEntry.MovieGenres = new List<MovieGenre>();
                        movieEntry.MoviesStudios = new List<MovieStudio>();
                        movieEntry.MoviesDirectors = new List<MovieDirector>();
                        moviesDictionary.Add(movieEntry.Id, movieEntry);
                    }

                    if (movieGenre is not null && !movieEntry.MovieGenres.Any(d => d.Id == movieGenre.Id))
                        movieEntry.MovieGenres.Add(movieGenre);

                    if (movieStudio is not null && !movieEntry.MoviesStudios.Any(g => g.Id == movieStudio.Id))
                        movieEntry.MoviesStudios.Add(movieStudio);

                    if (movieDirector is not null && !movieEntry.MoviesDirectors.Any(p => p.Id == movieDirector.Id))
                        movieEntry.MoviesDirectors.Add(movieDirector);

                    return movieEntry;
                },
                new { id },
                splitOn: "Id,Id,Id,Id,Id,Id" // The columns where each new entity starts
            );

            var result = moviesDictionary.Values.FirstOrDefault();

            return result;
        }
    }

    public Task<IEnumerable<MovieModel>> GetAsync(long offset, long limit)
    {
        throw new NotImplementedException();
    }

    public Task RemoveAsync(long id)
    {
        throw new NotImplementedException();
    }

    public Task RemoveRangeAsync(IEnumerable<long> ids)
    {
        throw new NotImplementedException();
    }

    public Task<MovieModel> UpdateAsync(MovieModel entity, long id)
    {
        throw new NotImplementedException();
    }
}
