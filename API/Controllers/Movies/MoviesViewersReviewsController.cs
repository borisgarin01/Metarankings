using Data.Repositories.Interfaces.Derived;
using Domain.Movies;
using Domain.RequestsModels.Movies.MoviesViewersReviews;
using Domain.Reviews;
using IdentityLibrary.DTOs;
using IdentityLibrary.Telegram;

namespace API.Controllers.Movies
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class MoviesViewersReviewsController : ControllerBase
    {
        private readonly IMoviesViewersReviewsRepository _moviesViewersReviewsRepository;
        private readonly IMoviesRepository _moviesRepository;

        private readonly UserManager<ApplicationUser> _usersManager;

        private readonly TelegramAuthenticator _telegramAuthenticator;

        private readonly ILogger<MoviesViewersReviewsController> _logger;

        public MoviesViewersReviewsController(IMoviesViewersReviewsRepository moviesViewersReviewsRepository, TelegramAuthenticator telegramAuthenticator, IMoviesRepository moviesRepository, UserManager<ApplicationUser> usersManager, ILogger<MoviesViewersReviewsController> logger)
        {
            _moviesViewersReviewsRepository = moviesViewersReviewsRepository;
            _telegramAuthenticator = telegramAuthenticator;
            _moviesRepository = moviesRepository;
            _usersManager = usersManager;
            _logger = logger;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> AddMovieViewerReviewAsync(AddMovieViewerReviewModel addMovieViewerReviewModel)
        {
            long userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            MovieReview movieViewerReviewToCheckExistance = await _moviesViewersReviewsRepository.GetUserReviewForMovieAsync(userId, addMovieViewerReviewModel.MovieId);

            if (movieViewerReviewToCheckExistance is not null)
                return BadRequest($"У пользователя {userId} уже есть отзыв на фильм {addMovieViewerReviewModel.MovieId}");

            Movie movie = await _moviesRepository.GetAsync(addMovieViewerReviewModel.MovieId);
            if (movie is null)
                return NotFound("Movie not found");

            var addGameReviewWithUserIdAndDateModel = new AddMovieViewerReviewWithUserIdAndDateModel(addMovieViewerReviewModel.MovieId, addMovieViewerReviewModel.TextContent, addMovieViewerReviewModel.Score, long.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value), DateTime.Now);

            var movieReviewId = await _moviesViewersReviewsRepository.AddAsync(addGameReviewWithUserIdAndDateModel);
            var createdMovieReview = await _moviesViewersReviewsRepository.GetAsync(movieReviewId);
            await _telegramAuthenticator.SendMessageAsync($"New movie review for movie {movie.Name} at {Request.Scheme}://{Request.Host}{Request.PathBase}/movies/details/{createdMovieReview.MovieId}");
            return Created($"api/MoviesViewersReviews/{createdMovieReview.Id}", createdMovieReview);

        }

        [HttpGet("{offset:long}/{limit:long}")]
        public async Task<ActionResult<IEnumerable<MovieReview>>> GetReviewsAsync(long offset, long limit)
        {
            IEnumerable<MovieReview> movieReviews = await _moviesViewersReviewsRepository.GetAsync(offset, limit);
            return Ok(movieReviews);
        }

        [HttpGet("{id:long}")]
        public async Task<ActionResult<MovieReview>> GetReview(long id)
        {
            MovieReview movieReview = await _moviesViewersReviewsRepository.GetAsync(id);
            if (movieReview is null)
                return NotFound();
            return Ok(movieReview);
        }

        [HttpPut("{id:long}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<MovieReview>> UpdateReview(long id, UpdateMovieViewerReviewWithUserIdAndDateModel updateMovieViewerReviewWithUserIdAndDateModel)
        {
            MovieReview movieReview = await _moviesViewersReviewsRepository.GetAsync(id);
            if (movieReview is null)
                return NotFound();

            if (long.Parse(User.Claims.First(a => a.Type == ClaimTypes.NameIdentifier).Value) != movieReview.ViewerId)
                return BadRequest("User are not a review author");

            else
                try
                {
                    MovieReview updatedMovieReview = await _moviesViewersReviewsRepository.UpdateAsync(updateMovieViewerReviewWithUserIdAndDateModel, id);
                    return Ok(updatedMovieReview);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
                    return StatusCode(500, $"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
                }
        }

        [HttpDelete("{id:long}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<MovieReview>> RemoveReview(long id)
        {
            MovieReview movieReview = await _moviesViewersReviewsRepository.GetAsync(id);
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
    }
}
