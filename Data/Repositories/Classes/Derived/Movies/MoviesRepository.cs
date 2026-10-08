using Data.Repositories.Interfaces.Derived;
using Domain.Common;
using Domain.Movies;
using Domain.RequestsModels.Movies;
using Domain.RequestsModels.Movies.Movies;
using Domain.Reviews;
using IdentityLibrary.DTOs;

namespace Data.Repositories.Classes.Derived.Movies;

public sealed class MoviesRepository : Repository<Movie, AddMovieModel, UpdateMovieModel>, IMoviesRepository
{
    public MoviesRepository(string connectionString) : base(connectionString)
    {
    }

    public override async Task<long> AddAsync(AddMovieModel entity)
    {
        using NpgsqlConnection connection = CreateConnection();
        await connection.OpenAsync();

        using NpgsqlTransaction transaction = await connection.BeginTransactionAsync();

        long insertedMovieId = await connection.QueryFirstAsync<long>(@"INSERT INTO Movies
(Name, OriginalName, ImageSource, PremierDate, Description, Trailer)
VALUES
(@Name, @OriginalName, @ImageSource, CAST(@PremierDate AS DATE), @Description, @Trailer)
RETURNING Id;", new
        {
            entity.Name,
            entity.OriginalName,
            entity.ImageSource,
            entity.PremierDate,
            entity.Description,
            Trailer = TrailerUrl.ToEmbed(entity.Trailer)
        }, transaction: transaction);

        foreach (long movieGenreId in await GetOrCreateIdsAsync(connection, transaction, "MoviesGenres", entity.MoviesGenresNames))
            await connection.ExecuteAsync("INSERT INTO MoviesMoviesGenres (MovieId, MovieGenreId) VALUES (@MovieId, @MovieGenreId);",
                new { MovieId = insertedMovieId, MovieGenreId = movieGenreId }, transaction: transaction);

        foreach (long movieStudioId in await GetOrCreateIdsAsync(connection, transaction, "MoviesStudios", entity.MoviesStudiosNames))
            await connection.ExecuteAsync("INSERT INTO MoviesMoviesStudios (MovieId, MovieStudioId) VALUES (@MovieId, @MovieStudioId);",
                new { MovieId = insertedMovieId, MovieStudioId = movieStudioId }, transaction: transaction);

        foreach (long movieDirectorId in await GetOrCreateIdsAsync(connection, transaction, "MoviesDirectors", entity.MoviesDirectorsNames))
            await connection.ExecuteAsync("INSERT INTO MoviesMoviesDirectors (MovieId, MovieDirectorId) VALUES (@MovieId, @MovieDirectorId);",
                new { MovieId = insertedMovieId, MovieDirectorId = movieDirectorId }, transaction: transaction);

        if (entity.MoviesCountriesNames is not null)
            await InsertCountriesAsync(connection, transaction, insertedMovieId,
                await GetOrCreateIdsAsync(connection, transaction, "MoviesCountries", entity.MoviesCountriesNames));

        if (entity.MoviesCrew is not null)
            await InsertCrewAsync(connection, transaction, insertedMovieId, entity.MoviesCrew);

        await transaction.CommitAsync();

        return insertedMovieId;
    }

    /// <summary>
    /// Идентификаторы справочника по именам (без повторов, в исходном порядке); отсутствующие записи создаются.
    /// </summary>
    /// <param name="table">Таблица справочника с колонками Id, Name; только константа из кода.</param>
    private static async Task<List<long>> GetOrCreateIdsAsync(NpgsqlConnection connection, NpgsqlTransaction transaction, string table, IEnumerable<string> names)
    {
        List<long> ids = new List<long>();

        foreach (string name in names.Select(n => n.Trim()).Where(n => n.Length > 0).Distinct())
        {
            long? id = await connection.QueryFirstOrDefaultAsync<long?>($"SELECT Id FROM {table} WHERE Name=@Name;",
                new { Name = name }, transaction: transaction);

            id ??= await connection.QueryFirstAsync<long>($"INSERT INTO {table} (Name) VALUES (@Name) RETURNING Id;",
                new { Name = name }, transaction: transaction);

            if (!ids.Contains(id.Value))
                ids.Add(id.Value);
        }

        return ids;
    }

