using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Net;
using System.Threading;

namespace BlazorClient.Auth;

public class JwtAuthorizationHandler : DelegatingHandler
{
    private readonly IAuthService _authService;
    private readonly ILogger<JwtAuthorizationHandler> _logger;
    private readonly SemaphoreSlim _refreshLock = new(1, 1);
    private bool _isRefreshing;

    public JwtAuthorizationHandler(
        IAuthService authService,
        ILogger<JwtAuthorizationHandler> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var path = request.RequestUri?.AbsolutePath;

        // Пропускаем refresh-token, login и register
        bool isRefreshToken = path?.Contains("/api/auth/refresh-token") == true;
        bool isLogin = path?.Contains("/api/auth/login") == true;
        bool isRegister = path?.Contains("/api/auth/register") == true;

        if (isRefreshToken || isLogin || isRegister)
        {
            return await base.SendAsync(request, cancellationToken);
        }

        // Получаем токен через сервис
        var accessToken = await _authService.GetCurrentAccessTokenAsync();

        if (!string.IsNullOrEmpty(accessToken))
        {
            if (IsTokenExpired(accessToken))
            {
                _logger.LogInformation("Token expired for {Url}, refreshing...", request.RequestUri);

                var newToken = await RefreshTokenAsync(cancellationToken);
                if (!string.IsNullOrEmpty(newToken))
                {
                    accessToken = newToken;
                }
                else
                {
                    request.Headers.Authorization = null;
                    var unauthorizedResponse = await base.SendAsync(request, cancellationToken);

                    if (unauthorizedResponse.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        await _authService.LogoutAsync();
                    }

                    return unauthorizedResponse;
                }
            }

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        // Отправляем запрос
        var response = await base.SendAsync(request, cancellationToken);

        // Если получили 401 и еще не пробовали обновить для этого запроса
        if (response.StatusCode == HttpStatusCode.Unauthorized &&
            !request.Properties.ContainsKey("TokenRefreshed") &&
            !string.IsNullOrEmpty(accessToken))
        {
            request.Properties["TokenRefreshed"] = true;

            _logger.LogWarning("Received 401 for {Url}, retrying with new token", request.RequestUri);

            var newToken = await RefreshTokenAsync(cancellationToken);
            if (!string.IsNullOrEmpty(newToken))
            {
                // ИСПРАВЛЕНО: создаем НОВЫЙ запрос для повторной отправки
                var retryRequest = await CreateRetryRequestAsync(request, newToken);

                // Убеждаемся, что тело запроса доступно для повторного чтения
                var retryResponse = await base.SendAsync(retryRequest, cancellationToken);
                return retryResponse;
            }
            else
            {
                await _authService.LogoutAsync();
                return response;
            }
        }

        return response;
    }

    // ИСПРАВЛЕНО: создание запроса для повторной отправки с правильным клонированием тела
    private async Task<HttpRequestMessage> CreateRetryRequestAsync(HttpRequestMessage original, string newToken)
    {
        var retryRequest = new HttpRequestMessage(original.Method, original.RequestUri);

        // Копируем заголовки (кроме Authorization)
        foreach (var header in original.Headers)
        {
            if (header.Key != "Authorization")
            {
                retryRequest.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
        }

        // Добавляем новый токен
        retryRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", newToken);

        // Копируем свойства
        foreach (var prop in original.Properties)
        {
            retryRequest.Properties[prop.Key] = prop.Value;
        }

        // ИСПРАВЛЕНО: корректное копирование тела запроса
        if (original.Content != null)
        {
            // Сохраняем позицию потока, если это возможно
            var originalStream = await original.Content.ReadAsStreamAsync();

            // Создаем новый MemoryStream для копирования
            var memoryStream = new MemoryStream();

            // Копируем содержимое
            await originalStream.CopyToAsync(memoryStream);

            // Сбрасываем позицию для чтения
            memoryStream.Position = 0;

            // Создаем новый контент
            retryRequest.Content = new StreamContent(memoryStream);

            // Копируем заголовки контента
            foreach (var header in original.Content.Headers)
            {
                retryRequest.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
        }

        return retryRequest;
    }

    private async Task<string?> RefreshTokenAsync(CancellationToken cancellationToken)
    {
        await _refreshLock.WaitAsync(cancellationToken);

        try
        {
            if (_isRefreshing)
            {
                _logger.LogWarning("Refresh already in progress, waiting...");
                while (_isRefreshing)
                {
                    await Task.Delay(100, cancellationToken);
                }

                return await _authService.GetCurrentAccessTokenAsync();
            }

            _isRefreshing = true;

            var result = await _authService.RefreshTokenAsync();

            if (result != null && result.IsAuthSuccessful && !string.IsNullOrEmpty(result.AccessToken))
            {
                return result.AccessToken;
            }

            return null;
        }
        finally
        {
            _isRefreshing = false;
            _refreshLock.Release();
        }
    }

    private bool IsTokenExpired(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            return jwtToken.ValidTo <= DateTime.UtcNow.AddMinutes(-1);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking token expiration");
            return true;
        }
    }
}