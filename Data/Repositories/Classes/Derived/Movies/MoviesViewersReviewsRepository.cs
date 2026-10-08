using Data.Repositories.Interfaces.Derived;
using Domain.Movies;
using Domain.RequestsModels.Movies.MoviesViewersReviews;
using Domain.Reviews;
using IdentityLibrary.DTOs;

namespace Data.Repositories.Classes.Derived.Movies;

public sealed class MoviesViewersReviewsRepository : Repository<MovieViewerReview, AddMovieViewerReviewWithUserIdAndDateModel, UpdateMovieViewerReviewModel>, IMoviesViewersReviewsRepository
{
    public MoviesViewersReviewsRepository(string connectionString) : base(connectionString)
    {
    }

    public override async Task<long> AddAsync(AddMovieViewerReviewWithUserIdAndDateModel entity, CancellationToken cancellationToken = default)
    {
        using (var connection = CreateConnection())
        {
            var insertedMovieViewerReviewId = await connection.QueryFirstOrDefaultAsync<long>(new CommandDefinition(@"
INSERT INTO ViewersMoviesReviews(ViewerId, MovieId, Score, TextContent, Date)
VALUES(@ViewerId, @MovieId, @Score, @TextContent, @Date)
RETURNING Id;", new
 {
     ViewerId = entity.UserId,
     MovieId = entity.MovieId,
     Score = entity.Score,
     TextContent = entity.TextContent,
     Date = entity.TimeStamp
 }, cancellationToken: cancellationToken));

            return insertedMovieViewerReviewId;
        }
    }

