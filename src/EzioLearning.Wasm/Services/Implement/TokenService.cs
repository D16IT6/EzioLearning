using Blazored.LocalStorage;
using EzioLearning.Share.Dto.Auth;
using EzioLearning.Share.Models.Response;
using EzioLearning.Share.Models.Token;
using EzioLearning.Wasm.Services.Interface;
using EzioLearning.Wasm.Utils.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using EzioLearning.Wasm.Utils.Extensions;

namespace EzioLearning.Wasm.Services.Implement;

public class TokenService(ILocalStorageService localStorageService, HttpClient httpClient) : ITokenService
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

    public async Task<DateTime> GetTokenExpiredTime()
    {
        var accessToken = await localStorageService.GetItemAsStringAsync(LocalStorageConstants.AccessToken);

        var jwtHandler = new JwtSecurityTokenHandler();

        if (!jwtHandler.CanReadToken(accessToken)) return DateTime.Now;

        var jwtToken = jwtHandler.ReadJwtToken(accessToken);
        return jwtToken.ValidTo;
    }


    public async Task<IEnumerable<Claim>> ParseClaimsFromJwt()
    {
        var accessToken = await localStorageService.GetItemAsStringAsync(LocalStorageConstants.AccessToken);

        var jwtTokenHandler = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);

        return jwtTokenHandler.Claims;
    }

    public async Task<TokenResponse> GetTokenFromLocalStorage()
    {
        var accessToken = await localStorageService.GetItemAsStringAsync(LocalStorageConstants.AccessToken);
        var refreshToken = await localStorageService.GetItemAsStringAsync(LocalStorageConstants.RefreshToken);
        return new TokenResponse
        {
            AccessToken = accessToken!,
            RefreshToken = refreshToken!
        };
    }

    public async Task<TokenResponse?> GenerateNewToken()
    {
        var token = await GetTokenFromLocalStorage();
        var userName = (await ParseClaimsFromJwt())
            .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!
            .Value;

        if (string.IsNullOrEmpty(userName)) return null;


        var requestModel = new RequestNewTokenDto
        {
            UserName = userName,
            RefreshToken = token.RefreshToken
        };

        var response = await httpClient.PostAsJsonAsync("api/Auth/NewToken", requestModel);

        var data = await response.GetResponse<ResponseBaseWithData<TokenResponse>>();
        if (response.StatusCode == HttpStatusCode.OK) await SaveFromResponse(data);

        return data.Data;

    }
    public async Task<bool> IsLiveToken()
    {
        var accessToken = await localStorageService.GetItemAsStringAsync(LocalStorageConstants.AccessToken);
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await httpClient.PostAsync("/api/Auth/TestToken", null);

        var success = response.IsSuccessStatusCode;
        var isTokenExpired = await IsTokenExpired();

        return success && !isTokenExpired;
    }

    public async Task<bool> IsTokenExpired()
    {
        var accessToken = await localStorageService.GetItemAsStringAsync(LocalStorageConstants.AccessToken);

        var jwtHandler = new JwtSecurityTokenHandler();

        if (!jwtHandler.CanReadToken(accessToken)) return true;

        var jwtToken = jwtHandler.ReadJwtToken(accessToken);
        var expiration = jwtToken.ValidTo;
        return expiration < DateTime.UtcNow;
    }
}