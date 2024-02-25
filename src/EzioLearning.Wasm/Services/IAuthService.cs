using EzioLearning.Core.Dtos.Auth;
using EzioLearning.Core.Models.Response;
using Microsoft.AspNetCore.Components.Forms;

namespace EzioLearning.Wasm.Services
{
    public interface IAuthService
    {
        public Task<ResponseBase?> Login(LoginRequestDto loginRequestDto);
        public Task<ResponseBase?> Register(RegisterRequestClientDto registerRequestClientDto,IBrowserFile? avatar = null);
        public Task<ResponseBase?> Logout();
    }
}
