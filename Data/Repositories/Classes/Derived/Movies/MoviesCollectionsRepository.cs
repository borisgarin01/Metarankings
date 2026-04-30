using Data.Repositories.Interfaces;
using Domain.Games.Collections;
using Domain.Movies;
using Domain.Movies.MoviesCollections;
using Domain.RequestsModels.Movies.Collections;

namespace Data.Repositories.Classes.Derived.Movies;
public sealed class MoviesCollectionsRepository : Repository, IRepository<MoviesCollection, AddMoviesCollectionModel, UpdateMoviesCollectionModel>
{
    public MoviesCollectionsRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task<long> AddAsync(AddMoviesCollectionModel entity)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var insertedMovieCollectionId = await connection.QuerySingleAsync<long>(@"
INSERT INTO MoviesCollections(Name,Description, ImageSoruce) 
VALUES(@Name, @Description, @ImageSource)
RETURNING Id;", new { entity.Name, entity.Description, entity.ImageSource });

            return insertedMovieCollectionId;
        }
    }

    public async Task AddRangeAsync(IEnumerable<AddMoviesCollectionModel> entities)
    {
        foreach (var entity in entities)
        {
            await AddAsync(entity);
        }
    }

    public async Task<IEnumerable<MoviesCollection>> GetAllAsync()
    {
        using var connection = new NpgsqlConnection(ConnectionString);

        var moviesCollectionsDictionary = new Dictionary<long, MoviesCollection>();

        await connection.QueryAsync<MoviesCollection, Movie, MoviesCollection>(
            @"SELECT mc.Id, mc.Name, mc.ImageSource, mc.Description, mc.ImageSource,
                 m.Id, m.Name, m.OriginalName, m.ImageSource, m.PremierDate, m.Description
          FROM MoviesCollections mc
          LEFT JOIN MoviesCollectionsItems mci ON mc.Id = mci.MovieCollectionId
          LEFT JOIN Movies m ON m.Id = mci.MovieId
          ORDER BY mc.Id",
            (moviesCollection, movie) =>
            {
                if (!moviesCollectionsDictionary.TryGetValue(moviesCollection.Id, out var existingCollection))
                {
                    // Initialize Games list
                    moviesCollection.Movies = new List<Movie>();
                    moviesCollectionsDictionary.Add(moviesCollection.Id, moviesCollection);
                    existingCollection = moviesCollection;
                }

                if (movie is not null)
                {
                    existingCollection.Movies.Add(movie);
                }

                return existingCollection;
            },
            splitOn: "Id");

        return moviesCollectionsDictionary.Values;
    }

    public async Task<MoviesCollection> GetAsync(long id)
    {
        using var connection = new NpgsqlConnection(ConnectionString);

        var moviesCollection = await connection.QueryAsync<MoviesCollection, Movie, MoviesCollection>(
            @"SELECT mc.Id, mc.Name, mc.Description, mc.ImageSource,
                 m.Id, m.Name, m.OriginalName, m.ImageSource, m.PremierDate, m.Description
          FROM MoviesCollections mc
          LEFT JOIN MoviesCollectionsItems mci ON mc.Id = mci.MovieCollectionId
          LEFT JOIN Movies m ON m.Id = mci.MovieId
          WHERE gc.Id = @Id",
            (moviesCollection, movie) =>
            {
                if (!moviesCollection.Movies.Any(m => m.Id == movie.Id))
                    moviesCollection.Movies.Add(movie);
                return moviesCollection;
            },
            new { Id = id },
            splitOn: "Id");

        IEnumerable<MoviesCollection> moviesCollectionGrouped = moviesCollection.GroupBy(b => new { b.Id })
                .Select(g =>
                {
                    MoviesCollection movieCollection = g.First();
                    movieCollection.Movies = g.SelectMany(b => b.Movies).ToList();
                    return movieCollection;
                });

        return moviesCollectionGrouped.SingleOrDefault();
    }

    public async Task<IEnumerable<MoviesCollection>> GetAsync(long offset, long limit)
    {
        using var connection = new NpgsqlConnection(ConnectionString);

        var moviesCollectionsDictionary = new Dictionary<long, MoviesCollection>();

        await connection.QueryAsync<MoviesCollection, Movie, MoviesCollection>(
            @"SELECT mc.Id, mc.Name, mc.Description, mc.ImageSource,
                 m.Id, m.Name, m.OriginalName, m.ImageSource, m.PremierDate, m.Description
          FROM MoviesCollections mc
          LEFT JOIN MoviesCollectionsItems mci ON mc.Id = mci.MovieCollectionId
          LEFT JOIN Movies m ON m.Id = mci.MovieId
          WHERE mc.Id IN (
              SELECT Id 
              FROM MoviesCollections 
              ORDER BY Id ASC 
              OFFSET @offset LIMIT @limit
          )
          ORDER BY mc.Id",
            (moviesCollection, movie) =>
            {
                if (!moviesCollectionsDictionary.TryGetValue(moviesCollection.Id, out var existingCollection))
                {
                    // Initialize Movies list
                    moviesCollection.Movies = new List<Movie>();
                    moviesCollectionsDictionary.Add(moviesCollection.Id, moviesCollection);
                    existingCollection = moviesCollection;
                }

                if (movie is not null)
                {
                    existingCollection.Movies.Add(movie);
                }

                return existingCollection;
            },
            new { offset, limit },
            splitOn: "Id");

        return moviesCollectionsDictionary.Values;
    }

    public async Task RemoveAsync(long id)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            await connection.ExecuteAsync(@"DELETE FROM MoviesCollections WHERE Id=@Id", new { Id = id });
        }
    }

    public async Task RemoveRangeAsync(IEnumerable<long> ids)
    {
        foreach (var id in ids)
        {
            await RemoveAsync(id);
        }
    }

    public async Task<MoviesCollection> UpdateAsync(UpdateMoviesCollectionModel entity, long id)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var updatedGameCollection = await connection.QuerySingleOrDefaultAsync<MoviesCollection>(@"UPDATE MoviesCollections 
SET Name=@Name, Description=@Description, ImageSource=@ImageSource 
WHERE Id=@Id
RETURNING Description, Name, Id;", new
            {
                Name = entity.Name,
                ImageSource = entity.ImageSource,
                Id = id
            });

            return updatedGameCollection;
        }
    }
}
