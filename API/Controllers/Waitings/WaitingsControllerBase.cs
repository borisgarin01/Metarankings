using Data.Repositories.Classes.Derived.Waitings;
using Domain.RequestsModels.Waitings;
using Domain.Waitings;

namespace API.Controllers.Waitings;

/// <summary>
/// Общая логика голосования "жду / не жду" для игр и фильмов.
/// </summary>
[ApiController]
public abstract class WaitingsControllerBase : ControllerBase
{
    private const int MaxIdsPerRequest = 100;

    private readonly WaitingsRepository _waitingsRepository;
    private readonly ILogger _logger;

    protected WaitingsControllerBase(WaitingsRepository waitingsRepository, ILogger logger)
    {
        _waitingsRepository = waitingsRepository;
        _logger = logger;
    }

    [HttpGet("{entityId:long}")]
    public async Task<ActionResult<WaitingStatistics>> GetAsync(long entityId)
    {
        return Ok(await _waitingsRepository.GetStatisticsAsync(entityId, GetCurrentUserId()));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WaitingStatistics>>> GetAsync([FromQuery] long[] ids)
    {
        if (ids.Length > MaxIdsPerRequest)
            return BadRequest($"Нельзя запросить больше {MaxIdsPerRequest} элементов за раз");

        return Ok(await _waitingsRepository.GetStatisticsAsync(ids, GetCurrentUserId()));
    }

    /// <summary>
    /// Голос "жду" (IsWaiting = true) или "не жду" (IsWaiting = false).
    /// Повторный такой же голос отменяет его.
    /// </summary>
    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult<WaitingStatistics>> VoteAsync(AddWaitingVoteModel addWaitingVoteModel)
    {
        long? userId = GetCurrentUserId();
        if (userId is null)
            return Unauthorized();

        bool? isReleased = await _waitingsRepository.IsReleasedAsync(addWaitingVoteModel.EntityId);
        if (isReleased is null)
            return NotFound();

        if (isReleased.Value)
            return BadRequest("Голосовать за ожидание можно только до выхода");

        try
        {
            return Ok(await _waitingsRepository.VoteAsync(addWaitingVoteModel.EntityId, userId.Value, addWaitingVoteModel.IsWaiting));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save waiting vote for {EntityId}", addWaitingVoteModel.EntityId);
            return StatusCode(500);
        }
    }

    private long? GetCurrentUserId()
    {
        string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return long.TryParse(userId, out long parsedUserId) ? parsedUserId : null;
    }
}
