using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using EzioLearning.Share.Dto.Auth;
using EzioLearning.Share.Models.Response;
using EzioLearning.Share.Models.Token;
using EzioLearning.Wasm.Common;
using EzioLearning.Wasm.Services.Interface;
using Microsoft.AspNetCore.Components.Authorization;

namespace EzioLearning.Wasm.Providers;

public class ApiAuthenticationStateProvider(HttpClient httpClient, ITokenService tokenService)
    : AuthenticationStateProvider
{
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var emptyAuthenticationState = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

        var token = await tokenService.GetTokenFromLocalStorage();

        if (string.IsNullOrWhiteSpace(token.AccessToken))
            return emptyAuthenticationState;

        try
        {
            var isLive = await tokenService.IsLiveToken();
            if (!isLive)
            {
                if (await GenerateNewToken(token) is not ResponseBaseWithData<TokenResponse> newTokenResponse
                    || string.IsNullOrWhiteSpace(newTokenResponse.Data?.AccessToken))
                    return emptyAuthenticationState;

                token.AccessToken = newTokenResponse.Data.AccessToken;

            }

            var claims = await tokenService.ParseClaimsFromJwt(token.AccessToken);
            var identity = new ClaimsIdentity(claims, ApiConstants.ApiAuthenticationType);
            var authenticatedState = new AuthenticationState(new ClaimsPrincipal(identity));

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

            return authenticatedState;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error occurred while validating authentication state: {ex.Message}");
            return emptyAuthenticationState;
        }
    }

    private async Task<ResponseBase?> GenerateNewToken(TokenResponse token)
    {
        var userName = (await tokenService.ParseClaimsFromJwt(token.AccessToken))
            .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!
            .Value;

        if (string.IsNullOrEmpty(userName)) return null;


        var requestModel = new RequestNewTokenDto
        {
            UserName = userName,
            RefreshToken = token.RefreshToken!
        };

        var response = await httpClient.PostAsJsonAsync("api/Auth/NewToken", requestModel);

        var stream = await response.Content.ReadAsStreamAsync();
        ResponseBase? data = await JsonSerializer.DeserializeAsync<ResponseBaseWithData<TokenResponse>>(stream,
            JsonCommonOptions.DefaultSerializer);
        if (response.StatusCode == HttpStatusCode.OK) await tokenService.SaveFromResponse(data!);

        return data;
    }

    public void MarkUserAsAuthenticated(string userName)
    {
        var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name,
                    userName)
            },
            ApiConstants.ApiAuthenticationType));
        var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
        NotifyAuthenticationStateChanged(authState);
    }

    public void MarkUserAsLoggedOut()
    {
        var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
        var authState = Task.FromResult(new AuthenticationState(anonymousUser));
        NotifyAuthenticationStateChanged(authState);
    }
}