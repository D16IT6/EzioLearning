using EzioLearning.Share.Dto.Auth;
using EzioLearning.Share.Models.ExternalLogin;
using EzioLearning.Share.Models.Response;
using EzioLearning.Share.Models.Token;
using Microsoft.AspNetCore.Components.Forms;

namespace EzioLearning.Wasm.Services.Interface;

public interface IAuthService: IServiceBase
{
    public Task<ResponseBaseWithData<TokenResponse>> Login(LoginRequestDto loginRequestDto);
    public Task<ResponseBaseWithData<TokenResponse>> Register(RegisterRequestClientDto registerRequestClientDto, IBrowserFile? avatar = null);
    public Task<ResponseBase?> Logout();

    public Task<ResponseBase?> ForgotPassword(ForgotPasswordDto forgotPasswordDto);
    public Task<ResponseBase?> ConfirmPassword(ConfirmPasswordDto confirmPasswordDto);

    public Task<ResponseBaseWithData<ExternalLoginCacheInfo>> GetExternalLoginInfo(string cacheKey);
}