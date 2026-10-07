using API.Auth;
using Data.Repositories.Classes.Derived.ContentRequests;
using Domain.ContentRequests;
using Domain.RequestsModels.ContentRequests;

namespace API.Controllers;

/// <summary>
/// Заявки пользователей на добавление игр и фильмов, которых нет в базе.
/// Пользователь видит только свои заявки, администратор - все.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public sealed class ContentRequestsController : ControllerBase
{
    /// <summary>
    /// Сколько нерассмотренных заявок может висеть у одного пользователя (защита от спама).
    /// </summary>
    private const int MaxPendingRequestsPerUser = 10;

    private readonly ContentRequestsRepository _contentRequestsRepository;
    private readonly ILogger<ContentRequestsController> _logger;

    public ContentRequestsController(ContentRequestsRepository contentRequestsRepository, ILogger<ContentRequestsController> logger)
    {
        _contentRequestsRepository = contentRequestsRepository;
        _logger = logger;
    }

    // GET: api/ContentRequests?status=0
    [HttpGet]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<ContentRequest>>> GetAllAsync([FromQuery] ContentRequestStatus? status)
    {
        if (status is not null && !Enum.IsDefined(status.Value))
            return BadRequest("Неизвестный статус заявки");

        return Ok(await _contentRequestsRepository.GetAllAsync(status));
    }

    // GET: api/ContentRequests/my
    [HttpGet("my")]
    public async Task<ActionResult<IEnumerable<ContentRequest>>> GetMineAsync()
    {
        long? userId = User.GetUserId();
        if (userId is null)
            return Unauthorized();

        return Ok(await _contentRequestsRepository.GetByUserAsync(userId.Value));
    }

    // GET: api/ContentRequests/5
    [HttpGet("{id:long}")]
    public async Task<ActionResult<ContentRequest>> GetAsync(long id)
    {
        long? userId = User.GetUserId();
        if (userId is null)
            return Unauthorized();

        ContentRequest? contentRequest = await _contentRequestsRepository.GetAsync(id);

        // Чужую заявку не показываем и не раскрываем, что она существует
        if (contentRequest is null || (contentRequest.UserId != userId && !User.IsInRole("Admin")))
            return NotFound();

        return Ok(contentRequest);
    }

    // POST: api/ContentRequests
    [HttpPost]
    public async Task<ActionResult<ContentRequest>> AddAsync(AddContentRequestModel addContentRequestModel)
    {
        long? userId = User.GetUserId();
        if (userId is null)
            return Unauthorized();

        if (string.IsNullOrWhiteSpace(addContentRequestModel.Title))
            return BadRequest("Укажите название");

        if (await _contentRequestsRepository.CountPendingByUserAsync(userId.Value) >= MaxPendingRequestsPerUser)
            return StatusCode(StatusCodes.Status429TooManyRequests, $"У вас уже {MaxPendingRequestsPerUser} заявок на рассмотрении. Дождитесь ответа администратора.");

        try
        {
            long id = await _contentRequestsRepository.AddAsync(addContentRequestModel, userId.Value);
            ContentRequest? created = await _contentRequestsRepository.GetAsync(id);
            return Created($"/api/ContentRequests/{id}", created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save content request from user {UserId}", userId);
            return StatusCode(500);
        }
    }

    // PUT: api/ContentRequests/5/status
    [HttpPut("{id:long}/status")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public async Task<ActionResult<ContentRequest>> UpdateStatusAsync(long id, UpdateContentRequestStatusModel updateContentRequestStatusModel)
    {
        ContentRequest? updated = await _contentRequestsRepository.UpdateStatusAsync(id, updateContentRequestStatusModel);
        if (updated is null)
            return NotFound();

        return Ok(updated);
    }

    /// <summary>
    /// Автор может отозвать свою заявку, пока её не рассмотрели. Администратор может удалить любую.
    /// </summary>
    // DELETE: api/ContentRequests/5
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteAsync(long id)
    {
        long? userId = User.GetUserId();
        if (userId is null)
            return Unauthorized();

        ContentRequest? contentRequest = await _contentRequestsRepository.GetAsync(id);
        bool isAdmin = User.IsInRole("Admin");

        if (contentRequest is null || (contentRequest.UserId != userId && !isAdmin))
            return NotFound();

        if (!isAdmin && contentRequest.Status != ContentRequestStatus.Pending)
            return BadRequest("Рассмотренную заявку отозвать нельзя");

        await _contentRequestsRepository.RemoveAsync(id);
        return NoContent();
    }
}
