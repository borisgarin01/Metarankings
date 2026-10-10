using API.Auth;
using Data.Repositories.Classes.Derived;
using Domain.Common.News;

namespace API.Controllers;

[ApiController]
[OutputCache(PolicyName = CachePolicies.PublicRead, Tags = new[] { CacheTags.News })]
[Route("api/[controller]")]
public sealed class NewsController : ControllerBase
{
    private readonly NewsRepository _newsRepository;

    public NewsController(NewsRepository newsRepository)
    {
        _newsRepository = newsRepository;
    }

    // GET: api/news
    [HttpGet]
    public async Task<ActionResult<IEnumerable<NewsItem>>> GetAll(
        [FromQuery] long? offset,
        [FromQuery] long? limit)
    {
        IEnumerable<NewsItem> items;

        if (offset.HasValue && limit.HasValue)
        {
            items = await _newsRepository.GetAsync(offset.Value, limit.Value);
        }
        else
        {
            items = await _newsRepository.GetAllAsync();
        }

        return Ok(items);
    }

    // GET: api/news/5
    [HttpGet("{id:long}")]
    public async Task<ActionResult<NewsItem>> GetById(long id)
    {
        NewsItem? item = await _newsRepository.GetAsync(id);

        if (item is null)
        {
            return NotFound();
        }

        return Ok(item);
    }

    // POST: api/news
    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public async Task<ActionResult<NewsItem>> Create([FromBody] AddNewsItemModel addNewsItemModel)
    {
        long userId = User.GetUserId() ?? throw new InvalidOperationException("User identifier claim is missing or invalid.");

        // Маппинг: AddNewsItemModel (frontend) -> AddNewsItemDbModel (backend)
        var dbModel = new AddNewsItemDbModel
        {
            Title = addNewsItemModel.Title,
            TextContent = addNewsItemModel.TextContent,
            ImageSource = addNewsItemModel.ImageSource,
            UserId = userId,
            PublishTimestamp = DateTime.Now
        };

        long newId;
        try
        {
            newId = await _newsRepository.AddAsync(dbModel);
        }
        catch (Npgsql.PostgresException ex) when (ex.SqlState == "23505")
        {
            return Conflict(new { message = "News with the same title already exists." });
        }
        catch (Npgsql.PostgresException ex) when (ex.SqlState == "23503")
        {
            return BadRequest(new { message = "Specified user does not exist." });
        }

        NewsItem created = await _newsRepository.GetAsync(newId);
        return CreatedAtAction(nameof(GetById), new { id = newId }, created);
    }

    // PUT: api/news/5
    [HttpPut("{id:long}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public async Task<ActionResult<NewsItem>> Update(
        long id,
        [FromBody] UpdateNewsItemModel request)
    {
        long userId = User.GetUserId() ?? throw new InvalidOperationException("User identifier claim is missing or invalid.");

        // Маппинг: UpdateNewsItemModel (frontend) -> UpdateNewsItemDbModel (backend)
        var dbModel = new UpdateNewsItemDbModel
        {
            Title = request.Title,
            TextContent = request.TextContent,
            ImageSource = request.ImageSource,
            UserId = userId
        };

        NewsItem? updated;
        try
        {
            updated = await _newsRepository.UpdateAsync(dbModel, id);
        }
        catch (Npgsql.PostgresException ex) when (ex.SqlState == "23505")
        {
            return Conflict(new { message = "News with the same title already exists." });
        }
        catch (Npgsql.PostgresException ex) when (ex.SqlState == "23503")
        {
            return BadRequest(new { message = "Specified user does not exist." });
        }

        if (updated is null)
        {
            return NotFound();
        }

        return Ok(updated);
    }

    // DELETE: api/news/5
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(long id)
    {
        await _newsRepository.RemoveAsync(id);
        return NoContent();
    }

    // DELETE: api/news/batch
    [HttpDelete("batch")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteRange([FromBody] long[] ids)
    {
        if (ids is null || ids.Length == 0)
        {
            return BadRequest(new { message = "Ids cannot be empty." });
        }

        await _newsRepository.RemoveRangeAsync(ids);
        return NoContent();
    }
}