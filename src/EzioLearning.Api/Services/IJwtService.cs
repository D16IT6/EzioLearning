using EzioLearning.Domain.Entities.Identity;
using System.IdentityModel.Tokens.Jwt;

namespace EzioLearning.Api.Services
{
    public interface IJwtService
    {
        JwtSecurityToken GenerateAccessToken(AppUser user, IList<string> roleList);
        string GenerateRefreshToken(AppUser user);

    }
}
