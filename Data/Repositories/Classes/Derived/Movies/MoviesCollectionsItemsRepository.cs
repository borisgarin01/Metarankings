using Data.Repositories.Interfaces;
using Domain.Games;
using Domain.Games.Collections;
using Domain.Movies;
using Domain.Movies.Collections;
using Domain.RequestsModels.Movies.Collections;

namespace Data.Repositories.Classes.Derived.Games;

public sealed class MoviesCollectionsItemsRepository : Repository<MoviesCollectionItem, AddMoviesCollectionItemModel, UpdateMoviesCollectionItemModel>
{
    public MoviesCollectionsItemsRepository(string connectionString) : base(connectionString)
    {
    }

    public override async Task<long> AddAsync(AddMoviesCollectionItemModel entity, CancellationToken cancellationToken = default)
    {
        using (var connection = CreateConnection())
        {
            var insertedGameCollectionItemId = await connection.QuerySingleAsync<long>(new CommandDefinition(@"
INSERT INTO MoviesCollectionsItems (MovieId, MovieCollectionId)
VALUES (@MovieId, @MoviesCollectionId)
RETURNING Id;", new { entity.MovieId, entity.MoviesCollectionId }, cancellationToken: cancellationToken));

            return insertedGameCollectionItemId;
        }
    }

    public override async Task<IEnumerable<MoviesCollectionItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using (var connection = CreateConnection())
        {
            var moviesCollectionItems = await connection.QueryAsync<MoviesCollectionItem, Movie, MoviesCollection, MoviesCollectionItem>(new CommandDefinition(@"
SELECT mci.Id, mci.MovieId, mci.MovieCollectionId,
m.Id, m.Name, m.OriginalName, m.ImageSource, m.PremierDate, m.Description, g.Trailer, g.LocalizationId,
mc.Id, mc.Name, mc.Description, mc.ImageSource
FROM MoviesCollectionsItems mci
LEFT JOIN Movies m
on m.Id=mci.MovieId
LEFT JOIN MoviesCollections mc 
ON mc.Id=mci.MovieCollectionId;", cancellationToken: cancellationToken), (moviesCollectionItem, movie, moviesCollection) =>
            {
                if (movie is not null && moviesCollection is not null && !moviesCollection.MoviesCollectionItems.Any(m => m.Id == moviesCollectionItem.Id))
                {
                    moviesCollectionItem.Movie = movie;
                    moviesCollectionItem.MovieId = movie.Id;
                    moviesCollectionItem.MoviesCollection = moviesCollection;
                    moviesCollectionItem.MovieCollectionId = moviesCollection.Id;
                    moviesCollection.MoviesCollectionItems.Add(moviesCollectionItem);
                }

                return moviesCollectionItem;
            }, splitOn: "Id,Id");

            return moviesCollectionItems;
        }
    }

    public override async Task<MoviesCollectionItem> GetAsync(long id, CancellationToken cancellationToken = default)
    {
        using (var connection = CreateConnection())
        {
            var result = await connection.QueryAsync<MoviesCollectionItem, Movie, MoviesCollectionItem>(
                new CommandDefinition(@"SELECT mci.Id, mci.MovieId, mci.MovieCollectionId,
                    m.Id, m.Name, m.OriginalName, m.ImageSource, 
                    m.PremierDate, m.Description
             FROM MoviesCollectionsItems mci
             LEFT JOIN Movies m ON m.Id = mci.MovieId
             WHERE mci.Id = @Id;", new { Id = id }, cancellationToken: cancellationToken),
                (moviesCollectionItem, movie) =>
                {
                    moviesCollectionItem.Movie = movie;
                    return moviesCollectionItem;
                },
                splitOn: "Id"); // Add this to specify where the second object starts

            return result.FirstOrDefault();
        }
    }

    public override async Task<IEnumerable<MoviesCollectionItem>> GetAsync(long offset, long limit, CancellationToken cancellationToken = default)
    {
        using (var connection = CreateConnection())
        {
            var moviesCollectionsItems = await connection.QueryAsync<MoviesCollectionItem, Movie, MoviesCollectionItem>(new CommandDefinition(@"SELECT mci.Id, mci.MovieId, mci.MovieCollectionId,
m.Id, m.Name, m.OriginalName, m.ImageSource, m.PremierDate, m.Description
FROM MoviesCollectionsItems mci
ON m.Id=mci.MovieCollectionId
LEFT JOIN Movies m
on m.Id=mci.MovieId
ORDER BY mc.Id ASC
OFFSET @Offset LIMIT @Limit;", new { Offset = offset, Limit = limit }, cancellationToken: cancellationToken), (movieCollectionItem, movie) =>
            {
                movieCollectionItem.Movie = movie;

                return movieCollectionItem;
            });

            var moviesCollectionsItemsResult = moviesCollectionsItems
                .GroupBy(b => new { b.MovieId, b.MovieCollectionId })
                .Select(m =>
                {
                    return m.First();
                });

            return moviesCollectionsItemsResult;
        }
    }

    public override async Task RemoveAsync(long id, CancellationToken cancellationToken = default)
    {
        using (var connection = CreateConnection())
        {
            await connection.ExecuteAsync(new CommandDefinition(@"DELETE FROM MoviesCollectionsItems
WHERE Id=@Id;", new { Id = id }, cancellationToken: cancellationToken));
        }
    }

    public override async Task<MoviesCollectionItem> UpdateAsync(UpdateMoviesCollectionItemModel entity, long id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
