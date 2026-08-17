using IdentityLibrary.DTOs;
using IdentityLibrary.Models;
using IdentityLibrary.Repositories.Tokens.RefreshTokens.Interfaces;
using IdentityLibrary.Services.Interfaces;
using Microsoft.Extensions.Options;
using Settings;

namespace IdentityLibrary.Services.Classes
{
    public class AuthService : IAuthService
    {
        private readonly IRefreshTokensRepository _refreshTokensRepo;
        private readonly AuthTokenGenerator _tokenGenerator;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOptionsMonitor<AuthSettings> _authSettings;

        public AuthService(
            IRefreshTokensRepository refreshTokensRepo,
            AuthTokenGenerator tokenGenerator,
            UserManager<ApplicationUser> userManager,
            IOptionsMonitor<AuthSettings> authSettings)
        {
            _refreshTokensRepo = refreshTokensRepo;
            _tokenGenerator = tokenGenerator;
            _userManager = userManager;
            _authSettings = authSettings;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginModel loginModel)
        {
            var user = await _userManager.FindByEmailAsync(loginModel.UserEmail);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginModel.Password))
            {
                return new AuthResponseDto(false, false, "Invalid credentials", string.Empty, string.Empty);
            }

            if (user.TwoFactorEnabled)
            {
                return new AuthResponseDto(false, true, "Two-factor authentication required", string.Empty, string.Empty);
            }

            // ✅ Отзываем все старые токены
            await _refreshTokensRepo.RevokeAllByUserIdAsync(Convert.ToInt64(user.Id));

            var accessToken = await _tokenGenerator.GenerateAccessToken(user);
            var refreshTokenValue = _tokenGenerator.GenerateRefreshToken();

            var refreshToken = new RefreshToken(0, Convert.ToInt64(user.Id), refreshTokenValue, false, DateTime.UtcNow);
            await _refreshTokensRepo.CreateAsync(refreshToken);

            return new AuthResponseDto(true, false, string.Empty, accessToken, refreshTokenValue);
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequest request)
        {
            // 1. Проверяем refresh токен в БД
            var storedToken = await _refreshTokensRepo.GetByValueAsync(request.RefreshToken);
            if (storedToken == null)
            {
                return new AuthResponseDto(false, false, "Invalid refresh token", string.Empty, string.Empty);
            }

            // ✅ ПРОВЕРЯЕМ СРОК ГОДНОСТИ!
            var refreshTokenLifetime = _authSettings.CurrentValue.RefreshTokenLifetimeDays;
            if (storedToken.CreatedAt.AddDays(refreshTokenLifetime) < DateTime.UtcNow)
            {
                // Отзываем истекший токен
                await _refreshTokensRepo.RevokeAsync(storedToken.Id);
                return new AuthResponseDto(false, false, "Refresh token expired", string.Empty, string.Empty);
            }

            // ✅ ПРОВЕРЯЕМ, НЕ ОТОЗВАН ЛИ ТОКЕН
            if (storedToken.IsRevoked)
            {
                return new AuthResponseDto(false, false, "Refresh token is revoked", string.Empty, string.Empty);
            }

            // 2. Получаем пользователя
            var user = await _userManager.FindByIdAsync(storedToken.UserId.ToString());
            if (user == null)
            {
                return new AuthResponseDto(false, false, "User not found", string.Empty, string.Empty);
            }

            // 3. Отзываем старый токен (ротация)
            await _refreshTokensRepo.RevokeAsync(storedToken.Id);

            // 4. Генерируем новые токены
            var newAccessToken = await _tokenGenerator.GenerateAccessToken(user);
            var newRefreshToken = _tokenGenerator.GenerateRefreshToken();

            // 5. Сохраняем новый refresh токен
            var newToken = new RefreshToken(0, Convert.ToInt64(user.Id), newRefreshToken, false, DateTime.UtcNow);
            await _refreshTokensRepo.CreateAsync(newToken);

            return new AuthResponseDto(true, false, string.Empty, newAccessToken, newRefreshToken);
        }

        public async Task LogoutAsync(string refreshToken)
        {
            var token = await _refreshTokensRepo.GetByValueAsync(refreshToken);
            if (token != null)
            {
                await _refreshTokensRepo.RevokeAsync(token.Id);
            }
        }

        public async Task LogoutAllAsync(long userId)
        {
            await _refreshTokensRepo.RevokeAllByUserIdAsync(userId);
        }
    }
}