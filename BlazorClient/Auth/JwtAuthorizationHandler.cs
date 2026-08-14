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
        string accessToken = await _localStorage.GetItemAsync<string>("accessToken");

        if (!string.IsNullOrEmpty(accessToken))
        {
            // Проверяем, не истек ли токен
            if (IsTokenExpired(accessToken))
            {
                _logger.LogInformation("Token expired, attempting to refresh before request: {Url}", request.RequestUri);

                var refreshResult = await _authService.RefreshTokenAsync();

                if (refreshResult != null && refreshResult.IsAuthSuccessful)
                {
                    accessToken = refreshResult.AccessToken;
                    _logger.LogInformation("Token refreshed successfully");
                }
                else
                {
                    _logger.LogWarning("Failed to refresh token");
                    // Убираем токен, чтобы запрос не был отправлен с истекшим токеном
                    request.Headers.Authorization = null;
                    return await base.SendAsync(request, cancellationToken);
                }
            }

            if (!string.IsNullOrEmpty(accessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
        }

        HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

        // Если сервер вернул 401, пробуем обновить токен и повторить запрос
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            _logger.LogWarning("Received 401 for request: {Url}", request.RequestUri);

            // Отключаем автоматическую обработку повторных попыток, чтобы избежать бесконечного цикла
            // Используем флаг, чтобы не повторять запрос больше одного раза
            if (!request.Properties.ContainsKey("TokenRefreshed"))
            {
                request.Properties["TokenRefreshed"] = true;

                var refreshResult = await _authService.RefreshTokenAsync();

                if (refreshResult != null && refreshResult.IsAuthSuccessful)
                {
                    _logger.LogInformation("Token refreshed after 401, retrying request");

                    // Обновляем заголовок с новым токеном
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", refreshResult.AccessToken);

                    // Создаем новый запрос с тем же содержимым
                    var retryRequest = await CloneHttpRequestMessageAsync(request);

                    // Повторяем запрос
                    return await base.SendAsync(retryRequest, cancellationToken);
                }
                else
                {
                    _logger.LogWarning("Failed to refresh token after 401");
                }
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
            return jwtToken.ValidTo <= DateTime.UtcNow.AddMinutes(-1);
        }
        catch
        {
            return true;
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

        // Копируем тело запроса
        if (original.Content != null)
        {
            var contentStream = await original.Content.ReadAsStreamAsync();
            var memoryStream = new MemoryStream();
            await contentStream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            clone.Content = new StreamContent(memoryStream);
            foreach (var header in original.Content.Headers)
            {
                clone.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
        }

        // Копируем свойства
        foreach (var prop in original.Properties)
        {
            clone.Properties[prop.Key] = prop.Value;
        }

        return clone;
    }
}