using EzioLearning.Share.Dto.Auth;
using EzioLearning.Share.Models.Response;
using EzioLearning.Share.Models.Token;
using EzioLearning.Wasm.Common;
using EzioLearning.Wasm.Providers;
using EzioLearning.Wasm.Services.Interface;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace EzioLearning.Wasm.Services.Implement;

public class AuthService(
    HttpClient httpClient,
    ITokenService tokenService,
    AuthenticationStateProvider stateProvider
) : IAuthService
{
    private readonly ApiAuthenticationStateProvider _apiAuthenticationStateProvider =
        (ApiAuthenticationStateProvider)stateProvider;

    public async Task<ResponseBase?> Login(LoginRequestDto loginRequestDto)
    {
        var response = await httpClient.PostAsJsonAsync("api/Auth/Login", loginRequestDto);
        await using var stream = await response.Content.ReadAsStreamAsync();

        ResponseBase? data =
            await JsonSerializer.DeserializeAsync<ResponseBaseWithData<TokenResponse>>(stream,
                JsonCommonOptions.DefaultSerializer);

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
            //if (avatar.Size > 10 * 1024 * 1024)//Fake Validator 😂
            //{
            //    return new ResponseBase()
            //    {
            //        Status = HttpStatusCode.BadRequest,
            //        Errors = new Dictionary<string, string[]>()
            //        {
            //            { "FileSize", ["Kích thước file quá lớn, vui lòng chọn file khác"] }
            //        }
            //    };
            //}
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
        multipartContent.Add(new StringContent(model.DateOfBirth.ToString("yyyy-MM-dd")), nameof(model.DateOfBirth));


        if (!string.IsNullOrEmpty(model.LoginProvider))
            multipartContent.Add(new StringContent(model.LoginProvider!), nameof(model.LoginProvider));
        if (!string.IsNullOrEmpty(model.ProviderKey))
            multipartContent.Add(new StringContent(model.ProviderKey!), nameof(model.ProviderKey));
        if (!string.IsNullOrEmpty(model.ProviderName))
            multipartContent.Add(new StringContent(model.ProviderName!), nameof(model.ProviderName));

        var response = await httpClient.PostAsync("api/Auth/Register", multipartContent);


        await using var stream = await response.Content.ReadAsStreamAsync();
        ResponseBase? data =
            await JsonSerializer.DeserializeAsync<ResponseBaseWithData<TokenResponse>>(stream,
                JsonCommonOptions.DefaultSerializer);
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

    public async Task<ResponseBase?> ForgotPassword(ForgotPasswordDto forgotPasswordDto)
    {
        var data = await httpClient.PostAsJsonAsync("api/Auth/ForgotPassword", forgotPasswordDto,
            JsonCommonOptions.DefaultSerializer);

        await using var responseStream = await data.Content.ReadAsStreamAsync();
        var responseBase =
            await JsonSerializer.DeserializeAsync<ResponseBase>(responseStream, JsonCommonOptions.DefaultSerializer);

        return responseBase;
    }

    public async Task<ResponseBase?> ConfirmPassword(ConfirmPasswordDto confirmPasswordDto)
    {
        var data = await httpClient.PostAsJsonAsync("api/Auth/ConfirmPassword", confirmPasswordDto,
            JsonCommonOptions.DefaultSerializer);

        await using var responseStream = await data.Content.ReadAsStreamAsync();
        var responseBase =
            await JsonSerializer.DeserializeAsync<ResponseBase>(responseStream, JsonCommonOptions.DefaultSerializer);

        return responseBase;
    }
}