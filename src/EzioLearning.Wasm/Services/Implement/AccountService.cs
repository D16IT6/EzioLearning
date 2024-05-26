using System.Net.Http.Json;
using EzioLearning.Share.Dto.Account;
using EzioLearning.Share.Models.Response;
using EzioLearning.Wasm.Services.Interface;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Forms;
using EzioLearning.Wasm.Utils.Common;
using EzioLearning.Wasm.Utils.Extensions;

namespace EzioLearning.Wasm.Services.Implement
{
    public class AccountService(HttpClient httpClient) : IAccountService
    {
        public async Task<ResponseBaseWithData<AccountInfoDto>> GetInfo()
        {
            var response = await httpClient.GetAsync("api/Account/Info");
            return await response.GetResponse<ResponseBaseWithData<AccountInfoDto>>();
        }

        public async Task<ResponseBaseWithData<AccountInfoMinimalDto>> GetMinimalInfo()
        {
            var response = await httpClient.GetAsync("api/Account/MinimalInfo");
            await using var stream = await response.Content.ReadAsStreamAsync();

            return (await response.GetResponse<ResponseBaseWithData<AccountInfoMinimalDto>>());
        }


        public async Task<ResponseBaseWithData<AccountInfoDto>?> UpdateInfo(AccountInfoDto newInfo)
        {
            var response = await httpClient.PutAsJsonAsync("api/Account/Info", newInfo);

            await using var stream = await response.Content.ReadAsStreamAsync();

            return await response.GetResponse<ResponseBaseWithData<AccountInfoDto>>();
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
            
            return await response.GetResponse<ResponseBaseWithData<AccountInfoDto>>();
        }

        public async Task<ResponseBase?> ChangePassword(ChangePasswordDto model)
        {
            var response = await httpClient.PutAsJsonAsync("api/Account/ChangePassword",model,JsonCommonOptions.DefaultSerializer);

            return await response.GetResponse<ResponseBase>();
        }

        public async Task<ResponseBase?> Delete()
        {
            var response = await httpClient.DeleteAsync("api/Account");

            return await response.GetResponse<ResponseBase>();
        }

    }
}
