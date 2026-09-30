using Data.Repositories.Interfaces;
using Domain.Common.News;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class NewsController : ControllerBase
{
    private readonly IRepository<NewsItem, AddNewsItemModel, UpdateNewsItemModel> _newsRepository;

    public NewsController(
        IRepository<NewsItem, AddNewsItemModel, UpdateNewsItemModel> newsRepository)
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
    public async Task<ActionResult<NewsItem>> Create(AddNewsItemModel addNewsItemModel)
    {
        long newId;
        try
        {
            newId = await _newsRepository.AddAsync(addNewsItemModel);
        }
        catch (Npgsql.PostgresException ex) when (ex.SqlState == "23505")
        {
            // unique_violation по Title
            return Conflict(new { message = "News with the same title already exists." });
        }
        catch (Npgsql.PostgresException ex) when (ex.SqlState == "23503")
        {
            // foreign_key_violation по UserId
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
        NewsItem updated;
        try
        {
            updated = await _newsRepository.UpdateAsync(request, id);
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