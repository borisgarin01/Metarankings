using Microsoft.AspNetCore.OutputCaching;

namespace API.Caching;

/// <summary>
/// Обёртка над хранилищем output-кэша: при недоступности Redis запросы обслуживаются без кэша, а не падают с 500.
/// </summary>
public sealed class ResilientOutputCacheStore : IOutputCacheStore, IDisposable
{
    private readonly IOutputCacheStore _inner;
    private readonly ILogger<ResilientOutputCacheStore> _logger;

    public ResilientOutputCacheStore(IOutputCacheStore inner, ILogger<ResilientOutputCacheStore> logger)
    {
        _inner = inner;
        _logger = logger;
    }

    public async ValueTask EvictByTagAsync(string tag, CancellationToken cancellationToken)
    {
        try
        {
            await _inner.EvictByTagAsync(tag, cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Output cache: failed to evict tag {Tag}", tag);
        }
    }

    public async ValueTask<byte[]?> GetAsync(string key, CancellationToken cancellationToken)
    {
        try
        {
            return await _inner.GetAsync(key, cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogWarning(ex, "Output cache: failed to read key {Key}", key);
            return null;
        }
    }

    public async ValueTask SetAsync(string key, byte[] value, string[]? tags, TimeSpan validFor, CancellationToken cancellationToken)
    {
        try
        {
            await _inner.SetAsync(key, value, tags, validFor, cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogWarning(ex, "Output cache: failed to write key {Key}", key);
        }
    }

    public void Dispose()
    {
        (_inner as IDisposable)?.Dispose();
    }
}
