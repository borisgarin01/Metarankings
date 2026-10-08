using Data.Repositories.Interfaces;
using Domain.Movies;
using Domain.Movies.Collections;
using Domain.RequestsModels.Movies.Collections;

namespace Data.Repositories.Classes.Derived.Movies;

public sealed class MoviesCollectionsRepository : Repository<MoviesCollection, AddMoviesCollectionModel, UpdateMoviesCollectionModel>
{
    public MoviesCollectionsRepository(string connectionString) : base(connectionString)
    {
    }

    public override async Task<long> AddAsync(AddMoviesCollectionModel entity, CancellationToken cancellationToken = default)
    {
        using (var connection = CreateConnection())
        {
            var insertedMovieCollectionId = await connection.QuerySingleAsync<long>(new CommandDefinition(@"
INSERT INTO MoviesCollections(Name,Description, ImageSource) 
VALUES(@Name, @Description, @ImageSource)
RETURNING Id;", new { entity.Name, entity.Description, entity.ImageSource }, cancellationToken: cancellationToken));

            return insertedMovieCollectionId;
        }
    }

    public override async Task<IEnumerable<MoviesCollection>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();

        var moviesCollectionsDictionary = new Dictionary<long, MoviesCollection>();

        await connection.QueryAsync<MoviesCollection, MoviesCollectionItem, Movie, MoviesCollection>(
            new CommandDefinition(@"SELECT mc.Id, mc.Name, mc.ImageSource, mc.Description, mc.ImageSource,
                 mci.Id, mci.MovieCollectionId, mci.MovieId,
                 m.Id, m.Name, m.OriginalName, m.ImageSource, m.PremierDate, m.Description
          FROM MoviesCollections mc
          LEFT JOIN MoviesCollectionsItems mci ON mc.Id = mci.MovieCollectionId
          LEFT JOIN Movies m ON m.Id = mci.MovieId
          ORDER BY mc.Id", cancellationToken: cancellationToken),
            (moviesCollection, movieCollectionItem, movie) =>
            {
                if (!moviesCollectionsDictionary.TryGetValue(moviesCollection.Id, out var existingCollection))
                {
                    // Initialize Games list
                    moviesCollectionsDictionary.Add(moviesCollection.Id, moviesCollection);
                    existingCollection = moviesCollection;
                }

                if (movie is not null && movieCollectionItem is not null && !moviesCollection.MoviesCollectionItems.Any(mci => mci.Id == movieCollectionItem.Id))
                {
                    movieCollectionItem.Movie = movie;
                    movieCollectionItem.MovieId = movie.Id;
                    movieCollectionItem.MoviesCollection = moviesCollection;
                    movieCollectionItem.MovieCollectionId = moviesCollection.Id;
                    existingCollection.MoviesCollectionItems.Add(movieCollectionItem);
                }

                return existingCollection;
            },
            splitOn: "Id");

        return moviesCollectionsDictionary.Values;
    }

    public override async Task<MoviesCollection> GetAsync(long id, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();

        var moviesCollection = await connection.QueryAsync<MoviesCollection, MoviesCollectionItem, Movie, MoviesCollection>(
            new CommandDefinition(@"SELECT mc.Id, mc.Name, mc.Description, mc.ImageSource,
                 mci.Id, mci.MovieCollectionId, mci.MovieId,
                 m.Id, m.Name, m.OriginalName, m.ImageSource, m.PremierDate, m.Description
          FROM MoviesCollections mc
          LEFT JOIN MoviesCollectionsItems mci ON mc.Id = mci.MovieCollectionId
          LEFT JOIN Movies m ON m.Id = mci.MovieId
          WHERE mc.Id = @Id", new { Id = id }, cancellationToken: cancellationToken),
            (moviesCollection, movieCollectionItem, movie) =>
            {
                if (movie is not null && movieCollectionItem is not null && !moviesCollection.MoviesCollectionItems.Any(m => m.MovieId == movie.Id))
                {
                    movieCollectionItem.Movie = movie;
                    movieCollectionItem.MovieId = movie.Id;
                    movieCollectionItem.MoviesCollection = moviesCollection;
                    movieCollectionItem.MovieCollectionId = moviesCollection.Id;
                    moviesCollection.MoviesCollectionItems.Add(movieCollectionItem);
                }

                return moviesCollection;
            },
            splitOn: "Id");

        IEnumerable<MoviesCollection> moviesCollectionGrouped = moviesCollection.GroupBy(b => new { b.Id })
                .Select(g =>
                {
                    MoviesCollection movieCollection = g.First();
                    movieCollection.MoviesCollectionItems = g.SelectMany(b => b.MoviesCollectionItems).ToList();
                    return movieCollection;
                });

        return moviesCollectionGrouped.SingleOrDefault();
    }

    public override async Task<IEnumerable<MoviesCollection>> GetAsync(long offset, long limit, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();

        var moviesCollectionsDictionary = new Dictionary<long, MoviesCollection>();

        await connection.QueryAsync<MoviesCollection, MoviesCollectionItem, Movie, MoviesCollection>(
            new CommandDefinition(@"SELECT mc.Id, mc.Name, mc.Description, mc.ImageSource,
                 mci.Id, mci.MovieCollectionId, mci.MovieId,
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
          ORDER BY mc.Id", new { offset, limit }, cancellationToken: cancellationToken),
            (moviesCollection, movieCollectionItem, movie) =>
            {
                if (!moviesCollectionsDictionary.TryGetValue(moviesCollection.Id, out var existingCollection))
                {
                    // Initialize Movies list
                    moviesCollectionsDictionary.Add(moviesCollection.Id, moviesCollection);
                    existingCollection = moviesCollection;
                }

                if (movieCollectionItem is not null && movie is not null && !moviesCollection.MoviesCollectionItems.Any(m => m.MovieId == movie.Id))
                {
                    movieCollectionItem.Movie = movie;
                    movieCollectionItem.MovieId = movie.Id;
                    movieCollectionItem.MoviesCollection = moviesCollection;
                    movieCollectionItem.MovieCollectionId = moviesCollection.Id;
                    existingCollection.MoviesCollectionItems.Add(movieCollectionItem);
                }

                return existingCollection;
            },
            splitOn: "Id");

        return moviesCollectionsDictionary.Values;
    }

    public override async Task RemoveAsync(long id, CancellationToken cancellationToken = default)
    {
        using (var connection = CreateConnection())
        {
            await connection.ExecuteAsync(new CommandDefinition(@"DELETE FROM MoviesCollections WHERE Id=@Id", new { Id = id }, cancellationToken: cancellationToken));
        }
    }

    public override async Task<MoviesCollection> UpdateAsync(UpdateMoviesCollectionModel entity, long id, CancellationToken cancellationToken = default)
    {
        using (var connection = CreateConnection())
        {
            var updatedGameCollection = await connection.QuerySingleOrDefaultAsync<MoviesCollection>(new CommandDefinition(@"UPDATE MoviesCollections 
SET Name=@Name, Description=@Description, ImageSource=@ImageSource 
WHERE Id=@Id
RETURNING Description, Name, Id;", new
            {
                Name = entity.Name,
                ImageSource = entity.ImageSource,
                Id = id
            }, cancellationToken: cancellationToken));

            return updatedGameCollection;
        }
    }
}
