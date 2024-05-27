using System.Net.Http.Headers;
using System.Security.Claims;
using EzioLearning.Share.Models.Token;
using EzioLearning.Wasm.Services.Interface;
using EzioLearning.Wasm.Utils.Common;
using Microsoft.AspNetCore.Components.Authorization;

namespace EzioLearning.Wasm.Providers;

public class ApiAuthenticationStateProvider(HttpClient httpClient, ITokenService tokenService)
    : AuthenticationStateProvider
{
    private static readonly SemaphoreSlim SemaphoreSlim = new(1);
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var emptyAuthenticationState = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

        var token = await tokenService.GetTokenFromLocalStorage();

        if (string.IsNullOrWhiteSpace(token.AccessToken))
            return emptyAuthenticationState;
        
        try
        {
            await SemaphoreSlim.WaitAsync();

            if (await tokenService.IsLiveToken())
            {
                return await SetTokenAuthenticated(token);
            }

            var newTokenResponse = await tokenService.GenerateNewToken();
            if (newTokenResponse == null
                || string.IsNullOrWhiteSpace(newTokenResponse.AccessToken))
            {
                return emptyAuthenticationState;
            }

            token.AccessToken = newTokenResponse.AccessToken;

            return await SetTokenAuthenticated(token);
        }
        catch 
        {
            return emptyAuthenticationState;
        }
        finally
        {
            SemaphoreSlim.Release();
        }
    }

    private async Task<AuthenticationState> SetTokenAuthenticated(TokenResponse token)
    {
        var claims = await tokenService.ParseClaimsFromJwt();
        var identity = new ClaimsIdentity(claims, ApiConstants.ApiAuthenticationType);
        var authenticatedState = new AuthenticationState(new ClaimsPrincipal(identity));

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
        return authenticatedState;
    }
    
    public void MarkUserAsLoggedOut()
    {
        var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
        var emptyAuthState = Task.FromResult(new AuthenticationState(anonymousUser));
        NotifyAuthenticationStateChanged(emptyAuthState);
    }
}