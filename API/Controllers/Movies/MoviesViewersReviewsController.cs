using Data.Repositories.Classes.Derived.Movies;
using Data.Repositories.Interfaces.Derived;
using Domain.Movies;
using Domain.RequestsModels.Movies.MoviesViewersReviews;
using Domain.Reviews;
using IdentityLibrary.DTOs;

namespace API.Controllers.Movies;

[ApiController]
[Route("api/movies/[controller]")]
public sealed class MoviesViewersReviewsController : ControllerBase
{
    private readonly IMoviesViewersReviewsRepository _moviesViewersReviewsRepository;
    private readonly IMoviesRepository _moviesRepository;
    private readonly MoviesViewersReviewsShiftsRepository _moviesViewersReviewsShiftsRepository;

    private readonly UserManager<ApplicationUser> _usersManager;

    private readonly ILogger<MoviesViewersReviewsController> _logger;

    public MoviesViewersReviewsController(IMoviesViewersReviewsRepository moviesViewersReviewsRepository, IMoviesRepository moviesRepository, UserManager<ApplicationUser> usersManager, ILogger<MoviesViewersReviewsController> logger, MoviesViewersReviewsShiftsRepository moviesViewersReviewsShiftsRepository)
    {
        _moviesViewersReviewsRepository = moviesViewersReviewsRepository;
        _moviesViewersReviewsShiftsRepository = moviesViewersReviewsShiftsRepository;
        _moviesRepository = moviesRepository;
        _usersManager = usersManager;
        _logger = logger;
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult> AddMovieViewerReviewAsync(AddMovieViewerReviewModel addMovieViewerReviewModel)
    {
        long userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        MovieViewerReview movieViewerReviewToCheckExistance = await _moviesViewersReviewsRepository.GetUserReviewForMovieAsync(userId, addMovieViewerReviewModel.MovieId);

        if (movieViewerReviewToCheckExistance is not null)
            return BadRequest($"У пользователя {userId} уже есть отзыв на фильм {addMovieViewerReviewModel.MovieId}");

        Movie movie = await _moviesRepository.GetAsync(addMovieViewerReviewModel.MovieId);
        if (movie is null)
            return NotFound("Movie not found");

        if (!movie.IsReleased)
            return BadRequest("Оценки и отзывы можно оставлять только после выхода фильма");

        var addGameReviewWithUserIdAndDateModel = new AddMovieViewerReviewWithUserIdAndDateModel(addMovieViewerReviewModel.MovieId, addMovieViewerReviewModel.TextContent, addMovieViewerReviewModel.Score, long.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value), DateTime.Now);

        var movieReviewId = await _moviesViewersReviewsRepository.AddAsync(addGameReviewWithUserIdAndDateModel);
        var createdMovieReview = await _moviesViewersReviewsRepository.GetAsync(movieReviewId);
        return Created($"api/MoviesViewersReviews/{createdMovieReview.Id}", createdMovieReview);

    }

    [HttpGet("{offset:long}/{limit:long}")]
    public async Task<ActionResult<IEnumerable<MovieViewerReview>>> GetReviewsAsync(long offset, long limit)
    {
        IEnumerable<MovieViewerReview> movieReviews = await _moviesViewersReviewsRepository.GetAsync(offset, limit);
        return Ok(movieReviews);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<MovieViewerReview>> GetReview(long id)
    {
        MovieViewerReview movieReview = await _moviesViewersReviewsRepository.GetAsync(id);
        if (movieReview is null)
            return NotFound();
        return Ok(movieReview);
    }

    [HttpPut("{id:long}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult<MovieViewerReview>> UpdateReview(long id, UpdateMovieViewerReviewModel updateMovieViewerReviewModel)
    {
        MovieViewerReview movieReview = await _moviesViewersReviewsRepository.GetAsync(id);
        if (movieReview is null)
            return NotFound();

        if (long.Parse(User.Claims.First(a => a.Type == ClaimTypes.NameIdentifier).Value) != movieReview.ViewerId)
            return BadRequest("User are not a review author");

        Movie movie = await _moviesRepository.GetAsync(movieReview.MovieId);
        if (movie is null || !movie.IsReleased)
            return BadRequest("Оценки и отзывы можно оставлять только после выхода фильма");

        else
            try
            {
                MovieViewerReview updatedMovieReview = await _moviesViewersReviewsRepository.UpdateAsync(updateMovieViewerReviewModel, id);
                return Ok(updatedMovieReview);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
                return StatusCode(500, $"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
    }

    [HttpDelete("{id:long}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult<MovieViewerReview>> RemoveReview(long id)
    {
        MovieViewerReview movieReview = await _moviesViewersReviewsRepository.GetAsync(id);
        if (movieReview is null)
            return NotFound();

        if (long.Parse(User.Claims.First(a => a.Type == ClaimTypes.NameIdentifier).Value) != movieReview.ViewerId
            && User.Claims.FirstOrDefault(a => a.Type == ClaimTypes.Role && a.Value == "Admin") is null)
            return BadRequest("User are not a review author");

        try
        {
            await _moviesViewersReviewsRepository.RemoveAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            return StatusCode(500, $"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
        }
    }

    [HttpPost("shift")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult<long>> Shift(Domain.RequestsModels.Games.GamesGamersReviews.Shifts.Frontend.AddMovieViewerReviewShiftModel addMovieViewerReviewShiftModel)
    {
        try
        {
            long shifterId = long.Parse(User.Claims.First(a => a.Type == ClaimTypes.NameIdentifier).Value);

            _logger.LogInformation("MovieViewerReviewId - {MovieViewerReviewId}, Direction - {Direction}, ShifterId - {ShifterId}", addMovieViewerReviewShiftModel.MovieViewerReviewId, addMovieViewerReviewShiftModel.Direction, shifterId);

            MovieViewerReview movieReview = await _moviesViewersReviewsRepository.GetAsync(addMovieViewerReviewShiftModel.MovieViewerReviewId);

            if (movieReview is null)
            {
                _logger.LogWarning("movieReview is null");
                return NotFound("Отзыв не найден");
            }

            if (shifterId == movieReview.ViewerId)
            {
                _logger.LogWarning("shifterId == movieReview.ViewerId. Нельзя голосовать за свои обзоры");
                return BadRequest("Нельзя голосовать за свои обзоры");
            }

            MovieViewerReviewShift shift = await _moviesViewersReviewsShiftsRepository.GetByShifterIdAsync(shifterId, movieReview.Id);
            if (shift is null)
            {
                long insertedShift = await _moviesViewersReviewsShiftsRepository.AddAsync(new Domain.RequestsModels.Games.GamesGamersReviews.Shifts.Backend.AddMovieViewerReviewShiftModel(movieReview.Id, shifterId, addMovieViewerReviewShiftModel.Direction));
                return Ok(insertedShift);
            }
            else if (shift.Direction != addMovieViewerReviewShiftModel.Direction)
            {
                MovieViewerReviewShift updatedShift = await _moviesViewersReviewsShiftsRepository.UpdateAsync(new Domain.RequestsModels.Games.GamesGamersReviews.Shifts.Backend.UpdateMovieViewerReviewShiftModel(shift.MovieViewerReviewId, shift.ShifterId, addMovieViewerReviewShiftModel.Direction), shift.Id);
                return Ok(updatedShift.Id);
            }

            _logger.LogWarning("Пользователь уже голосовал за обзор");
            return BadRequest("Пользователь уже голосовал за обзор");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при голосовании за отзыв о фильме");
            return StatusCode(500, ex.Message);
        }
    }
}
