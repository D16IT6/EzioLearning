using EzioLearning.Blazor.Client.Providers;
using EzioLearning.Core.Dtos.Auth;
using EzioLearning.Core.Models.Response;
using EzioLearning.Core.Models.Token;
using EzioLearning.Wasm.Providers;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace EzioLearning.Wasm.Services
{
    public class AuthService(HttpClient httpClient, ITokenService tokenService, AuthenticationStateProvider stateProvider) : IAuthService
    {
        private readonly ApiAuthenticationStateProvider _apiAuthenticationStateProvider =
            (ApiAuthenticationStateProvider)stateProvider;
        public async Task<ResponseBase?> Login(LoginRequestDto loginRequestDto)
        {
            var response = await httpClient.PostAsJsonAsync("api/Auth/Login", loginRequestDto);
            await using var stream = await response.Content.ReadAsStreamAsync();

            ResponseBase? data = await JsonSerializer.DeserializeAsync<ResponseBaseWithData<TokenResponse>>(stream, JsonCommonOptions.DefaultSerializer);

            if (data!.Status == HttpStatusCode.OK)
            {
                await tokenService.SaveFromResponse(data);
                _apiAuthenticationStateProvider.MarkUserAsAuthenticated(loginRequestDto.UserName!);
            }

            return data;
        }

        public async Task<ResponseBase?> Register(RegisterRequestClientDto model, IBrowserFile? avatar)
        {
            var multipartContent = new MultipartFormDataContent();

            if (avatar != null)
            {
                var fileContent = new StreamContent(avatar.OpenReadStream());

                multipartContent.Add(fileContent, $"{nameof(model.Avatar)}", avatar.Name);
            }

            multipartContent.Add(new StringContent(model.FirstName!), nameof(model.FirstName));
            multipartContent.Add(new StringContent(model.LastName!), nameof(model.LastName));
            multipartContent.Add(new StringContent(model.UserName!), nameof(model.UserName));

            multipartContent.Add(new StringContent(model.Password!), nameof(model.Password));
            multipartContent.Add(new StringContent(model.ConfirmPassword!), nameof(model.ConfirmPassword));

            multipartContent.Add(new StringContent(model.PhoneNumber!), nameof(model.PhoneNumber));
            multipartContent.Add(new StringContent(model.Email!), nameof(model.Email));
            multipartContent.Add(new StringContent(model.DateOfBirth.ToString("yyyy-MM-dd")), nameof(model.DateOfBirth));


            multipartContent.Add(new StringContent(model.LoginProvider!), nameof(model.LoginProvider));
            multipartContent.Add(new StringContent(model.ProviderName!), nameof(model.ProviderName));
            multipartContent.Add(new StringContent(model.ProviderKey!), nameof(model.ProviderKey));


            var response = await httpClient.PostAsync("api/Auth/Register", multipartContent);


            await using var stream = await response.Content.ReadAsStreamAsync();
            ResponseBase? data = await JsonSerializer.DeserializeAsync<ResponseBaseWithData<TokenResponse>>(stream, JsonCommonOptions.DefaultSerializer);
            return data;
        }

        public async Task<ResponseBase?> Logout()
        {
            ResponseBase? data = null;
            var response = await httpClient.PostAsync("api/Auth/RevokeToken", null);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                await using var stream = await response.Content.ReadAsStreamAsync();
                data = await JsonSerializer.DeserializeAsync<ResponseBase>(stream, JsonCommonOptions.DefaultSerializer);
            }

            await tokenService.DeleteToken();

            _apiAuthenticationStateProvider.MarkUserAsLoggedOut();

            return data;
        }
    }
}
