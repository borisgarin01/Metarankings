using Microsoft.Extensions.Caching.Distributed;

namespace API.Controllers;

/// <summary>
/// Проверка работы распределённого кэша (Redis).
/// Случайное число кэшируется на 30 секунд: повторные запросы в течение этого времени возвращают то же значение.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public sealed class CacheTestController : ControllerBase
{
    private const string CacheKey = "cache-test:random";

    private static readonly DistributedCacheEntryOptions CacheEntryOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
    };

    private readonly IDistributedCache _cache;

    public CacheTestController(IDistributedCache cache)
    {
        _cache = cache;
    }

    // GET: api/CacheTest
    [HttpGet]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
    {
        string cached = await _cache.GetStringAsync(CacheKey, cancellationToken);
        if (cached is not null)
            return Ok(new { value = int.Parse(cached, CultureInfo.InvariantCulture), fromCache = true, cache = _cache.GetType().Name });

        int value = Random.Shared.Next();
        await _cache.SetStringAsync(CacheKey, value.ToString(CultureInfo.InvariantCulture), CacheEntryOptions, cancellationToken);

        return Ok(new { value, fromCache = false, cache = _cache.GetType().Name });
    }
}
