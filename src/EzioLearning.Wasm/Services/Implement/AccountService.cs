using EzioLearning.Share.Dto.Account;
using EzioLearning.Share.Models.Response;
using EzioLearning.Wasm.Common;
using EzioLearning.Wasm.Services.Interface;
using System.Text.Json;

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
    }
}
