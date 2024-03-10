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

        if (string.IsNullOrWhiteSpace(token.AccessToken)) return emptyAuthenticationState;

        try
        {
            var validatedAuthenticationState = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(
                await tokenService.ParseClaimsFromJwt(token.AccessToken),
                ApiConstants.ApiAuthenticationType)));

            if (await tokenService.IsTokenExpired(token.AccessToken))
            {
                if (await GenerateNewToken(token)
                    is not ResponseBaseWithData<TokenResponse> response)
                    return emptyAuthenticationState;

                token.AccessToken = response.Data?.AccessToken ?? string.Empty;

                validatedAuthenticationState = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(
                    await tokenService.ParseClaimsFromJwt(token.AccessToken),
                    ApiConstants.ApiAuthenticationType)));
            }
            httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token.AccessToken);
            return validatedAuthenticationState;
        }
        catch
        {
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

        var response = await httpClient.PostAsJsonAsync("api/Auth/RefreshToken", requestModel);

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