    public async Task<IEnumerable<MovieViewerReview>> GetByTimespanAsync(DateTime dateFrom, DateTime dateTo, CancellationToken cancellationToken = default)
    {
        using (var connection = CreateConnection())
        {
            var moviesReviewsForTimespan = await connection.QueryAsync<MovieViewerReview, Movie, ApplicationUser, MovieViewerReview>(new CommandDefinition(@"
SELECT 
    vmr.MovieId, vmr.ViewerId, vmr.Score, vmr.TextContent, vmr.Date,
    m.Id, m.Name, m.OriginalName, m.Image, m.PremierDate, m.Description,
    au.Id, au.UserName, au.NormalizedUserName, au.Email, au.NormalizedEmail, 
        au.EmailConfirmed, au.PasswordHash, au.PhoneNumber, au.PhoneNumberConfirmed, au.TwoFactorEnabled
FROM ViewersMoviesReviews vmr
INNER JOIN Movies m
on vmr.MovieId=m.Id
INNER JOIN ApplicationUsers aur
on vmr.ViewerId=au.Id;", new { dateFrom, dateTo }, cancellationToken: cancellationToken), (movieReview, movie, applicationUser) =>
            {
                movieReview = movieReview with
                {
                    Movie = movie,
                    ApplicationUser = applicationUser
                };
                return movieReview;
            });

            return moviesReviewsForTimespan;
        }
    }


    public async Task<MovieViewerReview> GetUserReviewForMovieAsync(long userId, long movieId, CancellationToken cancellationToken = default)
    {
        using (var connection = CreateConnection())
        {
            var moviesReviewsForTimespan = await connection.QueryAsync<MovieViewerReview, Movie, ApplicationUser, MovieViewerReview>(new CommandDefinition(@"
SELECT vmr.Id, vmr.MovieId, vmr.ViewerId, vmr.Score, vmr.TextContent, vmr.Date,
    m.Id, m.Name, m.OriginalName, m.ImageSource, m.PremierDate, m.Description,
    au.Id, au.UserName, au.NormalizedUserName, au.Email, au.NormalizedEmail, 
        au.EmailConfirmed, au.PasswordHash, au.PhoneNumber, au.PhoneNumberConfirmed, au.TwoFactorEnabled
FROM ViewersMoviesReviews vmr
LEFT JOIN Movies m on vmr.MovieId=m.ID
LEFT JOIN ApplicationUsers au on au.Id=vmr.ViewerId
WHERE ViewerId=@userId and MovieId=@movieId;", new { userId, movieId }, cancellationToken: cancellationToken), (movieReview, movie, applicationUser) =>
            {
                movieReview = movieReview with
                {
                    Movie = movie,
                    ApplicationUser = applicationUser
                };
                return movieReview;
            });

            return moviesReviewsForTimespan.FirstOrDefault();
        }
    }

    public override Task AddRangeAsync(IEnumerable<AddMovieViewerReviewWithUserIdAndDateModel> entities, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task<IEnumerable<MovieViewerReview>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override async Task<MovieViewerReview> GetAsync(long id, CancellationToken cancellationToken = default)
    {
        using (var connection = CreateConnection())
        {
            var moviesReviewsForTimespan = await connection.QueryAsync<MovieViewerReview, Movie, ApplicationUser, MovieViewerReview>(new CommandDefinition(@"
SELECT vmr.Id, vmr.MovieId, vmr.ViewerId, vmr.Score, vmr.TextContent, vmr.Date,
    m.Id, m.Name, m.OriginalName, m.ImageSource, m.PremierDate, m.Description,
    au.Id, au.UserName, au.NormalizedUserName, au.Email, au.NormalizedEmail, 
        au.EmailConfirmed, au.PasswordHash, au.PhoneNumber, au.PhoneNumberConfirmed, au.TwoFactorEnabled
FROM ViewersMoviesReviews vmr
LEFT JOIN Movies m on vmr.MovieId=m.ID
LEFT JOIN ApplicationUsers au on au.Id=vmr.ViewerId
WHERE vmr.Id=@id;", new { id }, cancellationToken: cancellationToken), (movieReview, movie, applicationUser) =>
            {
                movieReview = movieReview with
                {
                    Movie = movie,
                    ApplicationUser = applicationUser
                };
                return movieReview;
            });

            return moviesReviewsForTimespan.FirstOrDefault();
        }
    }

    public override async Task<IEnumerable<MovieViewerReview>> GetAsync(long offset, long limit, CancellationToken cancellationToken = default)
    {
        using (var connection = CreateConnection())
        {
            var moviesReviewsForTimespan = await connection.QueryAsync<MovieViewerReview, Movie, ApplicationUser, MovieViewerReview>(new CommandDefinition(@"
SELECT vmr.Id, vmr.MovieId, vmr.ViewerId, vmr.Score, vmr.TextContent, vmr.Date,
    m.Id, m.Name, m.OriginalName, m.ImageSource, m.PremierDate, m.Description,
    au.Id, au.UserName, au.NormalizedUserName, au.Email, au.NormalizedEmail, 
        au.EmailConfirmed, au.PasswordHash, au.PhoneNumber, au.PhoneNumberConfirmed, au.TwoFactorEnabled
FROM ViewersMoviesReviews vmr
LEFT JOIN Movies m on vmr.MovieId=m.ID
LEFT JOIN ApplicationUsers au on au.Id=vmr.ViewerId
order by vmr.Id desc
OFFSET @offset ROWS FETCH NEXT @limit ROWS ONLY", new { offset, limit }, cancellationToken: cancellationToken), (movieReview, movie, applicationUser) =>
            {
                movieReview = movieReview with
                {
                    Movie = movie,
                    ApplicationUser = applicationUser
                };
                return movieReview;
            });

            return moviesReviewsForTimespan;
        }
    }


    public override async Task RemoveAsync(long id, CancellationToken cancellationToken = default)
    {
        using (var connection = CreateConnection())
        {
            // Лайки/дизлайки удаляются каскадно (ON DELETE CASCADE в ViewersMoviesReviewsShifts)
            await connection.ExecuteAsync(new CommandDefinition("DELETE FROM ViewersMoviesReviews WHERE Id=@id", new { id }, cancellationToken: cancellationToken));
        }
    }

    public override async Task<MovieViewerReview> UpdateAsync(UpdateMovieViewerReviewModel entity, long id, CancellationToken cancellationToken = default)
    {
        using (var connection = CreateConnection())
        {
            await connection.ExecuteAsync(new CommandDefinition(@"UPDATE ViewersMoviesReviews
SET TextContent=@TextContent, Score=@Score, Date=@TimeStamp
WHERE Id=@id", new
            {
                entity.TextContent,
                entity.Score,
                TimeStamp = DateTime.Now,
                id
            }, cancellationToken: cancellationToken));
        }

        return await GetAsync(id, cancellationToken);
    }
}
