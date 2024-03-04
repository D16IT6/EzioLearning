using EzioLearning.Share.Dto.Auth;
using EzioLearning.Share.Models.Response;
using Microsoft.AspNetCore.Components.Forms;

namespace EzioLearning.Wasm.Services.Interface;

public interface IAuthService
{
    public Task<ResponseBase?> Login(LoginRequestDto loginRequestDto);
    public Task<ResponseBase?> Register(RegisterRequestClientDto registerRequestClientDto, IBrowserFile? avatar = null);
    public Task<ResponseBase?> Logout();

    public Task<ResponseBase?> ForgotPassword(ForgotPasswordDto forgotPasswordDto);
    public Task<ResponseBase?> ConfirmPassword(ConfirmPasswordDto confirmPasswordDto);
}