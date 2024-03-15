using EzioLearning.Share.Dto.Account;
using EzioLearning.Share.Models.Response;
using Microsoft.AspNetCore.Components.Forms;

namespace EzioLearning.Wasm.Services.Interface
{
    public interface IAccountService: IServiceBase
    {
        Task<ResponseBaseWithData<AccountInfoDto>> GetInfo();
        Task<ResponseBaseWithData<AccountInfoMinimalDto>> GetMinimalInfo();
        Task<ResponseBaseWithData<AccountInfoDto>?> UpdateInfo(AccountInfoDto accountInfo);
        Task<ResponseBaseWithData<AccountInfoDto>?> UpdateAvatar(IBrowserFile? avatar = null);
        Task<ResponseBase?> ChangePassword(ChangePasswordDto model);
    }
}
