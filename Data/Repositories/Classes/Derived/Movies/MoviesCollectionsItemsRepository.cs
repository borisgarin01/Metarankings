using Data.Repositories.Interfaces;
using Domain.Games;
using Domain.Games.Collections;
using Domain.Movies;
using Domain.Movies.Collections;
using Domain.RequestsModels.Movies.Collections;

namespace Data.Repositories.Classes.Derived.Games;

public sealed class MoviesCollectionsItemsRepository : Repository, IRepository<MoviesCollectionItem, AddMoviesCollectionItemModel, UpdateMoviesCollectionItemModel>
{
    public MoviesCollectionsItemsRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task<long> AddAsync(AddMoviesCollectionItemModel entity)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var insertedGameCollectionItemId = await connection.QuerySingleAsync<long>(@"
INSERT INTO MoviesCollectionsItems (MovieId, MovieCollectionId)
VALUES (@MovieId, @MoviesCollectionId)
RETURNING Id;",
new { entity.MovieId, entity.MoviesCollectionId });

            return insertedGameCollectionItemId;
        }
    }

    public async Task AddRangeAsync(IEnumerable<AddMoviesCollectionItemModel> entities)
    {
        foreach (var entity in entities)
        {
            await AddAsync(entity);
        }
    }

    public async Task<IEnumerable<MoviesCollectionItem>> GetAllAsync()
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var moviesCollectionItems = await connection.QueryAsync<MoviesCollectionItem, Movie, MoviesCollection, MoviesCollectionItem>(@"
SELECT mci.Id, mci.MovieId, mci.MovieCollectionId,
m.Id, m.Name, m.OriginalName, m.ImageSource, m.PremierDate, m.Description, g.Trailer, g.LocalizationId,
mc.Id, mc.Name, mc.Description, mc.ImageSource
FROM MoviesCollectionsItems mci
LEFT JOIN Movies m
on m.Id=mci.MovieId
LEFT JOIN MoviesCollections mc 
ON mc.Id=mci.MovieCollectionId;", (moviesCollectionItem, movie, moviesCollection) =>
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

    public async Task<MoviesCollectionItem> GetAsync(long id)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var result = await connection.QueryAsync<MoviesCollectionItem, Movie, MoviesCollectionItem>(
                @"SELECT mci.Id, mci.MovieId, mci.MovieCollectionId,
                    m.Id, m.Name, m.OriginalName, m.ImageSource, 
                    m.PremierDate, m.Description
             FROM MoviesCollectionsItems mci
             LEFT JOIN Movies m ON m.Id = mci.MovieId
             WHERE mci.Id = @Id;",
                (moviesCollectionItem, movie) =>
                {
                    moviesCollectionItem.Movie = movie;
                    return moviesCollectionItem;
                },
                new { Id = id },
                splitOn: "Id"); // Add this to specify where the second object starts

            return result.FirstOrDefault();
        }
    }

    public async Task<IEnumerable<MoviesCollectionItem>> GetAsync(long offset, long limit)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            var moviesCollectionsItems = await connection.QueryAsync<MoviesCollectionItem, Movie, MoviesCollectionItem>(@"SELECT mci.Id, mci.MovieId, mci.MovieCollectionId,
m.Id, m.Name, m.OriginalName, m.ImageSource, m.PremierDate, m.Description
FROM MoviesCollectionsItems mci
ON m.Id=mci.MovieCollectionId
LEFT JOIN Movies m
on m.Id=mci.MovieId
ORDER BY mc.Id ASC
OFFSET @Offset LIMIT @Limit;", (movieCollectionItem, movie) =>
            {
                movieCollectionItem.Movie = movie;

                return movieCollectionItem;
            }, new { Offset = offset, Limit = limit });

            var moviesCollectionsItemsResult = moviesCollectionsItems
                .GroupBy(b => new { b.MovieId, b.MovieCollectionId })
                .Select(m =>
                {
                    return m.First();
                });

            return moviesCollectionsItemsResult;
        }
    }

    public async Task RemoveAsync(long id)
    {
        using (var connection = new NpgsqlConnection(ConnectionString))
        {
            await connection.ExecuteAsync(@"DELETE FROM MoviesCollectionsItems
WHERE Id=@Id;", new { Id = id });
        }
    }

    public async Task RemoveRangeAsync(IEnumerable<long> ids)
    {
        foreach (var id in ids)
        {
            await RemoveAsync(id);
        }
    }

    public async Task<MoviesCollectionItem> UpdateAsync(UpdateMoviesCollectionItemModel entity, long id)
    {
        throw new NotImplementedException();
    }
}
