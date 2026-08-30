using BlazorClient.Components.PagesComponents.Home;
using Data.Repositories.Interfaces;
using Data.Repositories.Interfaces.Derived;
using Domain.Common;
using Domain.Games;
using Domain.Games.Collections;
using Domain.Movies;
using Domain.Movies.Collections;
using Domain.RequestsModels.Games.Collections;
using Domain.RequestsModels.Movies.Collections;
using Domain.Reviews;
using ViewModels;

namespace API.Controllers.Home;

[ApiController]
[Route("api/[controller]")]
public sealed class HomeController : ControllerBase
{
    private readonly ILogger<HomeController> _logger;

    private readonly IGamesPlayersReviewsRepository _gamesPlayersReviewsRepository;
    private readonly IGamesRepository _gamesRepository;
    private readonly IMoviesRepository _moviesRepository;
    private readonly IMoviesViewersReviewsRepository _moviesViewersReviewsRepository;
    private readonly IRepository<GamesCollection, AddGamesCollectionModel, UpdateGamesCollectionModel> _gamesCollectionsRepository;
    private readonly IRepository<MoviesCollection, AddMoviesCollectionModel, UpdateMoviesCollectionModel> _moviesCollectionsRepository;

    public HomeController(IGamesPlayersReviewsRepository gamesPlayersReviewsRepository, ILogger<HomeController> logger, IGamesRepository gamesRepository, IRepository<GamesCollection, AddGamesCollectionModel, UpdateGamesCollectionModel> gamesCollectionsRepository, IRepository<MoviesCollection, AddMoviesCollectionModel, UpdateMoviesCollectionModel> moviesCollectionsRepository, IMoviesRepository moviesRepository, IMoviesViewersReviewsRepository moviesViewersReviewsRepository)
    {
        _gamesPlayersReviewsRepository = gamesPlayersReviewsRepository;
        _logger = logger;
        _gamesRepository = gamesRepository;
        _gamesCollectionsRepository = gamesCollectionsRepository;
        _moviesCollectionsRepository = moviesCollectionsRepository;
        _moviesRepository = moviesRepository;
        _moviesViewersReviewsRepository = moviesViewersReviewsRepository;
    }

    [HttpGet("collections/{pageNumber:int}/{pageSize:int}")]
    public async Task<ActionResult<IEnumerable<CollectionsItemComponent>>> GetCollectionsComponents(int pageNumber, int pageSize)
    {
        IEnumerable
            <GamesCollection> gamesCollections = await _gamesCollectionsRepository.GetAsync((pageNumber - 1) * pageSize, pageSize);
        IEnumerable<MoviesCollection> moviesCollections = await _moviesCollectionsRepository.GetAsync((pageNumber - 1) * pageSize, pageSize);

        IEnumerable<CollectionsItemComponent> collectionsItemComponent = gamesCollections
            .Select(x => new CollectionsItemComponent
            {
                Href = $"/games/collections/{x.Id}",
                ImageAlt = x.Name,
                ImageSource = x.ImageSource,
                Title = x.Name
            })
            .Union(moviesCollections
            .Select(x => new CollectionsItemComponent
            {
                Href = $"/movies/collections/{x.Id}",
                ImageAlt = x.Name,
                ImageSource = x.ImageSource,
                Title = x.Name
            }));

        return Ok(collectionsItemComponent);
    }

    [HttpGet("soon-at-cinemas")]
    public async Task<ActionResult<IEnumerable<SoonAtCinemasItemComponent>>> GetSoonAtCinemasItemsComponents()
    {
        IEnumerable<Movie> movies = await _moviesRepository.GetAsync(DateTime.Today, DateTime.Today.AddMonths(1));

        return Ok(movies.Select(m => new SoonAtCinemasItemComponent
        {
            Href = $"/movies/{m.Id}",
            ImageAlt = m.Name,
            Title = m.Name,
            ReleaseDate = m.PremierDate.HasValue ? m.PremierDate.Value : DateOnly.FromDateTime(DateTime.Today),
            OriginalName = m.OriginalName,
            ImageSource = m.ImageSource,
            Genres = m.MovieGenres.Select(b => new Link(b.Name, $"movies/{b.Id}")).ToArray()
        }));
    }

    [HttpGet("movies-reviews")]
    public async Task<ActionResult<IEnumerable<MovieReviewListViewModel>>> GetMoviesReviews()
    {
        IEnumerable<MovieViewerReview> moviesReviews = await _moviesViewersReviewsRepository.GetByTimespanAsync(DateTime.Today.AddDays(-30), DateTime.Today);

        return Ok(moviesReviews.Select(b => new MovieReviewListViewModel(b.Id, b.Movie.Name)));
    }

    [HttpGet("games-reviews/{pageNumber:long}/{pageSize:long}")]
    public async Task<ActionResult<IEnumerable<GameReviewListViewModel>>> GetGamesReviewsAsync(long pageNumber, long pageSize)
    {
        try
        {
            IEnumerable<GameReview> gamesReviews = await _gamesPlayersReviewsRepository.GetAsync((pageNumber - 1) * pageSize, pageSize);

            return Ok(gamesReviews.Select(b => new GameReviewListViewModel(b.GameId, b.Game.Name)));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message, ex.StackTrace);
            return StatusCode(500, new { ex.Message, ex.StackTrace });
        }
    }

    [HttpGet("nearest/{limit}")]
    public async Task<ActionResult<IEnumerable<GamesReleaseDateItemViewModel>>> GetNearestAsync(short limit)
    {
        IEnumerable<Game> games = await _gamesRepository.GetNearestAsync(limit);

        IEnumerable<GamesReleaseDateItemViewModel> gamesReleaseDatetItemViewModels = games.Select(b => new GamesReleaseDateItemViewModel($"/games/Details/{b.Id}", b.Name, b.Image, b.Name, b.Name, b.Platforms.Select(c => new Link(c.Name, $"/platforms/{c.Id}")).ToArray(), b.Genres.Select(c => new Link(c.Name, $"/games/genres/{c.Id}")).ToArray(), b.ReleaseDate.HasValue ? b.ReleaseDate.Value : DateOnly.FromDateTime(DateTime.Today.AddYears(5))));

        return Ok(gamesReleaseDatetItemViewModels);
    }

    [HttpGet("games-release-dates/{pageNumber}/{pageSize}")]
    public async Task<ActionResult<IEnumerable<GamesReleaseDateItemComponent>>> GetGamesReleasesDatesAsync(int pageNumber, int pageSize)
    {
        IEnumerable<Game> nearestNextReleaseDatesGames = await _gamesRepository.GetLastAsync((pageNumber - 1) * pageSize, pageSize);

        IEnumerable<GamesReleaseDateItemViewModel> gamesReleaseDateItemsComponents = nearestNextReleaseDatesGames
            .Select(b => new GamesReleaseDateItemViewModel($"/games/Details/{b.Id}", b.Name, b.Image, b.Name, b.Name, b.Platforms.Select(a => new Link(a.Name, $"/platforms/{a.Id}")).ToArray(), b.Genres.Select(a => new Link(a.Name, $"/games/genres/{a.Id}")).ToArray(), b.ReleaseDate.HasValue ? b.ReleaseDate.Value : default));

        return Ok(gamesReleaseDateItemsComponents);
    }
}
