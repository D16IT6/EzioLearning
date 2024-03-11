using System.Net.Http.Json;
using EzioLearning.Share.Dto.Account;
using EzioLearning.Share.Models.Response;
using EzioLearning.Wasm.Common;
using EzioLearning.Wasm.Services.Interface;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Forms;

namespace EzioLearning.Wasm.Services.Implement
{
    public class AccountService(HttpClient httpClient) : IAccountService
    {
        public async Task<ResponseBaseWithData<AccountInfoDto>> GetInfo()
        {
            var response = await httpClient.GetAsync("api/Account/Info");
            await using var stream = await response.Content.ReadAsStreamAsync();

            return (await JsonSerializer.DeserializeAsync<ResponseBaseWithData<AccountInfoDto>>(stream,
                JsonCommonOptions.DefaultSerializer))!;

        }

        public async Task<ResponseBaseWithData<AccountInfoDto>?> UpdateInfo(AccountInfoDto newInfo)
        {
            var response = await httpClient.PutAsJsonAsync("api/Account/Info", newInfo);

            await using var stream = await response.Content.ReadAsStreamAsync();

            var responseData =
                await JsonSerializer.DeserializeAsync<ResponseBaseWithData<AccountInfoDto>>(stream,
                    JsonCommonOptions.DefaultSerializer);
            return responseData;
        }

        public async Task<ResponseBaseWithData<AccountInfoDto>?> UpdateAvatar(IBrowserFile? avatar = null)
        {
            var formContent = new MultipartFormDataContent();

            if (avatar is { Size: > 0 })
            {
                formContent.Add(new StreamContent(avatar.OpenReadStream()), nameof(AccountInfoDto.Avatar), avatar.Name);
            }
            else
            {
                formContent.Add(new StringContent("Test"), "Test");
            }

            var response = await httpClient.PutAsync("/api/Account/Avatar", formContent);

            await using var stream = await response.Content.ReadAsStreamAsync();

            var responseData =
                await JsonSerializer.DeserializeAsync<ResponseBaseWithData<AccountInfoDto>>(stream,
                    JsonCommonOptions.DefaultSerializer);
            return responseData;
        }

        public async Task<ResponseBaseWithData<string>?> GetAvatar()
        {
            var response = await httpClient.GetAsync("api/Account/Avatar");

            await using var stream = await response.Content.ReadAsStreamAsync();

            var responseData =
                await JsonSerializer.DeserializeAsync<ResponseBaseWithData<string>>(stream,
                    JsonCommonOptions.DefaultSerializer);
            return responseData;
        }
    }
}
