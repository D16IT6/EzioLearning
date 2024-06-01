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
    public async Task<ResponseBaseWithData<TokenResponse>> Register(RegisterRequestClientDto model)
    {
        var multipartContent = new MultipartFormDataContent().CreateFormContent(model,"avatar");

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
}