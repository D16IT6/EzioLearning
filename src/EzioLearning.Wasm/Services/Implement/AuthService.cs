using EzioLearning.Share.Dto.Auth;
using EzioLearning.Share.Models.Response;
using EzioLearning.Share.Models.Token;
using EzioLearning.Wasm.Providers;
using EzioLearning.Wasm.Services.Interface;
using EzioLearning.Wasm.Utils.Common;
using EzioLearning.Wasm.Utils.Extensions;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http.Json;
using EzioLearning.Share.Models.ExternalLogin;

namespace EzioLearning.Wasm.Services.Implement;

public class AuthService(
    HttpClient httpClient,
    ITokenService tokenService,
    AuthenticationStateProvider stateProvider
) : IAuthService
{
    private readonly ApiAuthenticationStateProvider _apiAuthenticationStateProvider =
        (ApiAuthenticationStateProvider)stateProvider;

    public async Task<ResponseBaseWithData<TokenResponse>> Login(LoginRequestDto loginRequestDto)
    {
        var response = await httpClient.PostAsJsonAsync("api/Auth/Login", loginRequestDto);

        return await response.GetResponse<ResponseBaseWithData<TokenResponse>>();
    }
    public async Task<ResponseBaseWithData<TokenResponse>> Register(RegisterRequestClientDto model, IBrowserFile? avatar)
    {
        var multipartContent = new MultipartFormDataContent();

        if (avatar != null)
        {
            await using FileStream fs = new(Path.GetRandomFileName(), FileMode.Create);

            var fileContent = new StreamContent(avatar.OpenReadStream(avatar.Size));

            multipartContent.Add(fileContent, $"{nameof(model.Avatar)}", avatar.Name);
        }

        multipartContent.Add(new StringContent(model.FirstName!), nameof(model.FirstName));
        multipartContent.Add(new StringContent(model.LastName!), nameof(model.LastName));
        multipartContent.Add(new StringContent(model.UserName!), nameof(model.UserName));

        multipartContent.Add(new StringContent(model.Password!), nameof(model.Password));
        multipartContent.Add(new StringContent(model.ConfirmPassword!), nameof(model.ConfirmPassword));

        multipartContent.Add(new StringContent(model.PhoneNumber!), nameof(model.PhoneNumber));
        multipartContent.Add(new StringContent(model.Email!), nameof(model.Email));
        if (model.DateOfBirth.HasValue)
        {
            multipartContent.Add(new StringContent(model.DateOfBirth.Value.ToString("yyyy-MM-dd")), nameof(model.DateOfBirth));

        }

        if (!string.IsNullOrEmpty(model.LoginProvider))
            multipartContent.Add(new StringContent(model.LoginProvider!), nameof(model.LoginProvider));
        if (!string.IsNullOrEmpty(model.ProviderKey))
            multipartContent.Add(new StringContent(model.ProviderKey!), nameof(model.ProviderKey));
        if (!string.IsNullOrEmpty(model.ProviderName))
            multipartContent.Add(new StringContent(model.ProviderName!), nameof(model.ProviderName));

        var response = await httpClient.PostAsync("api/Auth/Register", multipartContent);

        return await response.GetResponse<ResponseBaseWithData<TokenResponse>>();
    }

    public async Task<ResponseBase?> Logout()
    {
        var response = await httpClient.PostAsync("api/Auth/RevokeToken", null);

        await tokenService.DeleteToken();

        _apiAuthenticationStateProvider.MarkUserAsLoggedOut();

        return await response.GetResponse<ResponseBase>();
    }

    public async Task<ResponseBase?> ForgotPassword(ForgotPasswordDto forgotPasswordDto)
    {
        var response = await httpClient.PostAsJsonAsync("api/Auth/ForgotPassword", forgotPasswordDto,
            JsonCommonOptions.DefaultSerializer);

        return await response.GetResponse<ResponseBase>();
    }

    public async Task<ResponseBase?> ConfirmPassword(ConfirmPasswordDto confirmPasswordDto)
    {
        var response = await httpClient.PostAsJsonAsync("api/Auth/ConfirmPassword", confirmPasswordDto,
            JsonCommonOptions.DefaultSerializer);

        return await response.GetResponse<ResponseBase>();
    }
    public async Task<ResponseBaseWithData<ExternalLoginCacheInfo>> GetExternalLoginInfo(string cacheKey)
    {
        var response =
            await httpClient
                .GetFromJsonAsync<ResponseBaseWithData<ExternalLoginCacheInfo>>(
                    $"/api/Auth/ExternalLoginInfo?cacheKey={cacheKey}", JsonCommonOptions.DefaultSerializer);
        return response!;
    }
}