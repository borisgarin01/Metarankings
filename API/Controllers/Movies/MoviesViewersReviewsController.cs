using API.Auth;
using Data.Repositories.Classes.Derived.Movies;
using Data.Repositories.Interfaces.Derived;
using Domain.Movies;
using Domain.RequestsModels.Movies.MoviesViewersReviews;
using Domain.Reviews;
using FrontendShift = Domain.RequestsModels.Games.GamesGamersReviews.Shifts.Frontend;
using BackendShift = Domain.RequestsModels.Games.GamesGamersReviews.Shifts.Backend;

namespace API.Controllers.Movies;

[ApiController]
[Route("api/movies/[controller]")]
public sealed class MoviesViewersReviewsController : ControllerBase
{
    private readonly IMoviesViewersReviewsRepository _moviesViewersReviewsRepository;
    private readonly IMoviesRepository _moviesRepository;
    private readonly MoviesViewersReviewsShiftsRepository _moviesViewersReviewsShiftsRepository;

    private readonly ILogger<MoviesViewersReviewsController> _logger;

    public MoviesViewersReviewsController(IMoviesViewersReviewsRepository moviesViewersReviewsRepository, IMoviesRepository moviesRepository, ILogger<MoviesViewersReviewsController> logger, MoviesViewersReviewsShiftsRepository moviesViewersReviewsShiftsRepository)
    {
        _moviesViewersReviewsRepository = moviesViewersReviewsRepository;
        _moviesViewersReviewsShiftsRepository = moviesViewersReviewsShiftsRepository;
        _moviesRepository = moviesRepository;
        _logger = logger;
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult> AddMovieViewerReviewAsync(AddMovieViewerReviewModel addMovieViewerReviewModel)
    {
        if (User.GetUserId() is not long userId)
            return Unauthorized();

        MovieViewerReview? existingReview = await _moviesViewersReviewsRepository.GetUserReviewForMovieAsync(userId, addMovieViewerReviewModel.MovieId);
        if (existingReview is not null)
            return BadRequest($"У пользователя {userId} уже есть отзыв на фильм {addMovieViewerReviewModel.MovieId}");

        Movie? movie = await _moviesRepository.GetAsync(addMovieViewerReviewModel.MovieId);
        if (movie is null)
            return NotFound("Movie not found");

        if (!movie.IsReleased)
            return BadRequest("Оценки и отзывы можно оставлять только после выхода фильма");

        AddMovieViewerReviewWithUserIdAndDateModel addMovieReviewWithUserIdAndDateModel = new(addMovieViewerReviewModel.MovieId, addMovieViewerReviewModel.TextContent, addMovieViewerReviewModel.Score, userId, DateTime.Now);

        long movieReviewId = await _moviesViewersReviewsRepository.AddAsync(addMovieReviewWithUserIdAndDateModel);
        MovieViewerReview createdMovieReview = await _moviesViewersReviewsRepository.GetAsync(movieReviewId);
        return Created($"api/MoviesViewersReviews/{createdMovieReview.Id}", createdMovieReview);
    }

    [HttpGet("{offset:long}/{limit:long}")]
    public async Task<ActionResult<IEnumerable<MovieViewerReview>>> GetReviewsAsync(long offset, long limit)
    {
        return Ok(await _moviesViewersReviewsRepository.GetAsync(offset, limit));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<MovieViewerReview>> GetReview(long id)
    {
        MovieViewerReview? movieReview = await _moviesViewersReviewsRepository.GetAsync(id);
        if (movieReview is null)
            return NotFound();

        return Ok(movieReview);
    }

    [HttpPut("{id:long}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult<MovieViewerReview>> UpdateReview(long id, UpdateMovieViewerReviewModel updateMovieViewerReviewModel)
    {
        MovieViewerReview? movieReview = await _moviesViewersReviewsRepository.GetAsync(id);
        if (movieReview is null)
            return NotFound();

        if (User.GetUserId() != movieReview.ViewerId)
            return BadRequest("User are not a review author");

        Movie? movie = await _moviesRepository.GetAsync(movieReview.MovieId);
        if (movie is null || !movie.IsReleased)
            return BadRequest("Оценки и отзывы можно оставлять только после выхода фильма");

        return Ok(await _moviesViewersReviewsRepository.UpdateAsync(updateMovieViewerReviewModel, id));
    }

    [HttpDelete("{id:long}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult<MovieViewerReview>> RemoveReview(long id)
    {
        MovieViewerReview? movieReview = await _moviesViewersReviewsRepository.GetAsync(id);
        if (movieReview is null)
            return NotFound();

        if (User.GetUserId() != movieReview.ViewerId && !User.HasClaim(ClaimTypes.Role, "Admin"))
            return BadRequest("User are not a review author");

        await _moviesViewersReviewsRepository.RemoveAsync(id);
        return NoContent();
    }

    [HttpPost("shift")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult<long>> Shift(FrontendShift.AddMovieViewerReviewShiftModel addMovieViewerReviewShiftModel)
    {
        if (User.GetUserId() is not long shifterId)
            return Unauthorized();

        _logger.LogInformation("MovieViewerReviewId - {MovieViewerReviewId}, Direction - {Direction}, ShifterId - {ShifterId}", addMovieViewerReviewShiftModel.MovieViewerReviewId, addMovieViewerReviewShiftModel.Direction, shifterId);

        MovieViewerReview? movieReview = await _moviesViewersReviewsRepository.GetAsync(addMovieViewerReviewShiftModel.MovieViewerReviewId);
        if (movieReview is null)
            return NotFound("Отзыв не найден");

        if (shifterId == movieReview.ViewerId)
            return BadRequest("Нельзя голосовать за свои обзоры");

        MovieViewerReviewShift? shift = await _moviesViewersReviewsShiftsRepository.GetByShifterIdAsync(shifterId, movieReview.Id);
        if (shift is null)
        {
            long insertedShiftId = await _moviesViewersReviewsShiftsRepository.AddAsync(new BackendShift.AddMovieViewerReviewShiftModel(movieReview.Id, shifterId, addMovieViewerReviewShiftModel.Direction));
            return Ok(insertedShiftId);
        }

        if (shift.Direction != addMovieViewerReviewShiftModel.Direction)
        {
            MovieViewerReviewShift updatedShift = await _moviesViewersReviewsShiftsRepository.UpdateAsync(new BackendShift.UpdateMovieViewerReviewShiftModel(shift.MovieViewerReviewId, shift.ShifterId, addMovieViewerReviewShiftModel.Direction), shift.Id);
            return Ok(updatedShift.Id);
        }

        return BadRequest("Пользователь уже голосовал за обзор");
    }
}
