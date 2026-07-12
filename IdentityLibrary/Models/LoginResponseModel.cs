namespace IdentityLibrary.Models;

public sealed record LoginResponseModel(string UserId, string AccessToken, long TokenExpired, string RefreshToken, bool RequiresTwoFactor);
