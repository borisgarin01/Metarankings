using API.Auth;
using Domain.Auth;
using IdentityLibrary.DTOs;
using IdentityLibrary.Models;
using IdentityLibrary.Repositories.Tokens.RefreshTokens.Interfaces;
using IdentityLibrary.Services.Classes;
using IdentityLibrary.Services.Interfaces;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MimeKit;
using Settings;
using System.Net;
using System.Net.Http;

namespace API.Controllers.Auth;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<ApplicationUser> _usersManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly AuthTokenGenerator _authTokenGenerator;
    private readonly TwoFactorAuthEmailProcessor _twoFactorAuthEmailProcessor;
    private readonly ITokensService _tokenService;
    private readonly IRefreshTokensRepository _refreshTokensRepo;

    private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
    private readonly ILogger<AuthController> _logger;
    private readonly IOptionsMonitor<AuthSettings> _authSettingsOptionsMonitor;
    private readonly IOptionsMonitor<TokenValidationParameters> _tokenValidationParameters;
    private readonly IOptionsMonitor<EmailSettings> _emailSettings;

    public AuthController(
        IConfiguration configuration,
        UserManager<ApplicationUser> usersManager,
        IPasswordHasher<ApplicationUser> passwordHasher,
        ILogger<AuthController> logger,
        IOptionsMonitor<AuthSettings> authSettingsOptionsMonitor,
        IOptionsMonitor<EmailSettings> emailSettings,
        IOptionsMonitor<TokenValidationParameters> tokenValidationParameters,
        SignInManager<ApplicationUser> signInManager,
        AuthTokenGenerator authTokenGenerator,
        TwoFactorAuthEmailProcessor twoFactorAuthEmailProcessor,
        ITokensService tokenService,
        IRefreshTokensRepository refreshTokensRepo)
    {
        _configuration = configuration;
        _usersManager = usersManager;
        _passwordHasher = passwordHasher;
        _logger = logger;
        _authSettingsOptionsMonitor = authSettingsOptionsMonitor;
        _emailSettings = emailSettings;
        _tokenValidationParameters = tokenValidationParameters;
        _signInManager = signInManager;
        _authTokenGenerator = authTokenGenerator;
        _twoFactorAuthEmailProcessor = twoFactorAuthEmailProcessor;
        _tokenService = tokenService;
        _refreshTokensRepo = refreshTokensRepo;
    }

    // ============================================================
    // HELPERS
    // ============================================================

    private async Task<AuthResponseDto> IssueTokensAsync(ApplicationUser user)
    {
        await _refreshTokensRepo.RevokeAllByUserIdAsync(Convert.ToInt64(user.Id));

        string accessToken = await _authTokenGenerator.GenerateAccessToken(user);
        string refreshTokenValue = _authTokenGenerator.GenerateRefreshToken();

        var refreshToken = new IdentityLibrary.DTOs.RefreshToken(
            0,
            Convert.ToInt64(user.Id),
            refreshTokenValue,
            false,
            DateTime.UtcNow);
        await _refreshTokensRepo.CreateAsync(refreshToken);

        return new AuthResponseDto(true, false, string.Empty, accessToken, refreshTokenValue);
    }

    private string? GetCurrentUserId()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }

    // ============================================================
    // EXTERNAL PROVIDERS
    // ============================================================

    [HttpGet("external-providers")]
    public async Task<ActionResult<IEnumerable<Domain.Auth.AuthenticationScheme>>> GetExternalProviders()
    {
        IEnumerable<Microsoft.AspNetCore.Authentication.AuthenticationScheme> externalProviders =
            await _signInManager.GetExternalAuthenticationSchemesAsync();

        return Ok(externalProviders.Select(ep => new Domain.Auth.AuthenticationScheme(ep.Name, ep.DisplayName)));
    }

    // ============================================================
    // CLASSIC LOGIN / REGISTER
    // ============================================================

    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginModel loginModel)
    {
        if (loginModel is null)
            return BadRequest("Неверный логин");

        if (string.IsNullOrWhiteSpace(loginModel.UserEmail) || string.IsNullOrWhiteSpace(loginModel.Password))
            return BadRequest("Email и пароль должны быть указаны");

        ApplicationUser? user = await _usersManager.FindByEmailAsync(loginModel.UserEmail);
        if (user is null)
            return NotFound("Пользователь не зарегистрирован");

        bool isValidPassword = await _usersManager.CheckPasswordAsync(user, loginModel.Password);
        if (!isValidPassword)
            return BadRequest("Неверный пароль");

        if (user.TwoFactorEnabled)
        {
            string token = await _usersManager.GenerateTwoFactorTokenAsync(user, "Email");
            _logger.LogInformation("2FA code for {Email}: {Code}", user.Email, token);
            return Ok(new AuthResponseDto(false, true, "2FA required", string.Empty, string.Empty));
        }

        AuthResponseDto response = await IssueTokensAsync(user);
        return Ok(response);
    }

    [HttpPost("ConfirmLoginViaEmail")]
    public async Task<ActionResult> ConfirmLoginViaEmail(ConfirmLoginModel model)
    {
        if (string.IsNullOrWhiteSpace(model.UserId) || string.IsNullOrWhiteSpace(model.TwoFactorToken))
            return BadRequest("User ID and token are required");

        ApplicationUser? user = await _usersManager.FindByIdAsync(model.UserId);
        if (user is null)
            return NotFound();

        bool isValidTwoFactorToken = await _usersManager.VerifyTwoFactorTokenAsync(user, "Email", model.TwoFactorToken);
        if (!isValidTwoFactorToken)
            return BadRequest("Invalid 2FA token");

        AuthResponseDto response = await IssueTokensAsync(user);
        return Ok(response);
    }

    [HttpPost("register")]
    public async Task<ActionResult<string>> Register(RegisterModel registerModel)
    {
        ApplicationUser? userToCheckExistance = await _usersManager.FindByEmailAsync(registerModel.UserEmail);
        if (userToCheckExistance is not null)
            return BadRequest($"Пользователь с {registerModel.UserEmail} уже существует");

        IQueryable<ApplicationUser> registeredUsers = _usersManager.Users;
        userToCheckExistance = registeredUsers.FirstOrDefault(b => b.NormalizedUserName == registerModel.UserName.ToUpperInvariant());
        if (userToCheckExistance is not null)
            return BadRequest($"Пользователь с логином {registerModel.UserName} уже существует");

        if (!string.Equals(registerModel.Password, registerModel.PasswordConfirmation))
            return BadRequest("Пароль не совпадает с подтверждением пароля");

        string passwordHash = _passwordHasher.HashPassword(null, registerModel.Password);

        ApplicationUser? user = new()
        {
            Email = registerModel.UserEmail,
            PasswordHash = passwordHash,
            EmailConfirmed = false,
            NormalizedEmail = registerModel.UserEmail.ToUpperInvariant(),
            NormalizedUserName = registerModel.UserName.ToUpperInvariant(),
            PhoneNumber = registerModel.PhoneNumber,
            PhoneNumberConfirmed = false,
            TwoFactorEnabled = false,
            UserName = registerModel.UserName,
            SecurityStamp = DateTime.Now.ToString()
        };

        IdentityResult userCreationResult = await _usersManager.CreateAsync(user);

        if (!userCreationResult.Succeeded)
            return StatusCode(500, "User creation error");

        // Admin role
        if (string.Equals(registerModel.UserEmail, _authSettingsOptionsMonitor.CurrentValue.AdminEmail)
            && string.Equals(registerModel.Password, _authSettingsOptionsMonitor.CurrentValue.AdminPassword))
        {
            ApplicationUser? userToBindToAdminRole = await _usersManager.FindByEmailAsync(user.Email);
            IdentityResult addingToAdminRoleIdentityResult = await _usersManager.AddToRoleAsync(userToBindToAdminRole, "Admin");
            if (!addingToAdminRoleIdentityResult.Succeeded)
            {
                _logger.LogError(
                    $"Ошибка добавления к роли администратора. {string.Join(", ", addingToAdminRoleIdentityResult.Errors.Select(b => $"{b.Code}, {b.Description}"))}");
            }
        }

        user = await _usersManager.FindByEmailAsync(registerModel.UserEmail);

        string code = WebUtility.UrlEncode(await _usersManager.GenerateEmailConfirmationTokenAsync(user));
        string? callbackUrl = Url.Action("ConfirmEmail", "Auth", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);

        MimeMessage emailMessage = new();
        emailMessage.From.Add(new MailboxAddress(_emailSettings.CurrentValue.Sender.Name, _emailSettings.CurrentValue.Sender.Email));
        emailMessage.To.Add(new MailboxAddress("", user.Email));
        emailMessage.Subject = "Confirm email";
        emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
        {
            Text = $"Confirm email: go to email confirmation <a href=\"{callbackUrl}\">link</a> to confirm your email"
        };

        using (SmtpClient client = new())
        {
            await client.ConnectAsync(_emailSettings.CurrentValue.Host, _emailSettings.CurrentValue.Port, _emailSettings.CurrentValue.UseSsl);
            await client.AuthenticateAsync(_emailSettings.CurrentValue.UserName, _emailSettings.CurrentValue.Password);
            _ = await client.SendAsync(emailMessage);
            await client.DisconnectAsync(true);
        }

        return Ok("Email verification has been set");
    }

    [HttpGet("ConfirmEmail")]
    public async Task<IActionResult> ConfirmEmail(string userId, string code)
    {
        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(code))
            return BadRequest("UserId and code are required");

        code = WebUtility.UrlDecode(code);

        ApplicationUser? user = await _usersManager.FindByIdAsync(userId);
        if (user is null)
            return NotFound("User not found");

        IdentityResult emailConfirmationResult = await _usersManager.ConfirmEmailAsync(user, code);
        if (!emailConfirmationResult.Succeeded)
            return StatusCode(StatusCodes.Status400BadRequest, userId);

        return Ok($"Email {user.Email} подтверждён.");
    }

    [HttpPost("logout")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult> Logout()
    {
        try
        {
            string? userId = GetCurrentUserId();
            if (userId is null)
                return Unauthorized();

            ApplicationUser? user = await _usersManager.FindByIdAsync(userId);
            if (user is null)
                return NotFound();

            await _refreshTokensRepo.RevokeAllByUserIdAsync(Convert.ToInt64(user.Id));
            return Ok("Logged out successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        if (string.IsNullOrEmpty(request.RefreshToken))
            return BadRequest("Refresh token is required");

        try
        {
            IdentityLibrary.DTOs.RefreshToken? storedToken = await _refreshTokensRepo.GetByValueAsync(request.RefreshToken);
            if (storedToken is null)
            {
                _logger.LogWarning("Invalid refresh token");
                return BadRequest("Invalid refresh token");
            }

            var refreshTokenLifetime = _authSettingsOptionsMonitor.CurrentValue.RefreshTokenLifetimeDays;
            if (storedToken.CreatedAt.AddDays(refreshTokenLifetime) < DateTime.UtcNow)
            {
                _logger.LogWarning("Refresh token expired for user {UserId}", storedToken.UserId);
                await _refreshTokensRepo.RevokeAsync(storedToken.Id);
                return BadRequest("Refresh token expired");
            }

            if (storedToken.IsRevoked)
            {
                _logger.LogWarning("Refresh token is revoked for user {UserId}", storedToken.UserId);
                return BadRequest("Refresh token is revoked");
            }

            ApplicationUser? user = await _usersManager.FindByIdAsync(storedToken.UserId.ToString());
            if (user is null)
            {
                _logger.LogWarning("User not found for refresh token");
                return BadRequest("User not found");
            }

            await _refreshTokensRepo.RevokeAsync(storedToken.Id);

            AuthResponseDto response = await IssueTokensAsync(user);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing token");
            return StatusCode(500, "Internal server error during token refresh");
        }
    }

    // ============================================================
    // VK ID — SCENARIO 2: LINK (user must be logged in)
    // ============================================================

    [HttpPost("link-vkid")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public IActionResult LinkVKID()
    {
        string? userId = GetCurrentUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        string state = Guid.NewGuid().ToString("N");
        string codeVerifier = _authTokenGenerator.GenerateCodeVerifier();
        string codeChallenge = _authTokenGenerator.GenerateCodeChallenge(codeVerifier);

        // Сохраняем userId + codeVerifier в HttpOnly cookie
        Response.Cookies.Append(
            $"vkid_link_{state}",
            $"{userId}|{codeVerifier}",
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,  // нужно для cross-site redirect от VK
                Expires = DateTimeOffset.UtcNow.AddMinutes(10),
                Path = "/api/auth"
            });

        string redirectUri = $"{Request.Scheme}://{Request.Host}/api/auth/vkid-link-callback";
        string clientId = _authSettingsOptionsMonitor.CurrentValue.VkId.ClientId;

        string authUrl =
            $"https://id.vk.ru/authorize?response_type=code" +
            $"&client_id={clientId}" +
            $"&scope=vkid.personal_info" +
            $"&redirect_uri={Uri.EscapeDataString(redirectUri)}" +
            $"&state={state}" +
            $"&code_challenge={codeChallenge}" +
            $"&code_challenge_method=S256";

        _logger.LogInformation("VK ID link initiated for user {UserId}", userId);

        return Ok(new { url = authUrl });
    }

    // ============================================================
    // VK ID — SCENARIO 3: LOGIN
    // ============================================================

    [HttpGet("login-vkid")]
    public IActionResult LoginVKID()
    {
        try
        {
            AuthenticationProperties properties = _signInManager
                .ConfigureExternalAuthenticationProperties(
                    "VK ID",
                    Url.Action(nameof(VKIDCallback), "Auth"));

            properties.Items["flow"] = "login";

            _logger.LogInformation("VK ID login initiated");
            return Challenge(properties, "VK ID");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при попытке входа через VK ID");
            return StatusCode(500, $"Ошибка: {ex.Message}");
        }
    }

    // ============================================================
    // VK ID — COMMON CALLBACK
    // ============================================================

    [HttpGet("vkid-callback")]
    public async Task<ActionResult> VKIDCallback()
    {
        try
        {
            _logger.LogInformation("vkid-callback");

            AuthenticateResult result = await HttpContext.AuthenticateAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            if (!result.Succeeded || result.Principal is null)
            {
                _logger.LogWarning("VK ID authentication failed");
                return Redirect($"{Request.Scheme}://{Request.Host}/login?error=vkid_auth_failed");
            }

            foreach (Claim claim in result.Principal.Claims)
                _logger.LogInformation("Claim: {Type} = {Value}", claim.Type, claim.Value);

            string flow = result.Properties?.Items["flow"] ?? "login";
            _logger.LogInformation("VK ID flow: {Flow}", flow);

            string? vkUserId = result.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(vkUserId))
            {
                _logger.LogError("VK UserId is null");
                return Redirect($"{Request.Scheme}://{Request.Host}/login?error=missing_vk_id");
            }

            string? givenName = result.Principal.FindFirst(ClaimTypes.GivenName)?.Value;
            string? surname = result.Principal.FindFirst(ClaimTypes.Surname)?.Value;
            string fullName = $"{givenName} {surname}".Trim();

            // ============================================================
            // SCENARIO 2: LINK VK TO CURRENT USER
            // ============================================================
            if (flow == "link")
            {
                string? linkUserId = result.Properties?.Items["linkUserId"];
                if (string.IsNullOrEmpty(linkUserId))
                {
                    _logger.LogError("linkUserId not found in properties");
                    return Redirect($"{Request.Scheme}://{Request.Host}/profile?error=missing_user_id");
                }

                ApplicationUser? currentUser = await _usersManager.FindByIdAsync(linkUserId);
                if (currentUser is null)
                {
                    _logger.LogError("User {UserId} not found for linking", linkUserId);
                    return Redirect($"{Request.Scheme}://{Request.Host}/profile?error=user_not_found");
                }

                ApplicationUser? existingVkUser = await _usersManager.FindByLoginAsync("VK ID", vkUserId);
                if (existingVkUser is not null && existingVkUser.Id != currentUser.Id)
                {
                    _logger.LogWarning(
                        "VK ID {VkUserId} already linked to user {ExistingUserId}",
                        vkUserId, existingVkUser.Id);
                    return Redirect($"{Request.Scheme}://{Request.Host}/profile?error=vkid_already_linked");
                }

                IdentityResult addLoginResult = await _usersManager.AddLoginAsync(
                    currentUser,
                    new UserLoginInfo("VK ID", vkUserId, "VK ID"));

                if (!addLoginResult.Succeeded)
                {
                    var errors = string.Join(", ", addLoginResult.Errors.Select(e => e.Description));
                    _logger.LogError("Failed to link VK ID: {Errors}", errors);
                    return Redirect($"{Request.Scheme}://{Request.Host}/profile?error={WebUtility.UrlEncode(errors)}");
                }

                _logger.LogInformation("VK ID {VkUserId} linked to user {UserId}", vkUserId, currentUser.Id);
                return Redirect($"{Request.Scheme}://{Request.Host}/profile?success=vkid_linked");
            }

            // ============================================================
            // SCENARIO 1 & 3: LOGIN
            // ============================================================
            ApplicationUser? user = await _usersManager.FindByLoginAsync("VK ID", vkUserId);

            // SCENARIO 1: VK ID not linked to any account
            if (user is null)
            {
                _logger.LogWarning("VK ID {VkUserId} is not linked to any account", vkUserId);
                return Redirect($"{Request.Scheme}://{Request.Host}/login?error=vkid_not_linked");
            }

            // SCENARIO 3: VK ID linked — login
            AuthResponseDto response = await IssueTokensAsync(user);

            return Redirect(
                $"{Request.Scheme}://{Request.Host}/auth/vkid-callback" +
                $"?Token={Uri.EscapeDataString(response.AccessToken)}" +
                $"&RefreshToken={Uri.EscapeDataString(response.RefreshToken ?? "")}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in VK ID callback");
            return Redirect($"{Request.Scheme}://{Request.Host}/login?error={WebUtility.UrlEncode(ex.Message)}");
        }
    }
    [HttpGet("vkid-link-callback")]
    public async Task<ActionResult> VKIDLinkCallback(
    [FromQuery] string code,
    [FromQuery] string state,
    [FromQuery(Name = "device_id")] string deviceId)
    {
        try
        {
            // 1. Валидация входных параметров
            if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(state))
                return Redirect($"{Request.Scheme}://{Request.Host}/auth/account?error=missing_params");

            if (string.IsNullOrWhiteSpace(deviceId))
            {
                _logger.LogWarning("device_id is missing");
                return Redirect($"{Request.Scheme}://{Request.Host}/auth/account?error=missing_device_id");
            }

            // 2. Читаем cookie с userId и codeVerifier
            if (!Request.Cookies.TryGetValue($"vkid_link_{state}", out string? cookieValue) ||
                string.IsNullOrWhiteSpace(cookieValue))
            {
                _logger.LogWarning("Invalid or expired state: {State}", state);
                return Redirect($"{Request.Scheme}://{Request.Host}/auth/account?error=invalid_state");
            }

            // 3. Удаляем cookie — она одноразовая
            Response.Cookies.Delete($"vkid_link_{state}", new CookieOptions
            {
                Path = "/",
                Secure = true,
                SameSite = SameSiteMode.None
            });

            // 4. Парсим cookie
            string[] parts = cookieValue.Split('|');
            if (parts.Length != 2)
            {
                _logger.LogError("Invalid cookie format for state {State}", state);
                return Redirect($"{Request.Scheme}://{Request.Host}/auth/account?error=invalid_state");
            }

            string userId = parts[0];
            string codeVerifier = parts[1];

            // 5. Находим пользователя
            ApplicationUser? currentUser = await _usersManager.FindByIdAsync(userId);
            if (currentUser is null)
            {
                _logger.LogError("User {UserId} not found", userId);
                return Redirect($"{Request.Scheme}://{Request.Host}/auth/account?error=user_not_found");
            }

            // 6. Обмениваем code на user_id VK
            string? vkUserId = await _authTokenGenerator.ExchangeVkCodeForUserIdAsync(code, codeVerifier, Request.Scheme, Request.Host.Host, deviceId);
            if (string.IsNullOrEmpty(vkUserId))
            {
                _logger.LogError("VK code exchange failed");
                return Redirect($"{Request.Scheme}://{Request.Host}/auth/account?error=vk_exchange_failed");
            }

            // 7. Проверяем, не привязан ли VK ID к другому аккаунту
            ApplicationUser? existingVkUser = await _usersManager.FindByLoginAsync("VK ID", vkUserId);
            if (existingVkUser is not null && existingVkUser.Id != currentUser.Id)
            {
                _logger.LogWarning(
                    "VK ID {VkUserId} already linked to user {ExistingUserId}",
                    vkUserId, existingVkUser.Id);
                return Redirect($"{Request.Scheme}://{Request.Host}/auth/account?error=vkid_already_linked");
            }

            // 8. Привязываем VK ID к текущему пользователю
            IdentityResult addLoginResult = await _usersManager.AddLoginAsync(
                currentUser,
                new UserLoginInfo("VK ID", vkUserId, "VK ID"));

            if (!addLoginResult.Succeeded)
            {
                var errors = string.Join(", ", addLoginResult.Errors.Select(e => e.Description));
                _logger.LogError("Failed to link VK ID: {Errors}", errors);
                return Redirect($"{Request.Scheme}://{Request.Host}/auth/account?error={WebUtility.UrlEncode(errors)}");
            }

            // 9. Успех
            _logger.LogInformation("VK ID {VkUserId} linked to user {UserId}", vkUserId, currentUser.Id);
            return Redirect($"{Request.Scheme}://{Request.Host}/auth/account?success=vkid_linked");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in VK ID link callback");
            return Redirect($"{Request.Scheme}://{Request.Host}/auth/account?error={WebUtility.UrlEncode(ex.Message)}");
        }
    }

    // ============================================================
    // Yandex — SCENARIO 3: LOGIN
    // ============================================================

    [HttpGet("login-yandex")]
    public IActionResult LoginYandexID()
    {
        try
        {
            AuthenticationProperties properties = _signInManager
                .ConfigureExternalAuthenticationProperties(
                    "Yandex",
                    Url.Action(nameof(YandexIDCallback), "Auth"));

            properties.Items["flow"] = "login";

            _logger.LogInformation("Yandex login initiated");
            return Challenge(properties, "Yandex");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при попытке входа через Yandex");
            return StatusCode(500, $"Ошибка: {ex.Message}");
        }
    }

    // ============================================================
    // Yandex — COMMON CALLBACK
    // ============================================================

    [HttpGet("yandexid-callback")]
    public async Task<ActionResult> YandexIDCallback()
    {
        try
        {
            _logger.LogInformation("yandexid-callback");

            AuthenticateResult result = await HttpContext.AuthenticateAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            if (!result.Succeeded || result.Principal is null)
            {
                _logger.LogWarning("Yandex authentication failed");
                return Redirect($"{Request.Scheme}://{Request.Host}/login?error=yandexid_auth_failed");
            }

            foreach (Claim claim in result.Principal.Claims)
                _logger.LogInformation("Claim: {Type} = {Value}", claim.Type, claim.Value);

            string flow = result.Properties?.Items["flow"] ?? "login";
            _logger.LogInformation("Yandex flow: {Flow}", flow);

            string? yandexUserId = result.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(yandexUserId))
            {
                _logger.LogError("Yandex UserId is null");
                return Redirect($"{Request.Scheme}://{Request.Host}/login?error=missing_yandex_id");
            }

            string? givenName = result.Principal.FindFirst(ClaimTypes.GivenName)?.Value;
            string? surname = result.Principal.FindFirst(ClaimTypes.Surname)?.Value;
            string fullName = $"{givenName} {surname}".Trim();

            // ============================================================
            // SCENARIO 2: LINK VK TO CURRENT USER
            // ============================================================
            if (flow == "link")
            {
                string? linkUserId = result.Properties?.Items["linkUserId"];
                if (string.IsNullOrEmpty(linkUserId))
                {
                    _logger.LogError("linkUserId not found in properties");
                    return Redirect($"{Request.Scheme}://{Request.Host}/profile?error=missing_user_id");
                }

                ApplicationUser? currentUser = await _usersManager.FindByIdAsync(linkUserId);
                if (currentUser is null)
                {
                    _logger.LogError("User {UserId} not found for linking", linkUserId);
                    return Redirect($"{Request.Scheme}://{Request.Host}/profile?error=user_not_found");
                }

                ApplicationUser? existingYandexUser = await _usersManager.FindByLoginAsync("Yandex", yandexUserId);
                if (existingYandexUser is not null && existingYandexUser.Id != currentUser.Id)
                {
                    _logger.LogWarning(
                        "Yandex {YandexUserId} already linked to user {ExistingUserId}",
                        yandexUserId, existingYandexUser.Id);
                    return Redirect($"{Request.Scheme}://{Request.Host}/profile?error=yandexid_already_linked");
                }

                IdentityResult addLoginResult = await _usersManager.AddLoginAsync(
                    currentUser,
                    new UserLoginInfo("Yandex", yandexUserId, "Yandex"));

                if (!addLoginResult.Succeeded)
                {
                    var errors = string.Join(", ", addLoginResult.Errors.Select(e => e.Description));
                    _logger.LogError("Failed to link Yandex: {Errors}", errors);
                    return Redirect($"{Request.Scheme}://{Request.Host}/profile?error={WebUtility.UrlEncode(errors)}");
                }

                _logger.LogInformation("Yandex {YandexUserId} linked to user {UserId}", yandexUserId, currentUser.Id);
                return Redirect($"{Request.Scheme}://{Request.Host}/profile?success=yandexid_linked");
            }

            // ============================================================
            // SCENARIO 1 & 3: LOGIN
            // ============================================================
            ApplicationUser? user = await _usersManager.FindByLoginAsync("Yandex", yandexUserId);

            // SCENARIO 1: VK ID not linked to any account
            if (user is null)
            {
                _logger.LogWarning("Yandex {YandexUserId} is not linked to any account", yandexUserId);
                return Redirect($"{Request.Scheme}://{Request.Host}/login?error=yandexid_not_linked");
            }

            // SCENARIO 3: VK ID linked — login
            AuthResponseDto response = await IssueTokensAsync(user);

            return Redirect(
                $"{Request.Scheme}://{Request.Host}/auth/yandexid-callback" +
                $"?Token={Uri.EscapeDataString(response.AccessToken)}" +
                $"&RefreshToken={Uri.EscapeDataString(response.RefreshToken ?? "")}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Yandex callback");
            return Redirect($"{Request.Scheme}://{Request.Host}/login?error={WebUtility.UrlEncode(ex.Message)}");
        }
    }

    [HttpGet("yandexid-link-callback")]
    public async Task<ActionResult> YandexIDLinkCallback(
    [FromQuery] string code,
    [FromQuery] string state,
    [FromQuery(Name = "device_id")] string deviceId)
    {
        try
        {
            // 1. Валидация входных параметров
            if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(state))
                return Redirect($"{Request.Scheme}://{Request.Host}/auth/account?error=missing_params");

            if (string.IsNullOrWhiteSpace(deviceId))
            {
                _logger.LogWarning("device_id is missing");
                return Redirect($"{Request.Scheme}://{Request.Host}/auth/account?error=missing_device_id");
            }

            // 2. Читаем cookie с userId и codeVerifier
            if (!Request.Cookies.TryGetValue($"yandexid_link_{state}", out string? cookieValue) ||
                string.IsNullOrWhiteSpace(cookieValue))
            {
                _logger.LogWarning("Invalid or expired state: {State}", state);
                return Redirect($"{Request.Scheme}://{Request.Host}/auth/account?error=invalid_state");
            }

            // 3. Удаляем cookie — она одноразовая
            Response.Cookies.Delete($"yandexid_link_{state}", new CookieOptions
            {
                Path = "/",
                Secure = true,
                SameSite = SameSiteMode.None
            });

            // 4. Парсим cookie
            string[] parts = cookieValue.Split('|');
            if (parts.Length != 2)
            {
                _logger.LogError("Invalid cookie format for state {State}", state);
                return Redirect($"{Request.Scheme}://{Request.Host}/auth/account?error=invalid_state");
            }

            string userId = parts[0];
            string codeVerifier = parts[1];

            // Find user from cookie
            ApplicationUser? currentUser = await _usersManager.FindByIdAsync(userId);
            if (currentUser is null)
            {
                _logger.LogError("User {UserId} not found", userId);
                return Redirect($"{Request.Scheme}://{Request.Host}/auth/account?error=user_not_found");
            }

            // Exchange code for Yandex User ID
            string? yandexUserId = await _authTokenGenerator.ExchangeYandexCodeForUserIdAsync(code, codeVerifier, Request.Scheme, Request.Host.Host, deviceId);
            if (string.IsNullOrEmpty(yandexUserId))
            {
                _logger.LogError("Yandex code exchange failed");
                return Redirect($"{Request.Scheme}://{Request.Host}/auth/account?error=yandex_exchange_failed");
            }

            // Check if this Yandex is already linked to a different user
            ApplicationUser? existingYandexUser = await _usersManager.FindByLoginAsync("Yandex", yandexUserId);
            if (existingYandexUser is not null && existingYandexUser.Id != currentUser.Id)
            {
                _logger.LogWarning("Yandex {YandexUserId} already linked to user {ExistingUserId}", yandexUserId, existingYandexUser.Id);
                return Redirect($"{Request.Scheme}://{Request.Host}/auth/account?error=yandexid_already_linked");
            }

            // Link the Yandex to the current user
            IdentityResult addLoginResult = await _usersManager.AddLoginAsync(
                currentUser,
                new UserLoginInfo("Yandex", yandexUserId, "Yandex"));

            if (!addLoginResult.Succeeded)
            {
                var errors = string.Join(", ", addLoginResult.Errors.Select(e => e.Description));
                _logger.LogError("Failed to link Yandex: {Errors}", errors);
                return Redirect($"{Request.Scheme}://{Request.Host}/auth/account?error={WebUtility.UrlEncode(errors)}");
            }

            // Success
            _logger.LogInformation("Yandex {YandexUserId} linked to user {UserId}", yandexUserId, currentUser.Id);
            return Redirect($"{Request.Scheme}://{Request.Host}/auth/account?success=yandexid_linked");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Yandex link callback");
            return Redirect($"{Request.Scheme}://{Request.Host}/auth/account?error={WebUtility.UrlEncode(ex.Message)}");
        }
    }

    // ============================================================
    // PROFILE / USER MANAGEMENT
    // ============================================================

    [HttpGet("current-user")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult<ApplicationUser>> GetCurrentUserAsync()
    {
        string? userId = GetCurrentUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        ApplicationUser? user = await _usersManager.FindByIdAsync(userId);
        if (user is null)
            return NotFound();

        return Ok(user);
    }

    [HttpPost("set-password")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult> ChangePassword(ChangePasswordModel changePasswordModel)
    {
        string? userId = GetCurrentUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        ApplicationUser? user = await _usersManager.FindByIdAsync(userId);
        if (user is null)
            return NotFound();

        if (!string.IsNullOrWhiteSpace(user.PasswordHash))
        {
            bool isValidPassword = await _usersManager.CheckPasswordAsync(user, changePasswordModel.CurrentPassword);
            if (!isValidPassword)
                return BadRequest("Current password is incorrect");

            IdentityResult changePasswordResult = await _usersManager.ChangePasswordAsync(
                user, changePasswordModel.CurrentPassword, changePasswordModel.NewPassword);

            if (changePasswordResult.Succeeded)
                return Ok("Password has been changed successfully");

            return StatusCode(StatusCodes.Status500InternalServerError, changePasswordResult);
        }
        else
        {
            IdentityResult addPasswordResult = await _usersManager.AddPasswordAsync(user, changePasswordModel.NewPassword);
            if (addPasswordResult.Succeeded)
                return Ok("Password has been added successfully");

            return StatusCode(StatusCodes.Status500InternalServerError, addPasswordResult);
        }
    }

    [HttpPost("addPassword/{password}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult> AddPasswordAsync(string password)
    {
        _logger.LogInformation("addPassword");

        try
        {
            string? userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            ApplicationUser? user = await _usersManager.FindByIdAsync(userId);
            if (user is null)
                return NotFound();

            if (string.IsNullOrWhiteSpace(user.PasswordHash))
            {
                await _usersManager.AddPasswordAsync(user, password);
                return Ok();
            }

            return BadRequest("Password is already set for this user");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Ошибка при добавлении пароля: {ex.Message}");
            return StatusCode(500, $"Ошибка при добавлении пароля: {ex.Message}");
        }
    }

    [HttpPost("setTwoFactorEnabled")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult> SetTwoFactorEnabled(Domain.Auth.SetTwoFactorEnabledModel setTwoFactorEnabledModel)
    {
        string? userId = GetCurrentUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        ApplicationUser? applicationUser = await _usersManager.FindByIdAsync(userId);
        if (applicationUser is null)
            return NotFound();

        IdentityResult settingTwoFactorEnabledResult = await _usersManager.SetTwoFactorEnabledAsync(
            applicationUser, setTwoFactorEnabledModel.TwoFactorEnabled);

        if (settingTwoFactorEnabledResult.Succeeded)
            return Ok("Two factor enabled fact has been changed successfully");

        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpPost("assignToAdmin")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Admin")]
    public async Task<ActionResult> AssignToAdmin(string humanToAssignToAdminEmail)
    {
        ApplicationUser? humanToAssignToAdmin = await _usersManager.FindByEmailAsync(humanToAssignToAdminEmail);
        if (humanToAssignToAdmin is null)
            return NotFound("Human to assign to admin not found");

        IdentityResult identityResult = await _usersManager.AddToRoleAsync(humanToAssignToAdmin, "Admin");
        if (identityResult is null)
            return NotFound();
        if (!identityResult.Succeeded)
            return StatusCode(StatusCodes.Status500InternalServerError, identityResult);

        return Ok(identityResult);
    }

    [HttpPost("addExternalLogin")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult> AddExternalLogin()
    {
        try
        {
            string? userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            ApplicationUser? authUser = await _usersManager.FindByIdAsync(userId);
            if (authUser is null)
                return NotFound();

            IdentityResult identityResult = await _usersManager.AddLoginAsync(
                authUser,
                new UserLoginInfo("Google", authUser.Email, "Google"));

            if (identityResult.Succeeded)
                return Ok(identityResult);

            return BadRequest(identityResult);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    // ============================================================
    // PASSWORD RESET
    // ============================================================

    [HttpPost("resetPassword")]
    public async Task<ActionResult> ResetPassword(Domain.Auth.ResetPasswordModel resetPasswordModel)
    {
        ApplicationUser? user = await _usersManager.FindByEmailAsync(resetPasswordModel.Email);
        if (user is null)
            return NotFound();

        string resetPasswordToken = await _usersManager.GeneratePasswordResetTokenAsync(user);

        MimeMessage emailMessage = new();
        emailMessage.From.Add(new MailboxAddress(_emailSettings.CurrentValue.Sender.Name, _emailSettings.CurrentValue.Sender.Email));
        emailMessage.To.Add(new MailboxAddress("", user.Email));
        emailMessage.Subject = "Reset password";
        emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
        {
            Text = $"Reset password token - {resetPasswordToken}"
        };

        using (SmtpClient client = new())
        {
            await client.ConnectAsync(_emailSettings.CurrentValue.Host, _emailSettings.CurrentValue.Port, _emailSettings.CurrentValue.UseSsl);
            await client.AuthenticateAsync(_emailSettings.CurrentValue.UserName, _emailSettings.CurrentValue.Password);
            _ = await client.SendAsync(emailMessage);
            await client.DisconnectAsync(true);
        }

        return Ok($"Email with reset password token has been send to {resetPasswordModel.Email}");
    }

    [HttpPost("resetPasswordConfirm")]
    public async Task<ActionResult> ResetPasswordConfirm(Domain.Auth.ResetPasswordConfirmModel resetPasswordModel)
    {
        ApplicationUser? user = await _usersManager.FindByEmailAsync(resetPasswordModel.Email);
        if (user is null)
            return NotFound();

        IdentityResult passwordResettingResult = await _usersManager.ResetPasswordAsync(
            user, resetPasswordModel.ResetPasswordToken, resetPasswordModel.NewPassword);

        if (passwordResettingResult.Succeeded)
            return Ok("Password has been changed successfully");

        return StatusCode(StatusCodes.Status500InternalServerError);
    }
}