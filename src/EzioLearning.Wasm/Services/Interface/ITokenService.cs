using System.Security.Claims;
using EzioLearning.Share.Models.Response;
using EzioLearning.Share.Models.Token;

namespace EzioLearning.Wasm.Services.Interface;

public interface ITokenService : IServiceBase
{
    Task SaveFromResponse(ResponseBase response);
    Task SaveToken(TokenResponse token);
    Task DeleteToken();
    Task<bool> IsTokenExpired();
    Task<DateTime> GetTokenExpiredTime();
    Task<IEnumerable<Claim>> ParseClaimsFromJwt();
    Task<TokenResponse> GetTokenFromLocalStorage();

    Task<TokenResponse?> GenerateNewToken();
    Task<bool> IsLiveToken();
}