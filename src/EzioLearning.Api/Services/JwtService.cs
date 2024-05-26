using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using EzioLearning.Domain.Common;
using EzioLearning.Domain.Entities.Identity;
using EzioLearning.Share.Models.Token;
using Microsoft.IdentityModel.Tokens;

namespace EzioLearning.Api.Services;

public class JwtService(JwtConfiguration jwtConfiguration)
{
    public readonly SymmetricSecurityKey SecurityKey = new(Encoding.Unicode.GetBytes(jwtConfiguration.PrivateKey));

    public JwtSecurityToken GenerateAccessToken(AppUser user, IEnumerable<string> roleList, IEnumerable<AppPermission> permissions)
    {
        var credentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.Sid, Guid.NewGuid().ToString()),
            new(ClaimTypes.PrimarySid, user.Id.ToString()),
            //new(ClaimTypes.Name, user.FirstName + " " + user.LastName),
            new(ClaimTypes.NameIdentifier, user.UserName!)
            //new(ClaimTypes.Email, user.Email!),
            //new(CustomClaimTypes.Avatar, user.Avatar),
        };
        claims.AddRange(roleList
            .Select(role => new Claim(
                ClaimTypes.Role,
                role))
        );

        //claims.AddRange(permissions
        //    .Select(permission => new Claim(
        //        CustomClaimTypes.Permissions,
        //        permission.Name))
        //);

        var tempExpiredAccessTokenTime = DateTime.UtcNow.AddMinutes(jwtConfiguration.ExpiredAfterMinutes);
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