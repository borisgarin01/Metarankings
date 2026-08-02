using IdentityLibrary.DTOs;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace IdentityLibrary.Services.Interfaces;

public interface ITokensService
{
    SigningCredentials GetSigningCredentials();
    Task<List<Claim>> GetClaims(ApplicationUser user);
    JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}
