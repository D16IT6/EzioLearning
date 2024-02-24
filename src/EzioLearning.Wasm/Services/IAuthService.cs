using EzioLearning.Core.Dtos.Auth;
using EzioLearning.Core.Models.Response;

namespace EzioLearning.Wasm.Services
{
    public interface IAuthService
    {
        public Task<ResponseBase?> Login(LoginRequestDto loginRequestDto);
        public Task<ResponseBase?> Logout();
    }
}
