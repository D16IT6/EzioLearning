using EzioLearning.Share.Dto.Auth;
using EzioLearning.Share.Models.Response;
using EzioLearning.Share.Models.Token;
using EzioLearning.Wasm.Providers;
using EzioLearning.Wasm.Services.Interface;
using EzioLearning.Wasm.Utils.Common;
using EzioLearning.Wasm.Utils.Extensions;
using Microsoft.AspNetCore.Components.Authorization;
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
    public async Task<ResponseBaseWithData<TokenResponse>> Register(RegisterRequestClientDto model)
    {
        var multipartContent = new MultipartFormDataContent().CreateFormContent(model,nameOfFileContent:["Avatar"]);

        if (model.DateOfBirth.HasValue)
        {
            multipartContent.Add(new StringContent(model.DateOfBirth.Value.ToString("yyyy-MM-dd")), nameof(model.DateOfBirth));
        }

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

    public Task<string> GetExternalLoginUrl(string provider, string? returnUrl)
    {
        //var response = await httpClient.GetAsync($"api/Auth/{provider}Login?{nameof(returnUrl)}={returnUrl}");

        //if (response.StatusCode is System.Net.HttpStatusCode.Found or System.Net.HttpStatusCode.Redirect or System.Net.HttpStatusCode.MovedPermanently)
        //{
        //    var redirectUri = response.Headers.Location;
        //    return redirectUri?.AbsoluteUri ?? "";
        //}
        //return string.Empty;

        return Task.FromResult($"{ApiConstants.BaseUrl}/api/Auth/{provider}Login?{nameof(returnUrl)}={returnUrl}");
    }
}