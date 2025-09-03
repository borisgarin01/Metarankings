using Dapper;
using Data.Repositories.Interfaces.Derived;
using Domain.Movies;
using Domain.RequestsModels.Movies.MoviesViewersReviews;
using Domain.Reviews;
using IdentityLibrary.DTOs;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Data.Repositories.Classes.Derived.Movies;

public sealed class MoviesViewersReviewsRepository : Repository, IMoviesViewersReviewsRepository
{
    public MoviesViewersReviewsRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task<long> AddAsync(AddMovieViewerReviewWithUserIdAndDateModel entity)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var insertedMovieViewerReviewId = await connection.QueryFirstOrDefaultAsync<long>(@"
INSERT INTO ViewersMoviesReviews(ViewerId, MovieId, Score, TextContent, Date)
VALUES(@ViewerId, @MovieId, @Score, @TextContent, @Date);",
 new
 {
     ViewerId = entity.UserId,
     MovieId = entity.MovieId,
     Score = entity.Score,
     TextContent = entity.TextContent,
     Date = entity.TimeStamp
 });

            return insertedMovieViewerReviewId;
        }
    }

    public async Task<IEnumerable<MovieReview>> GetByTimespanAsync(DateTime dateFrom, DateTime dateTo)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var moviesReviewsForTimespan = await connection.QueryAsync<MovieReview, Movie, ApplicationUser, MovieReview>(@"
SELECT 
    vmr.Id, vmr.MovieId, vmr.UserId, vmr.Score, vmr.TextContent, vmr.Date,
    m.Id, m.Name, m.OriginalName, m.Image, m.Score, m.ScoresCount, m.PremierDate, m.Description,
    au.Id, au.UserName, au.NormalizedUserName, au.Email, au.NormalizedEmail, 
        au.EmailConfirmed, au.PasswordHash, au.PhoneNumber, au.PhoneNumberConfirmed, au.TwoFactorEnabled
FROM ViewersMoviesReviews vmr
INNER JOIN Movies m
on vmr.MovieId=m.Id
INNER JOIN ApplicationUsers aur
on vmr.ViewerId=au.Id;", (movieReview, movie, applicationUser) =>
            {
                movieReview = movieReview with
                {
                    Movie = movie,
                    ApplicationUser = applicationUser
                };
                return movieReview;
            }, new { dateFrom, dateTo });

            return moviesReviewsForTimespan;
        }
    }


    public async Task<MovieReview> GetUserReviewForMovieAsync(long userId, long movieId)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var moviesReviewsForTimespan = await connection.QueryAsync<MovieReview, Movie, ApplicationUser, MovieReview>(@"
SELECT 
    vmr.Id, vmr.MovieId, vmr.UserId, vmr.Score, vmr.TextContent, vmr.Date 
WHERE UserId=@userId and MovieId=@movieId;", (movieReview, movie, applicationUser) =>
            {
                movieReview = movieReview with
                {
                    Movie = movie,
                    ApplicationUser = applicationUser
                };
                return movieReview;
            }, new { userId, movieId });

            return moviesReviewsForTimespan.FirstOrDefault();
        }
    }

    public Task AddRangeAsync(IEnumerable<AddMovieViewerReviewWithUserIdAndDateModel> entities)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<MovieReview>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<MovieReview> GetAsync(long id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<MovieReview>> GetAsync(long offset, long limit)
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

    public Task<MovieReview> UpdateAsync(UpdateMovieViewerReviewWithUserIdAndDateModel entity, long id)
    {
        throw new NotImplementedException();
    }
}
