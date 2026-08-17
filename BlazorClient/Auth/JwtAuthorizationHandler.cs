using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Net;
using System.Threading;

namespace BlazorClient.Auth;

public class JwtAuthorizationHandler : DelegatingHandler
{
    private readonly ILocalStorageService _localStorage;
    private readonly IAuthService _authService;
    private readonly ILogger<JwtAuthorizationHandler> _logger;

    public JwtAuthorizationHandler(
        ILocalStorageService localStorage,
        IAuthService authService,
        ILogger<JwtAuthorizationHandler> logger)
    {
        _localStorage = localStorage;
        _authService = authService;
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        string? path = request.RequestUri?.AbsolutePath;

        // КРИТИЧНО: Пропускаем refresh-token, чтобы избежать бесконечного цикла
        bool isRefreshToken = path?.Contains("/api/auth/refresh-token") == true;

        if (!isRefreshToken)
        {
            string? accessToken = await _localStorage.GetItemAsync<string>("accessToken", cancellationToken);

            if (!string.IsNullOrEmpty(accessToken))
            {
                // Проверяем, не истек ли токен
                if (IsTokenExpired(accessToken))
                {
                    _logger.LogInformation("Token expired for request: {Url}, attempting to refresh", request.RequestUri);

                    var refreshResult = await _authService.RefreshTokenAsync();

                    if (refreshResult != null && refreshResult.IsAuthSuccessful)
                    {
                        accessToken = refreshResult.AccessToken;
                        _logger.LogInformation("Token refreshed successfully before request");
                    }
                    else
                    {
                        _logger.LogWarning("Failed to refresh token before request");
                        // Отправляем запрос без токена, сервер вернет 401
                        request.Headers.Authorization = null;
                        return await base.SendAsync(request, cancellationToken);
                    }
                }

                if (!string.IsNullOrEmpty(accessToken))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                }
            }
        }
        else
        {
            _logger.LogDebug("Skipping auth for refresh-token endpoint");
        }

        HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

        // Обрабатываем 401 только если это НЕ refresh-token запрос
        if (response.StatusCode == HttpStatusCode.Unauthorized && !isRefreshToken)
        {
            _logger.LogWarning("Received 401 for request: {Url}", request.RequestUri);

            // Проверяем, не пытались ли мы уже обновить токен для этого запроса
            if (!request.Properties.ContainsKey("TokenRefreshed"))
            {
                request.Properties["TokenRefreshed"] = true;

                _logger.LogInformation("Attempting to refresh token after 401");
                var refreshResult = await _authService.RefreshTokenAsync();

                if (refreshResult != null && refreshResult.IsAuthSuccessful)
                {
                    _logger.LogInformation("Token refreshed after 401, retrying request");

                    // Обновляем заголовок с новым токеном
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", refreshResult.AccessToken);

                    // Создаем клон запроса для повторной отправки
                    var retryRequest = await CloneHttpRequestMessageAsync(request);

                    // Повторяем запрос
                    return await base.SendAsync(retryRequest, cancellationToken);
                }
                else
                {
                    _logger.LogWarning("Failed to refresh token after 401");
                    // Вызываем logout, чтобы очистить состояние
                    await _authService.LogoutAsync();
                }
            }
            else
            {
                _logger.LogWarning("Token already refreshed for this request, not retrying again");
            }
        }

        return response;
    }

    private bool IsTokenExpired(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            // Добавляем небольшой запас в 1 минуту
            bool isExpired = jwtToken.ValidTo <= DateTime.UtcNow.AddMinutes(-1);

            if (isExpired)
            {
                _logger.LogDebug("Token expired at: {Expiry}, Current UTC: {Now}",
                    jwtToken.ValidTo, DateTime.UtcNow);
            }

            return isExpired;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking token expiration");
            return true; // В случае ошибки считаем токен истекшим
        }
    }

    private async Task<HttpRequestMessage> CloneHttpRequestMessageAsync(HttpRequestMessage original)
    {
        var clone = new HttpRequestMessage(original.Method, original.RequestUri);

        // Копируем заголовки
        foreach (var header in original.Headers)
        {
            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        // Копируем свойства
        foreach (var prop in original.Properties)
        {
            clone.Properties[prop.Key] = prop.Value;
        }

        // Копируем тело запроса
        if (original.Content != null)
        {
            // Сохраняем позицию, чтобы не нарушить оригинальный поток
            var contentStream = await original.Content.ReadAsStreamAsync();

            if (contentStream.CanSeek)
            {
                contentStream.Position = 0;
            }

            var memoryStream = new MemoryStream();
            await contentStream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            clone.Content = new StreamContent(memoryStream);

            // Копируем заголовки контента
            foreach (var header in original.Content.Headers)
            {
                clone.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
        }

        return clone;
    }
}