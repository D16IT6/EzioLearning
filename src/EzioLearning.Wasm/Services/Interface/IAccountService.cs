using EzioLearning.Share.Dto.Account;
using EzioLearning.Share.Models.Response;

namespace EzioLearning.Wasm.Services.Interface
{
    public interface IAccountService: IServiceBase
    {
        Task<ResponseBaseWithData<AccountInfoDto>> GetInfo();
    }
}
