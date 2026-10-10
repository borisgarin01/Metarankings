using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.OutputCaching;

namespace API.Caching;

/// <summary>
/// После успешного изменяющего запроса сбрасывает теги output-кэша эндпоинта:
/// теги из [OutputCache] и из <see cref="InvalidatesCacheAttribute"/>.
/// </summary>
public sealed class OutputCacheInvalidationFilter : IAsyncResultFilter
{
    private readonly IOutputCacheStore _outputCacheStore;

    public OutputCacheInvalidationFilter(IOutputCacheStore outputCacheStore)
    {
        _outputCacheStore = outputCacheStore;
    }

    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        await next();

        HttpContext httpContext = context.HttpContext;
        string method = httpContext.Request.Method;
        if (HttpMethods.IsGet(method) || HttpMethods.IsHead(method) || HttpMethods.IsOptions(method))
            return;

        if (httpContext.Response.StatusCode >= StatusCodes.Status400BadRequest)
            return;

        IEnumerable<string> tags = context.ActionDescriptor.EndpointMetadata
            .OfType<OutputCacheAttribute>()
            .SelectMany(attribute => attribute.Tags ?? Array.Empty<string>())
            .Concat(context.ActionDescriptor.EndpointMetadata
                .OfType<InvalidatesCacheAttribute>()
                .SelectMany(attribute => attribute.Tags))
            .Distinct();

        foreach (string tag in tags)
            await _outputCacheStore.EvictByTagAsync(tag, CancellationToken.None);
    }
}