    private static async Task InsertCountriesAsync(NpgsqlConnection connection, NpgsqlTransaction transaction, long movieId, IEnumerable<long> countriesIds)
    {
        foreach (long movieCountryId in countriesIds.Distinct())
            await connection.ExecuteAsync("INSERT INTO MoviesMoviesCountries (MovieId, MovieCountryId) VALUES (@MovieId, @MovieCountryId);",
                new { MovieId = movieId, MovieCountryId = movieCountryId }, transaction: transaction);
    }

    /// <summary>
    /// Связывает фильм с участниками съёмочной группы; персоны ищутся по имени и создаются при отсутствии.
    /// Позиция — порядковый номер участника внутри своей роли.
    /// </summary>
    private static async Task InsertCrewAsync(NpgsqlConnection connection, NpgsqlTransaction transaction, long movieId, IEnumerable<MovieCrewMemberModel> crew)
    {
        foreach (IGrouping<MovieCrewRole, MovieCrewMemberModel> roleGroup in crew.Where(c => Enum.IsDefined(c.Role)).GroupBy(c => c.Role))
        {
            List<long> personsIds = await GetOrCreateIdsAsync(connection, transaction, "MoviesPersons", roleGroup.Select(c => c.Name));

            for (int position = 0; position < personsIds.Count; position++)
                await connection.ExecuteAsync(@"INSERT INTO MoviesMoviesPersons (MovieId, MoviePersonId, Role, Position)
VALUES (@MovieId, @MoviePersonId, @Role, @Position);",
                    new { MovieId = movieId, MoviePersonId = personsIds[position], Role = (short)roleGroup.Key, Position = position }, transaction: transaction);
        }
    }

