using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using EzioLearning.Domain.Common;
using EzioLearning.Domain.Entities.Identity;
using EzioLearning.Share.Models.Token;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace EzioLearning.Api.Services;

public class JwtService(JwtConfiguration jwtConfiguration)
{
    public readonly SymmetricSecurityKey SecurityKey = new(Encoding.Unicode.GetBytes(jwtConfiguration.PrivateKey));

    public JwtSecurityToken GenerateAccessToken(AppUser user, IList<string> roleList)
    {
        var credentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.Sid, Guid.NewGuid().ToString()),
            new(ClaimTypes.PrimarySid, user.Id.ToString()),
            new(ClaimTypes.Name, user.FirstName! + " " + user.LastName!),
            new(ClaimTypes.NameIdentifier, user.UserName!),
            new(ClaimTypes.Email, user.Email!),
            new(CustomClaimTypes.Avatar, user.Avatar!)
        };
        foreach (var role in roleList)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var tempExpiredAccessTokenTime = DateTime.Now.AddMinutes(jwtConfiguration.ExpiredAfterMinutes);
        //var tempExpiredAccessTokenTime = DateTime.UtcNow.AddSeconds(30);//Test token

        var naturalExpiredTokenTime =
            user.RefreshTokenExpiryTime < tempExpiredAccessTokenTime && user.RefreshTokenExpiryTime != null
                ? user.RefreshTokenExpiryTime
                : tempExpiredAccessTokenTime;

        var jwtToken = new JwtSecurityToken(
            jwtConfiguration.Issuer,
            jwtConfiguration.Audience,
            expires: naturalExpiredTokenTime,
            signingCredentials: credentials,
            claims: claims
        );

        return jwtToken;
    }

    public string GenerateRefreshToken(AppUser? _)
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}