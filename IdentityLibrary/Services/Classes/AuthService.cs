using IdentityLibrary.DTOs;
using IdentityLibrary.Models;
using IdentityLibrary.Repositories.Tokens.RefreshTokens.Interfaces;
using IdentityLibrary.Services.Interfaces;

namespace IdentityLibrary.Services.Classes
{
    public class AuthService : IAuthService
    {
        private readonly IRefreshTokensRepository _refreshTokensRepo;
        private readonly AuthTokenGenerator _tokenGenerator;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthService(
            IRefreshTokensRepository refreshTokensRepo,
            AuthTokenGenerator tokenGenerator,
            UserManager<ApplicationUser> userManager)
        {
            _refreshTokensRepo = refreshTokensRepo;
            _tokenGenerator = tokenGenerator;
            _userManager = userManager;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginModel loginModel)
        {
            // 1. Находим пользователя
            var user = await _userManager.FindByEmailAsync(loginModel.UserEmail);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginModel.Password))
            {
                return new AuthResponseDto(false, false, "Invalid credentials", string.Empty, string.Empty);
            }

            // 2. Проверяем 2FA (если нужно)
            if (user.TwoFactorEnabled)
            {
                // Твоя логика отправки кода
                return new AuthResponseDto(false, true, "Two-factor authentication required", string.Empty, string.Empty);
            }

            // 3. Отзываем старые токены
            await _refreshTokensRepo.RevokeAllByUserIdAsync(Convert.ToInt64(user.Id));

            // 4. Генерируем токены
            var accessToken = await _tokenGenerator.GenerateAccessToken(user);
            var refreshTokenValue = _tokenGenerator.GenerateRefreshToken();

            // 5. Сохраняем refresh токен
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
