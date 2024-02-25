using EzioLearning.Core.Models.Response;
using EzioLearning.Core.Models.Token;
using System.Security.Claims;

namespace EzioLearning.Wasm.Services
{
    public interface ITokenService
    {
        Task SaveFromResponse(ResponseBase response);
        Task SaveToken(TokenResponse token);
        Task DeleteToken();
        Task<bool> IsTokenExpired(string? accessToken = null);
        Task<IEnumerable<Claim>> ParseClaimsFromJwt(string? accessToken = null);
        Task<TokenResponse> GetTokenFromLocalStorage();
    }
}
