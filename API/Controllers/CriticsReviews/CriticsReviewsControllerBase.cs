using API.Auth;
using Data.Repositories.Classes.Derived.CriticsReviews;
using Domain.RequestsModels.CriticsReviews;
using Domain.Reviews;

namespace API.Controllers.CriticsReviews;

/// <summary>
/// Рецензии критиков для игр и фильмов. Читать могут все, изменять - только администратор.
/// </summary>
[ApiController]
public abstract class CriticsReviewsControllerBase : ControllerBase
{
    private readonly CriticsReviewsRepository _criticsReviewsRepository;
    private readonly ILogger _logger;

    protected CriticsReviewsControllerBase(CriticsReviewsRepository criticsReviewsRepository, ILogger logger)
    {
        _criticsReviewsRepository = criticsReviewsRepository;
        _logger = logger;
    }

    [HttpGet("entity/{entityId:long}")]
    public async Task<ActionResult<IEnumerable<CriticReview>>> GetByEntityAsync(long entityId, CancellationToken cancellationToken = default)
    {
        return Ok(await _criticsReviewsRepository.GetByEntityAsync(entityId, cancellationToken));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<CriticReview>> GetAsync(long id, CancellationToken cancellationToken = default)
    {
        CriticReview? criticReview = await _criticsReviewsRepository.GetAsync(id, cancellationToken);
        if (criticReview is null)
            return NotFound();

        return Ok(criticReview);
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Admin")]
    public async Task<ActionResult<CriticReview>> AddAsync(CriticReviewModel criticReviewModel, CancellationToken cancellationToken = default)
    {
        if (User.GetUserId() is not long userId)
            return Unauthorized();

        try
        {
            long id = await _criticsReviewsRepository.AddAsync(criticReviewModel, userId, cancellationToken);
            return Created($"{Request.Path}/{id}", await _criticsReviewsRepository.GetAsync(id, cancellationToken));
        }
        catch (Npgsql.PostgresException ex) when (ex.SqlState == Npgsql.PostgresErrorCodes.UniqueViolation)
        {
            return Conflict("Рецензия этого издания уже добавлена");
        }
        catch (Npgsql.PostgresException ex) when (ex.SqlState == Npgsql.PostgresErrorCodes.ForeignKeyViolation)
        {
            return NotFound("Игра или фильм не найдены");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add critic review for {EntityId}", criticReviewModel.EntityId);
            return StatusCode(500);
        }
    }

    [HttpPut("{id:long}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Admin")]
    public async Task<ActionResult<CriticReview>> UpdateAsync(long id, CriticReviewModel criticReviewModel, CancellationToken cancellationToken = default)
    {
        try
        {
            CriticReview? updatedCriticReview = await _criticsReviewsRepository.UpdateAsync(criticReviewModel, id, cancellationToken);
            if (updatedCriticReview is null)
                return NotFound();

            return Ok(updatedCriticReview);
        }
        catch (Npgsql.PostgresException ex) when (ex.SqlState == Npgsql.PostgresErrorCodes.UniqueViolation)
        {
            return Conflict("Рецензия этого издания уже добавлена");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update critic review {Id}", id);
            return StatusCode(500);
        }
    }

    [HttpDelete("{id:long}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Admin")]
    public async Task<ActionResult> RemoveAsync(long id, CancellationToken cancellationToken = default)
    {
        if (await _criticsReviewsRepository.GetAsync(id, cancellationToken) is null)
            return NotFound();

        await _criticsReviewsRepository.RemoveAsync(id, cancellationToken);
        return NoContent();
    }
}
