using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Blazored.LocalStorage;
using EzioLearning.Blazor.Client.Providers;
using EzioLearning.Core.Dtos.Auth;
using EzioLearning.Core.Models.Response;
using EzioLearning.Core.Models.Token;
using EzioLearning.Wasm.Providers;
using Microsoft.AspNetCore.Components.Authorization;

namespace EzioLearning.Wasm.Services
{
    public class AuthService(HttpClient httpClient, ILocalStorageService localStorageService, AuthenticationStateProvider stateProvider) : IAuthService
    {

        public async Task<ResponseBase?> Login(LoginRequestDto loginRequestDto)
        {
            ResponseBase? data = null;

            var response = await httpClient.PostAsJsonAsync("api/Auth/Login", loginRequestDto);
            var stream = await response.Content.ReadAsStreamAsync();

            switch (response.StatusCode)
            {
                case HttpStatusCode.BadRequest:

                    data = await JsonSerializer.DeserializeAsync<ResponseBase>(stream, JsonCommonOptions.DefaultSerializer);
                    return data;
                case HttpStatusCode.OK:

                    data = await JsonSerializer.DeserializeAsync<ResponseBaseWithData<TokenResponse>>(stream, JsonCommonOptions.DefaultSerializer);

                    var dataChild = data as ResponseBaseWithData<TokenResponse>;

                    await localStorageService.SetItemAsync(LocalStorageConstants.AccessToken, dataChild?.Data?.AccessToken);
                    await localStorageService.SetItemAsync(LocalStorageConstants.RefreshToken, dataChild?.Data?.RefreshToken);


                    ((ApiAuthenticationStateProvider)stateProvider).MarkUserAsAuthenticated(loginRequestDto.UserName!);
                    break;
            }

            return data;
        }
        
        public async Task Logout()
        {
            await localStorageService.RemoveItemAsync(LocalStorageConstants.AccessToken);
            await localStorageService.RemoveItemAsync(LocalStorageConstants.AccessToken);

            ((ApiAuthenticationStateProvider)stateProvider).MarkUserAsLoggedOut();
        }
    }
}
