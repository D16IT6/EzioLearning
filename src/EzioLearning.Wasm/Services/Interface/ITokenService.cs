using System.Security.Claims;
using EzioLearning.Share.Models.Response;
using EzioLearning.Share.Models.Token;

namespace EzioLearning.Wasm.Services.Interface;

public interface ITokenService : IServiceBase
{
    Task SaveFromResponse(ResponseBase response);
    Task SaveToken(TokenResponse token);
    Task DeleteToken();
    Task<bool> IsTokenExpired(string? accessToken = null);
    Task<IEnumerable<Claim>> ParseClaimsFromJwt(string? accessToken = null);
    Task<TokenResponse> GetTokenFromLocalStorage();
}