using System.Net.Http.Json;
using EzioLearning.Share.Dto.Account;
using EzioLearning.Share.Models.Response;
using EzioLearning.Wasm.Services.Interface;
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

            var responseData = (await response.GetResponse<ResponseBaseWithData<AccountInfoMinimalDto>>());

            if (responseData.Data != null && !string.IsNullOrWhiteSpace(responseData.Data.Avatar))
            {
                responseData.Data.Avatar = ApiConstants.BaseUrl + responseData.Data.Avatar + $"?t={Guid.NewGuid()}";
            }
            return responseData;
        }


        public async Task<ResponseBaseWithData<AccountInfoDto>?> UpdateInfo(AccountInfoDto newInfo)
        {
            var response = await httpClient.PutAsJsonAsync("api/Account/Info", newInfo);

            return await response.GetResponse<ResponseBaseWithData<AccountInfoDto>>();
        }            


        public async Task<ResponseBaseWithData<AccountInfoDto>?> UpdateAvatar(IBrowserFile? avatar = null)
        {
            var formContent = new MultipartFormDataContent();

            if (avatar is { Size: > 0 })
            {
                formContent.Add(new StreamContent(avatar.OpenReadStream()), nameof(AccountInfoDto.Avatar), avatar.Name);
            }

            var response = await httpClient.PutAsync("/api/Account/Avatar", formContent);
            
            return await response.GetResponse<ResponseBaseWithData<AccountInfoDto>>();
        }

        public async Task<ResponseBase?> ChangePassword(ChangePasswordDto model)
        {
            var response = await httpClient.PutAsJsonAsync("api/Account/ChangePassword",model,JsonCommonOptions.DefaultSerializer);

            return await response.GetResponse<ResponseBase>();
        }

        public async Task<ResponseBase?> ChangeEmail(ChangeEmailDto model)
        {
            var response = await httpClient.PutAsJsonAsync("/api/AccountRoute/ChangeEmail", model);
            var responseData = await response.GetResponse<ResponseBase>();
            return responseData;
        }

        public async Task<ResponseBase?> Delete()
        {
            var response = await httpClient.DeleteAsync("api/Account");

            return await response.GetResponse<ResponseBase>();
        }

    }
}