    /// <summary>
    /// Заполняет страны и съёмочную группу фильма отдельными запросами,
    /// чтобы не умножать строки основного JOIN-а.
    /// </summary>
    private static async Task LoadCountriesAndCrewAsync(NpgsqlConnection connection, Movie movie)
    {
        using SqlMapper.GridReader grid = await connection.QueryMultipleAsync(@"SELECT mc.Id, mc.Name
FROM MoviesMoviesCountries mmc
JOIN MoviesCountries mc ON mc.Id = mmc.MovieCountryId
WHERE mmc.MovieId=@Id
ORDER BY mmc.Id;

SELECT mp.Id, mp.Name, mmp.Role
FROM MoviesMoviesPersons mmp
JOIN MoviesPersons mp ON mp.Id = mmp.MoviePersonId
WHERE mmp.MovieId=@Id
ORDER BY mmp.Role, mmp.Position, mmp.Id;", new { movie.Id });

        movie.MoviesCountries = (await grid.ReadAsync<MovieCountry>()).ToList();
        movie.MoviesCrew = (await grid.ReadAsync<MovieCrewMember>()).ToList();
    }

    public override async Task<IEnumerable<Movie>> GetAllAsync()
    {
        using NpgsqlConnection connection = CreateConnection();
        string sql = @"SELECT         
m.Id, m.Name, m.ImageSource, m.OriginalName, m.PremierDate, m.Description,
COALESCE((SELECT AVG(Score)::float FROM ViewersMoviesReviews WHERE MovieId = m.Id), 0) AS UsersScore,
COALESCE((SELECT COUNT(*) FROM ViewersMoviesReviews WHERE MovieId = m.Id), 0) AS UsersReviewsCount,
COALESCE((SELECT AVG(Score)::float FROM MoviesCriticsReviews WHERE MovieId = m.Id), 0) AS CriticsScore,
COALESCE((SELECT COUNT(*) FROM MoviesCriticsReviews WHERE MovieId = m.Id), 0) AS CriticsReviewsCount,
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

        IEnumerable<Movie> query = await connection.QueryAsync(
            sql,
            (Movie movie, Domain.Movies.Genre movieGenre, MovieStudio movieStudio, MovieDirector movieDirector) =>
            {
                if (!moviesDictionary.TryGetValue(movie.Id, out Movie? movieEntry))
                {
                    movieEntry = movie;
                    movieEntry.MovieGenres = new List<Domain.Movies.Genre>();
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

    public override async Task<Movie> GetAsync(long id)
    {
        using NpgsqlConnection connection = CreateConnection();
        string sql = @"select m.Id, m.Name, m.ImageSource, m.OriginalName, m.PremierDate, m.Description, m.Trailer,
COALESCE((SELECT AVG(Score)::float FROM ViewersMoviesReviews WHERE MovieId = m.Id), 0) AS UsersScore,
COALESCE((SELECT COUNT(*) FROM ViewersMoviesReviews WHERE MovieId = m.Id), 0) AS UsersReviewsCount,
COALESCE((SELECT AVG(Score)::float FROM MoviesCriticsReviews WHERE MovieId = m.Id), 0) AS CriticsScore,
COALESCE((SELECT COUNT(*) FROM MoviesCriticsReviews WHERE MovieId = m.Id), 0) AS CriticsReviewsCount,
mg.Id, mg.Name,
ms.Id, ms.Name,
md.Id, md.Name,
vmr.Id, vmr.MovieId, vmr.ViewerId, vmr.Score, vmr.TextContent, vmr.Date,
vmrs.Id, vmrs.ViewerMovieReviewId AS MovieViewerReviewId, vmrs.ShifterId, vmrs.Direction,
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
    LEFT JOIN viewersMoviesReviewsShifts vmrs on vmrs.ViewerMovieReviewId = vmr.Id
    LEFT JOIN applicationUsers au on au.Id = vmr.ViewerId

WHERE m.id=@id";

        Dictionary<long, Movie> moviesDictionary = new Dictionary<long, Movie>();

        IEnumerable<Movie> query = await connection.QueryAsync(
            sql,
            (Movie movie, Domain.Movies.Genre movieGenre, MovieStudio movieStudio, MovieDirector movieDirector, MovieViewerReview movieReview, MovieViewerReviewShift movieViewerReviewShift, ApplicationUser applicationUser) =>
            {
                if (!moviesDictionary.TryGetValue(movie.Id, out Movie? movieEntry))
                {
                    movieEntry = movie;
                    movieEntry.MovieGenres = new List<Domain.Movies.Genre>();
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

                if (movieReview?.Id > 0 && applicationUser?.Id > 0)
                {
                    MovieViewerReview? existingReview = movieEntry.MovieReviews.FirstOrDefault(mr => mr.Id == movieReview.Id);

                    if (existingReview is null)
                    {
                        existingReview = movieReview with
                        {
                            ApplicationUser = applicationUser,
                            MovieViewerReviewShifts = new List<MovieViewerReviewShift>()
                        };
                        movieEntry.MovieReviews.Add(existingReview);
                    }

                    if (movieViewerReviewShift?.Id > 0
                        && !existingReview.MovieViewerReviewShifts.Any(s => s.Id == movieViewerReviewShift.Id))
                    {
                        existingReview.MovieViewerReviewShifts.Add(movieViewerReviewShift);
                    }
                }

                return movieEntry;
            },
            new { id },
            splitOn: "Id,Id,Id,Id,Id,Id" // Genre, MovieStudio, MovieDirector, MovieViewerReview, MovieViewerReviewShift, ApplicationUser
        );

        Movie? result = moviesDictionary.Values.FirstOrDefault();

        if (result is not null)
            await LoadCountriesAndCrewAsync(connection, result);

        return result;
    }

    public async Task<IEnumerable<Movie>> GetAsync(DateTime dateFrom, DateTime dateTo)
    {
        using NpgsqlConnection connection = CreateConnection();
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

        IEnumerable<Movie> query = await connection.QueryAsync(
            sql,
            (Movie movie, Domain.Movies.Genre movieGenre, MovieStudio movieStudio, MovieDirector movieDirector) =>
            {
                if (!moviesDictionary.TryGetValue(movie.Id, out Movie? movieEntry))
                {
                    movieEntry = movie;
                    movieEntry.MovieGenres = new List<Domain.Movies.Genre>();
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

    public override async Task<IEnumerable<Movie>> GetAsync(long offset, long limit)
    {
        using NpgsqlConnection connection = CreateConnection();
        string sql = @"SELECT         
m.id, m.name, m.imageSource, m.originalname, m.premierdate, m.description,
COALESCE((SELECT AVG(Score)::float FROM ViewersMoviesReviews WHERE MovieId = m.Id), 0) AS UsersScore,
COALESCE((SELECT COUNT(*) FROM ViewersMoviesReviews WHERE MovieId = m.Id), 0) AS UsersReviewsCount,
COALESCE((SELECT AVG(Score)::float FROM MoviesCriticsReviews WHERE MovieId = m.Id), 0) AS CriticsScore,
COALESCE((SELECT COUNT(*) FROM MoviesCriticsReviews WHERE MovieId = m.Id), 0) AS CriticsReviewsCount,
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

        IEnumerable<Movie> query = await connection.QueryAsync(
            sql,
            (Movie movie, Domain.Movies.Genre movieGenre, MovieStudio movieStudio, MovieDirector movieDirector) =>
            {
                if (!moviesDictionary.TryGetValue(movie.Id, out Movie? movieEntry))
                {
                    movieEntry = movie;
                    movieEntry.MovieGenres = new List<Domain.Movies.Genre>();
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
        using NpgsqlConnection connection = CreateConnection();
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

        IEnumerable<Movie> query = await connection.QueryAsync(
            sql,
            (Movie movie, Domain.Movies.Genre movieGenre, MovieStudio movieStudio, MovieDirector movieDirector) =>
            {
                if (!moviesDictionary.TryGetValue(movie.Id, out Movie? movieEntry))
                {
                    movieEntry = movie;
                    movieEntry.MovieGenres = new List<Domain.Movies.Genre>();
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
        using NpgsqlConnection connection = CreateConnection();
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

        IEnumerable<Movie> query = await connection.QueryAsync(
            sql,
            (Movie movie, Genre movieGenre, MovieStudio movieStudio, MovieDirector movieDirector) =>
            {
                if (!moviesDictionary.TryGetValue(movie.Id, out Movie? movieEntry))
                {
                    movieEntry = movie;
                    movieEntry.MovieGenres = new List<Domain.Movies.Genre>();
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

    /// <summary>
    /// Фильтры по режиссёрам, странам и участникам съёмочной группы; подставляются в WHERE подзапроса фильмов.
    /// </summary>
    private const string RelationsFilterSql = @"
       AND (@MoviesDirectorsIds::bigint[] IS NULL OR EXISTS (
            SELECT 1 FROM MoviesMoviesDirectors fmd
            WHERE fmd.MovieId = m.Id AND fmd.MovieDirectorId = ANY(@MoviesDirectorsIds::bigint[])))
       AND (@MoviesCountriesIds::bigint[] IS NULL OR EXISTS (
            SELECT 1 FROM MoviesMoviesCountries fmc
            WHERE fmc.MovieId = m.Id AND fmc.MovieCountryId = ANY(@MoviesCountriesIds::bigint[])))
       AND (@MoviesPersonsIds::bigint[] IS NULL OR EXISTS (
            SELECT 1 FROM MoviesMoviesPersons fmp
            WHERE fmp.MovieId = m.Id AND fmp.MoviePersonId = ANY(@MoviesPersonsIds::bigint[])))";

    public async Task<IEnumerable<Movie>> GetByParametersAsync(MovieFilterRequest filter)
    {
        long[]? genresIds = filter.GenresIds;
        long[]? moviesStudiosIds = filter.MoviesStudiosIds;
        int[]? years = filter.Years;
        int skip = filter.Skip;
        int take = filter.Take;

        using NpgsqlConnection connection = CreateConnection();

        string sql = @"SELECT
m.Id, m.Name, m.ImageSource, m.OriginalName, m.PremierDate, m.Description,
COALESCE((SELECT AVG(Score)::float FROM ViewersMoviesReviews WHERE MovieId = m.Id), 0) AS UsersScore,
COALESCE((SELECT COUNT(*) FROM ViewersMoviesReviews WHERE MovieId = m.Id), 0) AS UsersReviewsCount,
COALESCE((SELECT AVG(Score)::float FROM MoviesCriticsReviews WHERE MovieId = m.Id), 0) AS CriticsScore,
COALESCE((SELECT COUNT(*) FROM MoviesCriticsReviews WHERE MovieId = m.Id), 0) AS CriticsReviewsCount,
mg.Id, mg.Name,
ms.Id, ms.Name,
md.Id, md.Name
FROM (
    SELECT DISTINCT m.Id, m.Name, m.ImageSource, m.OriginalName, m.PremierDate, m.Description
    FROM Movies m
    LEFT JOIN MoviesMoviesGenres mmg ON mmg.MovieId = m.Id
    LEFT JOIN MoviesMoviesStudios mms ON mms.MovieId = m.Id
    WHERE 1=1
       AND (@GenresIds::bigint[] IS NULL OR mmg.MovieGenreId = ANY(@GenresIds::bigint[]))
       AND (@MoviesStudiosIds::bigint[] IS NULL OR mms.MovieStudioId = ANY(@MoviesStudiosIds::bigint[]))
       AND (@Years::int[] IS NULL OR EXTRACT(YEAR FROM m.PremierDate) = ANY(@Years::int[]))" + RelationsFilterSql + @"
    ORDER BY m.Id DESC
    OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY
) AS m
LEFT JOIN MoviesMoviesGenres mmg ON mmg.MovieId = m.Id
LEFT JOIN MoviesGenres mg ON mg.Id = mmg.MovieGenreId
LEFT JOIN MoviesMoviesStudios mms ON mms.MovieId = m.Id
LEFT JOIN MoviesStudios ms ON ms.Id = mms.MovieStudioId
LEFT JOIN MoviesMoviesDirectors mmd ON mmd.MovieId = m.Id
LEFT JOIN MoviesDirectors md ON md.Id = mmd.MovieDirectorId
ORDER BY m.Id DESC;";

        Dictionary<long, Movie> moviesDictionary = new Dictionary<long, Movie>();

        IEnumerable<Movie> query = await connection.QueryAsync(
            sql,
            (Movie movie, Domain.Movies.Genre movieGenre, MovieStudio movieStudio, MovieDirector movieDirector) =>
            {
                if (!moviesDictionary.TryGetValue(movie.Id, out Movie? movieEntry))
                {
                    movieEntry = movie;
                    movieEntry.MovieGenres = new List<Domain.Movies.Genre>();
                    movieEntry.MoviesStudios = new List<MovieStudio>();
                    movieEntry.MoviesDirectors = new List<MovieDirector>();
                    moviesDictionary.Add(movieEntry.Id, movieEntry);
                }

                if (movieGenre is not null && !movieEntry.MovieGenres.Any(g => g.Id == movieGenre.Id))
                    movieEntry.MovieGenres.Add(movieGenre);

                if (movieStudio is not null && !movieEntry.MoviesStudios.Any(s => s.Id == movieStudio.Id))
                    movieEntry.MoviesStudios.Add(movieStudio);

                if (movieDirector is not null && !movieEntry.MoviesDirectors.Any(d => d.Id == movieDirector.Id))
                    movieEntry.MoviesDirectors.Add(movieDirector);

                return movieEntry;
            },
            new
            {
                GenresIds = genresIds is { Length: > 0 } ? genresIds : null,
                MoviesStudiosIds = moviesStudiosIds is { Length: > 0 } ? moviesStudiosIds : null,
                Years = years is { Length: > 0 } ? years : null,
                MoviesDirectorsIds = NullIfEmpty(filter.MoviesDirectorsIds),
                MoviesCountriesIds = NullIfEmpty(filter.MoviesCountriesIds),
                MoviesPersonsIds = NullIfEmpty(filter.MoviesPersonsIds),
                Skip = skip,
                Take = take
            },
            splitOn: "Id,Id,Id,Id"
        );

        return moviesDictionary.Values.ToList();
    }

    private static long[]? NullIfEmpty(long[]? ids) => ids is { Length: > 0 } ? ids : null;

    public async Task<int> GetCountByParametersAsync(MovieFilterRequest filter)
    {
        long[]? genresIds = filter.GenresIds;
        long[]? moviesStudiosIds = filter.MoviesStudiosIds;
        int[]? years = filter.Years;

        using NpgsqlConnection connection = CreateConnection();

        string sql = @"SELECT COUNT(DISTINCT m.Id)
FROM Movies m
LEFT JOIN MoviesMoviesGenres mmg ON mmg.MovieId = m.Id
LEFT JOIN MoviesMoviesStudios mms ON mms.MovieId = m.Id
WHERE 1=1
    AND (@GenresIds::bigint[] IS NULL OR mmg.MovieGenreId = ANY(@GenresIds))
    AND (@MoviesStudiosIds::bigint[] IS NULL OR mms.MovieStudioId = ANY(@MoviesStudiosIds))
    AND (@Years::int[] IS NULL OR EXTRACT(YEAR FROM m.PremierDate) = ANY(@Years))" + RelationsFilterSql + ";";

        int count = await connection.ExecuteScalarAsync<int>(
            sql,
            new
            {
                GenresIds = genresIds is { Length: > 0 } ? genresIds : null,
                MoviesStudiosIds = moviesStudiosIds is { Length: > 0 } ? moviesStudiosIds : null,
                Years = years is { Length: > 0 } ? years : null,
                MoviesDirectorsIds = NullIfEmpty(filter.MoviesDirectorsIds),
                MoviesCountriesIds = NullIfEmpty(filter.MoviesCountriesIds),
                MoviesPersonsIds = NullIfEmpty(filter.MoviesPersonsIds)
            });

        return count;
    }

    public async Task<IEnumerable<Movie>> GetMostWaitingAsync(long[]? genresIds, int skip, int take)
    {
        using NpgsqlConnection connection = CreateConnection();

        string sql = @"SELECT
m.Id, m.Name, m.ImageSource, m.OriginalName, m.PremierDate, m.Description,
COALESCE((SELECT AVG(Score)::float FROM ViewersMoviesReviews WHERE MovieId = m.Id), 0) AS UsersScore,
COALESCE((SELECT COUNT(*) FROM ViewersMoviesReviews WHERE MovieId = m.Id), 0) AS UsersReviewsCount,
COALESCE((SELECT AVG(Score)::float FROM MoviesCriticsReviews WHERE MovieId = m.Id), 0) AS CriticsScore,
COALESCE((SELECT COUNT(*) FROM MoviesCriticsReviews WHERE MovieId = m.Id), 0) AS CriticsReviewsCount,
m.WaitingCount, m.NotWaitingCount,
mg.Id, mg.Name,
ms.Id, ms.Name,
md.Id, md.Name
FROM (
    SELECT m.Id, m.Name, m.ImageSource, m.OriginalName, m.PremierDate, m.Description,
        COALESCE(w.WaitingCount, 0) AS WaitingCount,
        COALESCE(w.NotWaitingCount, 0) AS NotWaitingCount
    FROM Movies m
    LEFT JOIN (
        SELECT MovieId,
            COUNT(*) FILTER (WHERE IsWaiting)::int AS WaitingCount,
            COUNT(*) FILTER (WHERE NOT IsWaiting)::int AS NotWaitingCount
        FROM MoviesWaitings
        GROUP BY MovieId
    ) w ON w.MovieId = m.Id
    WHERE (m.PremierDate IS NULL OR m.PremierDate > CURRENT_DATE)
       AND (@GenresIds::bigint[] IS NULL OR EXISTS (
            SELECT 1 FROM MoviesMoviesGenres mmg
            WHERE mmg.MovieId = m.Id AND mmg.MovieGenreId = ANY(@GenresIds::bigint[])))
    ORDER BY COALESCE(w.WaitingCount, 0) - COALESCE(w.NotWaitingCount, 0) DESC,
        COALESCE(w.WaitingCount, 0) DESC,
        m.PremierDate ASC NULLS LAST, m.Id DESC
    OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY
) AS m
LEFT JOIN MoviesMoviesGenres mmg ON mmg.MovieId = m.Id
LEFT JOIN MoviesGenres mg ON mg.Id = mmg.MovieGenreId
LEFT JOIN MoviesMoviesStudios mms ON mms.MovieId = m.Id
LEFT JOIN MoviesStudios ms ON ms.Id = mms.MovieStudioId
LEFT JOIN MoviesMoviesDirectors mmd ON mmd.MovieId = m.Id
LEFT JOIN MoviesDirectors md ON md.Id = mmd.MovieDirectorId
ORDER BY m.WaitingCount - m.NotWaitingCount DESC, m.WaitingCount DESC, m.PremierDate ASC NULLS LAST, m.Id DESC;";

        Dictionary<long, Movie> moviesDictionary = new Dictionary<long, Movie>();

        IEnumerable<Movie> query = await connection.QueryAsync(
            sql,
            (Movie movie, Domain.Movies.Genre movieGenre, MovieStudio movieStudio, MovieDirector movieDirector) =>
            {
                if (!moviesDictionary.TryGetValue(movie.Id, out Movie? movieEntry))
                {
                    movieEntry = movie;
                    movieEntry.MovieGenres = new List<Domain.Movies.Genre>();
                    movieEntry.MoviesStudios = new List<MovieStudio>();
                    movieEntry.MoviesDirectors = new List<MovieDirector>();
                    moviesDictionary.Add(movieEntry.Id, movieEntry);
                }

                if (movieGenre is not null && !movieEntry.MovieGenres.Any(g => g.Id == movieGenre.Id))
                    movieEntry.MovieGenres.Add(movieGenre);

                if (movieStudio is not null && !movieEntry.MoviesStudios.Any(s => s.Id == movieStudio.Id))
                    movieEntry.MoviesStudios.Add(movieStudio);

                if (movieDirector is not null && !movieEntry.MoviesDirectors.Any(d => d.Id == movieDirector.Id))
                    movieEntry.MoviesDirectors.Add(movieDirector);

                return movieEntry;
            },
            new
            {
                GenresIds = genresIds is { Length: > 0 } ? genresIds : null,
                Skip = skip,
                Take = take
            },
            splitOn: "Id,Id,Id,Id"
        );

        return moviesDictionary.Values.ToList();
    }

    public async Task<int> GetMostWaitingCountAsync(long[]? genresIds)
    {
        using NpgsqlConnection connection = CreateConnection();

        string sql = @"SELECT COUNT(*)
FROM Movies m
WHERE (m.PremierDate IS NULL OR m.PremierDate > CURRENT_DATE)
    AND (@GenresIds::bigint[] IS NULL OR EXISTS (
        SELECT 1 FROM MoviesMoviesGenres mmg
        WHERE mmg.MovieId = m.Id AND mmg.MovieGenreId = ANY(@GenresIds::bigint[])));";

        int count = await connection.ExecuteScalarAsync<int>(
            sql,
            new
            {
                GenresIds = genresIds is { Length: > 0 } ? genresIds : null
            });

        return count;
    }

    public override async Task RemoveAsync(long id)
    {
        using NpgsqlConnection connection = CreateConnection();
        await connection.ExecuteAsync("DELETE FROM Movies WHERE Id=@Id", new { Id = id });
    }

    public override async Task<Movie> UpdateAsync(UpdateMovieModel entity, long id)
    {
        using NpgsqlConnection connection = CreateConnection();
        connection.Open();

        using NpgsqlTransaction transaction = connection.BeginTransaction();

        int affectedRows = await connection.ExecuteAsync(@"UPDATE Movies SET
Name=@Name, OriginalName=@OriginalName, ImageSource=@ImageSource, PremierDate=CAST(@PremierDate AS DATE),
Description=@Description, Trailer=@Trailer
WHERE Id=@Id;", new
        {
            Id = id,
            entity.Name,
            entity.OriginalName,
            entity.ImageSource,
            entity.PremierDate,
            entity.Description,
            Trailer = TrailerUrl.ToEmbed(entity.Trailer)
        }, transaction: transaction);

        if (affectedRows == 0)
            return null;

        await connection.ExecuteAsync(@"DELETE FROM MoviesMoviesGenres WHERE MovieId=@Id;
DELETE FROM MoviesMoviesStudios WHERE MovieId=@Id;
DELETE FROM MoviesMoviesDirectors WHERE MovieId=@Id;", new { Id = id }, transaction: transaction);

        foreach (long movieGenreId in entity.MoviesGenresIds.Distinct())
            await connection.ExecuteAsync("INSERT INTO MoviesMoviesGenres (MovieId, MovieGenreId) VALUES (@MovieId, @MovieGenreId);",
                new { MovieId = id, MovieGenreId = movieGenreId }, transaction: transaction);

        foreach (long movieStudioId in entity.MoviesStudiosIds.Distinct())
            await connection.ExecuteAsync("INSERT INTO MoviesMoviesStudios (MovieId, MovieStudioId) VALUES (@MovieId, @MovieStudioId);",
                new { MovieId = id, MovieStudioId = movieStudioId }, transaction: transaction);

        foreach (long movieDirectorId in entity.MoviesDirectorsIds.Distinct())
            await connection.ExecuteAsync("INSERT INTO MoviesMoviesDirectors (MovieId, MovieDirectorId) VALUES (@MovieId, @MovieDirectorId);",
                new { MovieId = id, MovieDirectorId = movieDirectorId }, transaction: transaction);

        if (entity.MoviesCountriesIds is not null)
        {
            await connection.ExecuteAsync("DELETE FROM MoviesMoviesCountries WHERE MovieId=@Id;", new { Id = id }, transaction: transaction);
            await InsertCountriesAsync(connection, transaction, id, entity.MoviesCountriesIds);
        }

        if (entity.MoviesCrew is not null)
        {
            await connection.ExecuteAsync("DELETE FROM MoviesMoviesPersons WHERE MovieId=@Id;", new { Id = id }, transaction: transaction);
            await InsertCrewAsync(connection, transaction, id, entity.MoviesCrew);
        }

        await transaction.CommitAsync();

        return await GetAsync(id);
    }
}
