using Blazored.LocalStorage;
using EzioLearning.Share.Models.Response;
using EzioLearning.Share.Models.Token;
using EzioLearning.Wasm.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using EzioLearning.Wasm.Services.Interface;

namespace EzioLearning.Wasm.Services.Implement;

public class TokenService(ILocalStorageService localStorageService) : ITokenService
{
    public async Task SaveFromResponse(ResponseBase response)
    {
        if (response is ResponseBaseWithData<TokenResponse> responseWithData) await SaveToken(responseWithData.Data!);
    }

    public async Task SaveToken(TokenResponse token)
    {
        if (!string.IsNullOrEmpty(token.AccessToken) &&
            !string.IsNullOrEmpty(token.RefreshToken))
        {
            await localStorageService.SetItemAsStringAsync(LocalStorageConstants.AccessToken, token.AccessToken);
            await localStorageService.SetItemAsStringAsync(LocalStorageConstants.RefreshToken, token.RefreshToken);
        }
    }

    public async Task DeleteToken()
    {
        await localStorageService.RemoveItemAsync(LocalStorageConstants.AccessToken);
        await localStorageService.RemoveItemAsync(LocalStorageConstants.RefreshToken);
    }

    public async Task<IEnumerable<Claim>> ParseClaimsFromJwt(string? accessToken)
    {
        accessToken ??= await localStorageService.GetItemAsStringAsync(LocalStorageConstants.AccessToken);

        var jwtTokenHandler = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);

        return jwtTokenHandler.Claims;
    }

    public async Task<TokenResponse> GetTokenFromLocalStorage()
    {
        var accessToken = await localStorageService.GetItemAsStringAsync(LocalStorageConstants.AccessToken);
        var refreshToken = await localStorageService.GetItemAsStringAsync(LocalStorageConstants.RefreshToken);
        return new TokenResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public async Task<bool> IsTokenExpired(string? accessToken)
    {
        accessToken ??= await localStorageService.GetItemAsStringAsync(LocalStorageConstants.AccessToken);

        var jwtHandler = new JwtSecurityTokenHandler();

        if (!jwtHandler.CanReadToken(accessToken)) return true;

        var jwtToken = jwtHandler.ReadJwtToken(accessToken);
        var expiration = jwtToken.ValidTo;
        var utcNow = DateTime.UtcNow;
        return expiration < DateTime.UtcNow;
    }
}