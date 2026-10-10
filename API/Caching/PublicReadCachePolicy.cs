using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Net.Http.Headers;

namespace API.Caching;

/// <summary>
/// Кэширует успешные GET/HEAD-ответы, в том числе для запросов с Bearer-токеном:
/// Blazor-клиент шлёт токен во всех запросах, а стандартная политика такие запросы не кэширует.
/// Применять только к эндпоинтам, ответ которых не зависит от текущего пользователя.
/// Авторизация эндпоинта проверяется до output-кэша (UseOutputCache стоит после UseAuthorization).
/// </summary>
public sealed class PublicReadCachePolicy : IOutputCachePolicy
{
    private readonly TimeSpan _expiration;

    public PublicReadCachePolicy(TimeSpan expiration)
    {
        _expiration = expiration;
    }

    public ValueTask CacheRequestAsync(OutputCacheContext context, CancellationToken cancellationToken)
    {
        HttpRequest request = context.HttpContext.Request;
        bool isReadRequest = HttpMethods.IsGet(request.Method) || HttpMethods.IsHead(request.Method);

        context.EnableOutputCaching = true;
        context.AllowCacheLookup = isReadRequest;
        context.AllowCacheStorage = isReadRequest;
        context.AllowLocking = true;
        context.ResponseExpirationTimeSpan = _expiration;

        context.CacheVaryByRules.QueryKeys = "*";
        // Сжатие и CORS-заголовки зависят от этих заголовков запроса
        context.CacheVaryByRules.HeaderNames = new[] { HeaderNames.AcceptEncoding, HeaderNames.Origin };

        return ValueTask.CompletedTask;
    }

    public ValueTask ServeFromCacheAsync(OutputCacheContext context, CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask ServeResponseAsync(OutputCacheContext context, CancellationToken cancellationToken)
    {
        HttpResponse response = context.HttpContext.Response;

        if (response.StatusCode != StatusCodes.Status200OK || response.Headers.SetCookie.Count > 0)
            context.AllowCacheStorage = false;

        return ValueTask.CompletedTask;
    }
}
