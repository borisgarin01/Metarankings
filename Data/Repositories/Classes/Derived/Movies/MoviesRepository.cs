using Data.Repositories.Interfaces.Derived;
using Domain.Games;
using Domain.Movies;
using Domain.RequestsModels.Movies.Movies;
using Domain.Reviews;
using IdentityLibrary.DTOs;

namespace Data.Repositories.Classes.Derived.Movies;

public sealed class MoviesRepository : Repository, IMoviesRepository
{
    public MoviesRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task<long> AddAsync(AddMovieModel entity)
    {
        using NpgsqlConnection connection = new NpgsqlConnection(ConnectionString);
        List<MovieGenre> insertedMovieGenres = new List<MovieGenre>();
        List<MovieStudio> insertedMovieStudios = new List<MovieStudio>();
        List<MovieDirector> insertedMovieDirectors = new List<MovieDirector>();

        foreach (string movieGenreName in entity.MoviesGenresNames)
        {
            MovieGenre? movieGenreToFind = await connection.QueryFirstOrDefaultAsync<MovieGenre>(@"SELECT Id, Name
FROM MoviesGenres
WHERE Name=@Name;", new { Name = movieGenreName });

            if (movieGenreToFind is null)
            {
                MovieGenre insertedMovieGenre = await connection.QueryFirstAsync<MovieGenre>(@"INSERT INTO MoviesGenres 
(Name)
VALUES (@Name)
RETURNING Id, Name;", new { Name = movieGenreName });
                insertedMovieGenres.Add(insertedMovieGenre);
            }
            else
                insertedMovieGenres.Add(movieGenreToFind);
        }
        foreach (string movieStudioName in entity.MoviesStudiosNames)
        {
            MovieStudio? moviesStudioToFind = await connection.QueryFirstOrDefaultAsync<MovieStudio>(@"SELECT Id, Name
FROM MoviesStudios
WHERE Name=@Name;", new { Name = movieStudioName });

            if (moviesStudioToFind is null)
            {
                MovieStudio insertedMovieStudio = await connection.QueryFirstAsync<MovieStudio>(@"INSERT INTO MoviesStudios 
(Name)
VALUES (@Name)
RETURNING Id, Name;", new { Name = movieStudioName });
                insertedMovieStudios.Add(insertedMovieStudio);
            }
            else
            {
                insertedMovieStudios.Add(moviesStudioToFind);
            }
        }

        foreach (string movieDirectorName in entity.MoviesDirectorsNames)
        {
            MovieDirector? moviesDirectorToFind = await connection.QueryFirstOrDefaultAsync<MovieDirector>(@"SELECT Id, Name
FROM MoviesDirectors
WHERE Name=@Name;", new { Name = movieDirectorName });

            if (moviesDirectorToFind is null)
            {
                MovieDirector insertedMovieDirector = await connection.QueryFirstAsync<MovieDirector>(@"INSERT INTO MoviesDirectors 
(Name)
VALUES (@Name)
RETURNING Id, Name;", new { Name = movieDirectorName });
                insertedMovieDirectors.Add(insertedMovieDirector);
            }
            else
            {
                insertedMovieDirectors.Add(moviesDirectorToFind);
            }
        }

        Movie insertedMovie = await connection.QueryFirstAsync<Movie>(@"INSERT INTO Movies 
(Name, OriginalName, ImageSource, PremierDate, Description) 
VALUES
(@Name, @OriginalName, @ImageSource, CAST(@PremierDate AS DATE), @Description)
RETURNING Id, Name, OriginalName, ImageSource, PremierDate, Description;", new
        {
            entity.Name,
            entity.OriginalName,
            entity.ImageSource,
            entity.PremierDate,
            entity.Description
        });

        foreach (MovieGenre movieGenre in insertedMovieGenres)
        {
            await connection.ExecuteAsync(@"INSERT INTO MoviesMoviesGenres (MovieId, MovieGenreId)
VALUES (@MovieId, @MovieGenreId);",
new { MovieId = insertedMovie.Id, MovieGenreId = movieGenre.Id });
        }

        foreach (MovieStudio movieStudio in insertedMovieStudios)
        {
            await connection.ExecuteAsync(@"INSERT INTO MoviesMoviesStudios (MovieId, MovieStudioId)
VALUES (@MovieId, @MovieStudioId);",
new { MovieId = insertedMovie.Id, MovieStudioId = movieStudio.Id });
        }

        foreach (MovieDirector insertedMovieDirector in insertedMovieDirectors)
        {
            await connection.ExecuteAsync(@"INSERT INTO MoviesMoviesDirectors (MovieId, MovieDirectorId)
VALUES (@MovieId, @MovieDirectorId);",
new { MovieId = insertedMovie.Id, MovieDirectorId = insertedMovieDirector.Id });
        }

        return insertedMovie.Id;
    }

    public async Task AddRangeAsync(IEnumerable<AddMovieModel> entities)
    {
        foreach (AddMovieModel movieModel in entities)
        {
            await AddAsync(movieModel);
        }
    }

    public async Task<IEnumerable<Movie>> GetAllAsync()
    {
        using NpgsqlConnection connection = new NpgsqlConnection(ConnectionString);
        string sql = @"SELECT         
m.Id, m.Name, m.ImageSource, m.OriginalName, m.PremierDate, m.Description,
mg.Id, mg.Name,
ms.Id, ms.Name,
md.Id, md.Name
    FROM movies m
    LEFT JOIN moviesMoviesGenres mmg ON mmg.movieId = m.Id
    LEFT JOIN moviesGenres mg ON mg.id = mmg.moviegenreid
    LEFT JOIN moviesMoviesStudios mms ON mms.movieId = m.Id
    LEFT JOIN moviesStudios ms ON mms.movieStudioId = ms.id
    LEFT JOIN moviesMoviesDirectors mmd ON mmd.movieId = m.id
    LEFT JOIN moviesDirectors md ON md.id = mmd.movieDirectorId";

        Dictionary<long, Movie> moviesDictionary = new Dictionary<long, Movie>();

        IEnumerable<Movie> query = await connection.QueryAsync<Movie, MovieGenre, MovieStudio, MovieDirector, Movie>(
            sql,
            (movie, movieGenre, movieStudio, movieDirector) =>
            {
                if (!moviesDictionary.TryGetValue(movie.Id, out Movie? movieEntry))
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

        List<Movie> result = moviesDictionary.Values.ToList();

        return result;
    }

    public async Task<Movie> GetAsync(long id)
    {
        using NpgsqlConnection connection = new NpgsqlConnection(ConnectionString);
        string sql = @"select m.Id, m.Name, m.ImageSource, m.OriginalName, m.PremierDate, m.Description,
mg.Id, mg.Name,
ms.Id, ms.Name,
md.Id, md.Name,
vmr.MovieId, vmr.ViewerId, vmr.Score, vmr.TextContent, vmr.Date,
au.Id, au.UserName, au.NormalizedUserName, au.Email, au.NormalizedEmail, 
au.EmailConfirmed, au.PasswordHash, au.PhoneNumber, au.PhoneNumberConfirmed, au.TwoFactorEnabled
    FROM movies m
    LEFT JOIN moviesMoviesGenres mmg ON mmg.movieId = m.Id
    LEFT JOIN moviesGenres mg ON mg.id = mmg.moviegenreid
    LEFT JOIN moviesMoviesStudios mms ON mms.movieId = m.Id
    LEFT JOIN moviesStudios ms ON mms.movieStudioId = ms.id
    LEFT JOIN moviesMoviesDirectors mmd ON mmd.movieId = m.id
    LEFT JOIN moviesDirectors md ON md.id = mmd.movieDirectorId
    LEFT JOIN viewersMoviesReviews vmr on vmr.movieId = m.Id
    LEFT JOIN applicationUsers au on au.Id = vmr.ViewerId

WHERE m.id=@id";

        Dictionary<long, Movie> moviesDictionary = new Dictionary<long, Movie>();

        IEnumerable<Movie> query = await connection.QueryAsync<Movie, MovieGenre, MovieStudio, MovieDirector, MovieReview, ApplicationUser, Movie>(
            sql,
            (movie, movieGenre, movieStudio, movieDirector, movieReview, applicationUser) =>
            {
                if (!moviesDictionary.TryGetValue(movie.Id, out Movie? movieEntry))
                {
                    movieEntry = movie;
                    movieEntry.MovieGenres = new List<MovieGenre>();
                    movieEntry.MoviesStudios = new List<MovieStudio>();
                    movieEntry.MoviesDirectors = new List<MovieDirector>();
                    moviesDictionary.Add(movieEntry.Id, movieEntry);
                }

                if (movieGenre is not null && !movieEntry.MovieGenres.Any(mg => mg.Id == movieGenre.Id))
                    movieEntry.MovieGenres.Add(movieGenre);

                if (movieStudio is not null && !movieEntry.MoviesStudios.Any(ms => ms.Id == movieStudio.Id))
                    movieEntry.MoviesStudios.Add(movieStudio);

                if (movieDirector is not null && !movieEntry.MoviesDirectors.Any(md => md.Id == movieDirector.Id))
                    movieEntry.MoviesDirectors.Add(movieDirector);

                if (movieReview is not null && !movieEntry.MovieReviews.Any(mr => mr.Id == movieReview.Id) && applicationUser is not null)
                {
                    movieReview = movieReview with { ApplicationUser = applicationUser };
                    movieEntry.MovieReviews.Add(movieReview);
                }

                return movieEntry;
            },
            new { id },
            splitOn: "Id,Id,Id,Id,MovieId,Id" // The columns where each new entity starts
        );

        Movie? result = moviesDictionary.Values.FirstOrDefault();

        return result;
    }

    public async Task<IEnumerable<Movie>> GetAsync(DateTime dateFrom, DateTime dateTo)
    {
        using NpgsqlConnection connection = new NpgsqlConnection(ConnectionString);
        string sql = @"SELECT         
m.id, m.name, m.imageSource, m.originalname, m.premierdate, m.description,
mg.id, mg.name,
ms.id, ms.name,
md.id, md.name
    FROM movies m
    LEFT JOIN moviesMoviesGenres mmg ON mmg.movieId = m.Id
    LEFT JOIN moviesGenres mg ON mg.id = mmg.moviegenreid
    LEFT JOIN moviesMoviesStudios mms ON mms.movieId = m.Id
    LEFT JOIN moviesStudios ms ON mms.movieStudioId = ms.id
    LEFT JOIN moviesMoviesDirectors mmd ON mmd.movieId = m.id
    LEFT JOIN moviesDirectors md ON md.id = mmd.movieDirectorId
WHERE m.premierDate between @dateFrom and @dateTo
ORDER BY m.id DESC;";

        Dictionary<long, Movie> moviesDictionary = new Dictionary<long, Movie>();

        IEnumerable<Movie> query = await connection.QueryAsync<Movie, MovieGenre, MovieStudio, MovieDirector, Movie>(
            sql,
            (movie, movieGenre, movieStudio, movieDirector) =>
            {
                if (!moviesDictionary.TryGetValue(movie.Id, out Movie? movieEntry))
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
            }, new { dateFrom, dateTo },
            splitOn: "Id,Id,Id,Id,Id,Id" // The columns where each new entity starts
        );

        List<Movie> result = moviesDictionary.Values.ToList();

        return result;
    }

    public async Task<IEnumerable<Movie>> GetAsync(long offset, long limit)
    {
        using NpgsqlConnection connection = new NpgsqlConnection(ConnectionString);
        string sql = @"SELECT         
m.id, m.name, m.imageSource, m.originalname, m.premierdate, m.description,
mg.id, mg.name,
ms.id, ms.name,
md.id, md.name
     FROM (
                select Id, Name, ImageSource, OriginalName, PremierDate, Description from Movies
                ORDER BY Id DESC
                OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY
            ) AS m
    LEFT JOIN moviesMoviesGenres mmg ON mmg.movieId = m.Id
    LEFT JOIN moviesGenres mg ON mg.id = mmg.moviegenreid
    LEFT JOIN moviesMoviesStudios mms ON mms.movieId = m.Id
    LEFT JOIN moviesStudios ms ON mms.movieStudioId = ms.id
    LEFT JOIN moviesMoviesDirectors mmd ON mmd.movieId = m.id
    LEFT JOIN moviesDirectors md ON md.id = mmd.movieDirectorId;";

        Dictionary<long, Movie> moviesDictionary = new Dictionary<long, Movie>();

        IEnumerable<Movie> query = await connection.QueryAsync<Movie, MovieGenre, MovieStudio, MovieDirector, Movie>(
            sql,
            (movie, movieGenre, movieStudio, movieDirector) =>
            {
                if (!moviesDictionary.TryGetValue(movie.Id, out Movie? movieEntry))
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
            }, new { Offset = offset, Limit = limit },
            splitOn: "Id,Id,Id,Id,Id,Id" // The columns where each new entity starts
        );

        List<Movie> result = moviesDictionary.Values.ToList();

        return result;
    }

    public async Task<IEnumerable<Movie>> GetByGenreAsync(long genreId)
    {
        using NpgsqlConnection connection = new NpgsqlConnection(ConnectionString);
        string sql = @"SELECT         
            m.id, m.name, m.imageSource, m.originalname, m.premierdate, m.description,
            mg.id, mg.name,
            ms.id, ms.name,
            md.id, md.name
        FROM movies m
        LEFT JOIN moviesMoviesGenres mmg ON mmg.movieId = m.Id
        LEFT JOIN moviesGenres mg ON mg.id = mmg.moviegenreid
        LEFT JOIN moviesMoviesStudios mms ON mms.movieId = m.Id
        LEFT JOIN moviesStudios ms ON mms.movieStudioId = ms.id
        LEFT JOIN moviesMoviesDirectors mmd ON mmd.movieId = m.id
        LEFT JOIN moviesDirectors md ON md.id = mmd.movieDirectorId
        WHERE mg.Id=@GenreId";

        Dictionary<long, Movie> moviesDictionary = new Dictionary<long, Movie>();

        IEnumerable<Movie> query = await connection.QueryAsync<Movie, MovieGenre, MovieStudio, MovieDirector, Movie>(
            sql,
            (movie, movieGenre, movieStudio, movieDirector) =>
            {
                if (!moviesDictionary.TryGetValue(movie.Id, out Movie? movieEntry))
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
            new { GenreId = genreId }, // Pass parameter as anonymous object
            splitOn: "Id,Id,Id,Id" // You had one too many "Id"
        );

        return moviesDictionary.Values;
    }

    public async Task<IEnumerable<Movie>> GetByNameAsync(string name)
    {
        using NpgsqlConnection connection = new NpgsqlConnection(ConnectionString);
        string sql = @"SELECT         
            m.id, m.name, m.imageSource, m.originalname, m.premierdate, m.description,
            mg.id, mg.name,
            ms.id, ms.name,
            md.id, md.name
        FROM movies m
        LEFT JOIN moviesMoviesGenres mmg ON mmg.movieId = m.Id
        LEFT JOIN moviesGenres mg ON mg.id = mmg.moviegenreid
        LEFT JOIN moviesMoviesStudios mms ON mms.movieId = m.Id
        LEFT JOIN moviesStudios ms ON mms.movieStudioId = ms.id
        LEFT JOIN moviesMoviesDirectors mmd ON mmd.movieId = m.id
        LEFT JOIN moviesDirectors md ON md.id = mmd.movieDirectorId
        WHERE m.name ILIKE '%' || @name || '%'
        ORDER BY m.Id DESC;";

        Dictionary<long, Movie> moviesDictionary = new Dictionary<long, Movie>();

        IEnumerable<Movie> query = await connection.QueryAsync<Movie, MovieGenre, MovieStudio, MovieDirector, Movie>(
            sql,
            (movie, movieGenre, movieStudio, movieDirector) =>
            {
                if (!moviesDictionary.TryGetValue(movie.Id, out Movie? movieEntry))
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
            new { name }, // Pass parameter as anonymous object
            splitOn: "Id,Id,Id,Id" // You had one too many "Id"
        );

        return moviesDictionary.Values;
    }

    public async Task RemoveAsync(long id)
    {
        using NpgsqlConnection connection = new NpgsqlConnection(ConnectionString);
        await connection.ExecuteAsync("DELETE FROM Movies WHERE Id=@Id", new { Id = id });
    }

    public Task RemoveRangeAsync(IEnumerable<long> ids)
    {
        throw new NotImplementedException();
    }

    public Task<Movie> UpdateAsync(UpdateMovieModel entity, long id)
    {
        throw new NotImplementedException();
    }
}
