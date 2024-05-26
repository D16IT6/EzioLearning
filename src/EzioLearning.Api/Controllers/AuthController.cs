using AutoMapper;
using EzioLearning.Api.Filters;
using EzioLearning.Api.Models.Auth;
using EzioLearning.Api.Services;
using EzioLearning.Api.Utils;
using EzioLearning.Core.Dto.Auth;
using EzioLearning.Domain.Entities.Identity;
using EzioLearning.Share.Dto.Auth;
using EzioLearning.Share.Models.Response;
using EzioLearning.Share.Models.Token;
using EzioLearning.Share.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Web;
using Microsoft.Extensions.Localization;
using EzioLearning.Core.Repositories.Auth;
using EzioLearning.Share.Models.ExternalLogin;

namespace EzioLearning.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(
    UserManager<AppUser> userManager,
    SignInManager<AppUser> signInManager,
    JwtService jwtService,
    JwtConfiguration jwtConfiguration,
    CacheService cacheService,
    FileService fileService,
    IMapper mapper,
    MailService mailService,
    PermissionService permissionService,
    IPermissionRepository permissionRepository, IStringLocalizer<AuthController> localizer)
    : ControllerBase
{
    private static readonly string PrefixCache = CacheConstant.AccessToken;
    private static readonly string ExternalLoginCallback = ExternalLoginConstants.CallBackPath;
    private static readonly string FolderPath = "Uploads/Images/Users/";

    #region Register

    [HttpPost("Register")]
    public async Task<IActionResult> CreateNewUser([FromForm] RegisterRequestApiDto model)
    {
        var errors = new Dictionary<string, string[]>();
        var newUser = mapper.Map<AppUser>(model);
        var image = model.Avatar;

        newUser.Id = Guid.NewGuid();

        var imagePath = ImageConstants.DefaultAvatarImage;

        if (image is { Length: > 0 })
        {
            if (!fileService.IsImageAccept(image.FileName))
            {
                errors.Add("AvatarExtensionNotAllow", [localizer.GetString("AvatarExtensionNotAllow")]);
                return BadRequest(new ResponseBase
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = localizer.GetString("AvatarExtensionNotAllow"),
                    Errors = errors
                });
            }

            imagePath = await fileService.SaveFile(image, FolderPath, newUser.Id.ToString());
        }

        newUser.Avatar = imagePath;


        var result = await userManager.CreateAsync(newUser, model.Password!);


        if (!result.Succeeded)
            return BadRequest(new ResponseBase
            {
                Status = HttpStatusCode.BadRequest,
                Message = localizer.GetString("AccountCreateFail"),
                Errors = errors
            });

        var addToRoleResult = await userManager.AddToRoleAsync(newUser, RoleConstants.User);

        if (!addToRoleResult.Succeeded)
            foreach (var addToRole in addToRoleResult.Errors)
                errors.Add(addToRole.Code, [addToRole.Description]);

        if (!string.IsNullOrEmpty(model.ProviderKey) && !string.IsNullOrEmpty(model.LoginProvider))
        {
            var addToLoginResult = await userManager.AddLoginAsync(newUser,
                new UserLoginInfo(model.LoginProvider, model.ProviderKey, model.ProviderName));

            if (addToLoginResult.Succeeded)
                return Ok(new ResponseBaseWithData<TokenResponse>
                {
                    Status = HttpStatusCode.OK,
                    Message = localizer.GetString("AccountCreateAndLinkSuccess", model.ProviderName!),
                    Data = await GenerateAndCacheToken(newUser)
                });

            foreach (var error in addToLoginResult.Errors) errors.Add(error.Code, [error.Description]);
        }

        await permissionService.AddPermissionsForNewUser(newUser);

        return Ok(new ResponseBaseWithData<TokenResponse>
        {
            Status = HttpStatusCode.OK,
            Message = localizer.GetString("AccountCreateSuccess"),
            Data = await GenerateAndCacheToken(newUser)
        });

    }

    #endregion

    #region Password Handler

    [HttpPost("ForgotPassword")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordDto model)
    {
        var errors = new Dictionary<string, string[]>();

        var user = await userManager.FindByEmailAsync(model.Email!);
        if (user == null)
        {
            errors.Add("EmailNotFound", [localizer.GetString("EmailNotFound")]);
            return BadRequest(new ResponseBase
            {
                Status = HttpStatusCode.BadRequest,
                Errors = errors,
                Message = localizer.GetString("EmailNotFound")
            });
        }

        var verifyCode = await userManager.GeneratePasswordResetTokenAsync(user);

        if (string.IsNullOrEmpty(verifyCode))
        {
            errors.Add("VerifyCodeCannotCreate", [localizer.GetString("VerifyCodeCannotCreate")]);
            return BadRequest(new ResponseBase
            {
                Status = HttpStatusCode.BadRequest,
                Errors = errors,
                Message = localizer.GetString("VerifyCodeCannotCreate")
            });
        }

        var uriBuilder = new UriBuilder(model.ClientConfirmUrl!);
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);

        query[nameof(verifyCode)] = verifyCode;
        query[nameof(user.Email)] = user.Email;

        uriBuilder.Query = query.ToString();

        var bodyBuilder = new StringBuilder();
        bodyBuilder.AppendLine($"<h2>Xin Chào {user.FullName}!</h2>");
        bodyBuilder.AppendLine("<p>Gần đây bạn đã gửi yêu cầu khôi phục mật khẩu từ chúng tôi</p>");
        bodyBuilder.AppendLine($"<p>Nếu bạn là người yêu cầu, hãy bấm vào " +
                               $"<a href=\"{uriBuilder.ToString()}\">đây</a>" +
                               $" để khôi phục mật khẩu. Link có tác dụng trong 2 giờ</p>");
        bodyBuilder.AppendLine("<p>Nếu không, xin hãy bỏ qua thư này!</p>");
        bodyBuilder.AppendLine("<h3>Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi!</h3>");

        var mailContent = new MailContent
        {
            Subject = "Thư xác thực khôi phục mật khẩu",
            HtmlBody = bodyBuilder.ToString(),
            To = user.Email
        };

        await mailService.SendMail(mailContent);
        return Ok(new ResponseBase
        {
            Message = localizer.GetString("EmailSendSuccess"),
            Status = HttpStatusCode.OK
        });
    }

    [HttpPost(nameof(ConfirmPassword))]
    public async Task<IActionResult> ConfirmPassword(ConfirmPasswordDto model)
    {
        var errors = new Dictionary<string, string[]>();
        if (string.IsNullOrEmpty(model.VerifyCode))
            errors.Add(nameof(model.VerifyCode), [localizer.GetString("VerifyCodeFail")]);

        var user = await userManager.FindByEmailAsync(model.Email!);
        if (user == null) errors.Add(nameof(model.Email), [localizer.GetString("EmailNotFound")]);

        var confirmPasswordResult =
            await userManager.ResetPasswordAsync(user!, model.VerifyCode!, model.ConfirmPassword!);

        errors.Add("ResetPassword", [localizer.GetString("ResetPasswordFail")]);
        errors.Add("ResetPassword", confirmPasswordResult.Errors.Select(x => x.Description).ToArray());

        if (errors.Any())
            return Ok(new ResponseBase
            {
                Message = localizer.GetString("ResetPasswordSuccess"),
                Status = HttpStatusCode.OK
            });

        return BadRequest(new ResponseBase
        {
            Status = HttpStatusCode.BadRequest,
            Errors = errors,
            Message = localizer.GetString("ResetPasswordFail")
        });
    }

    #endregion

    #region Token Handler

    [HttpPost("Login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginRequestDto model)
    {
        var errors = new Dictionary<string, string[]>();
        var user = await userManager.FindByNameAsync(model.UserName!);
        if (user == null)
        {
            errors.Add("NotFoundAccount", [localizer.GetString("AccountNotFound")]);
            return BadRequest(new ResponseBase
            {
                Status = HttpStatusCode.BadRequest,
                Errors = errors,
                Message = localizer.GetString("AccountNotFound")
            });
        }

        var signInResult = await signInManager
            .CheckPasswordSignInAsync(user, model.Password!, true);

        if (signInResult.IsLockedOut)
            errors.Add("LockedAccount", [localizer.GetString("AccountLocked")]);
        if (signInResult.IsNotAllowed)
            errors.Add("AccountNotVerify", [localizer.GetString("AccountNotVerify")]);

        if (signInResult.RequiresTwoFactor)
            errors.Add("AccountNotVerifyTwoFactor", [localizer.GetString("AccountNotVerifyTwoFactor")]);

        if (!signInResult.Succeeded)
            errors.Add("AccountFake", [localizer.GetString("AccountFake")]);

        if (errors.Any())
            return BadRequest(new ResponseBase
            {
                Errors = errors,
                Status = HttpStatusCode.BadRequest
            });

        var memoryToken = cacheService.Get<string>(user.CacheKey ?? "", PrefixCache);

        if (!string.IsNullOrEmpty(memoryToken))
            return Ok(new ResponseBaseWithData<TokenResponse>
            {
                Data = new TokenResponse
                {
                    AccessToken = memoryToken,
                    RefreshToken = user.RefreshToken
                },
                Message = localizer.GetString("LoginSuccess"),
                Status = HttpStatusCode.OK
            });

        var jwtToken = await GenerateAndCacheToken(user);

        return Ok(new ResponseBaseWithData<TokenResponse>
        {
            Data = jwtToken,
            Message = localizer.GetString("LoginSuccess"),
            Status = HttpStatusCode.OK
        });
    }

    [HttpPost("RevokeToken")]
    [Authorize]
    public async Task<IActionResult> RevokeToken()
    {
        var sId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid)?.Value ?? string.Empty;
        cacheService.Remove(sId, PrefixCache);

        var user = await GetUserFromEmailClaim();

        if (user != null)
        {
            user.CacheKey = string.Empty;
            await userManager.UpdateAsync(user);
        }

        return Ok(new ResponseBase
        {
            Status = HttpStatusCode.OK,
            Message = localizer.GetString("RevokeTokenSuccess")
        });
    }

    [HttpPost(nameof(RefreshToken))]
    //[Authorize]
    public async Task<IActionResult> RefreshToken([FromBody] RequestNewTokenDto model)
    {
        var user = await userManager.FindByNameAsync(model.UserName);

        if (user == null || user.RefreshToken != model.RefreshToken)
            return BadRequest(new ResponseBase
            {
                Status = HttpStatusCode.BadRequest,
                Message = localizer.GetString("RequestFake")
            });

        var jwtToken = await GenerateAndCacheToken(user);

        user.RefreshToken = jwtService.GenerateRefreshToken(user);
        var result = await userManager.UpdateAsync(user);

        if (result.Succeeded)
            return Ok(new ResponseBaseWithData<TokenResponse>
            {
                Data = jwtToken,
                Message = localizer.GetString("RefreshTokenSuccess"),
                Status = HttpStatusCode.OK
            });
        return BadRequest(new ResponseBase
        {
            Message = localizer.GetString("RefreshTokenFail"),
            Status = HttpStatusCode.BadRequest,
            Errors = (Dictionary<string, string[]>)result.Errors
                .Select(x =>
                    new KeyValuePair<string, string[]>(
                        x.Code, [localizer.GetString("RefreshTokenFail"), x.Description]
                    )
                )
        });
    }

    [HttpPost(nameof(NewToken))]
    //[Authorize]
    public async Task<IActionResult> NewToken([FromBody] RequestNewTokenDto model)
    {
        var user = await userManager.FindByNameAsync(model.UserName);

        if (user == null || user.RefreshToken != model.RefreshToken)
            return BadRequest(new ResponseBase
            {
                Status = HttpStatusCode.BadRequest,
                Message = localizer.GetString("RequestFake")
            });

        var jwtToken = await GenerateAndCacheToken(user);
        return Ok(new ResponseBaseWithData<TokenResponse>
        {
            Data = jwtToken,
            Message = localizer.GetString("RefreshTokenSuccess"),
            Status = HttpStatusCode.OK
        });
    }

    [HttpPost(nameof(TestToken))]
    [Authorize]
    [VerifyToken]
    public Task<IActionResult> TestToken()
    {
        return Task.FromResult<IActionResult>(Ok(new ResponseBase
        {
            Message = localizer.GetString("TokenLive"),
            Status = HttpStatusCode.OK
        }));
    }

    #endregion

    #region External Login

    [HttpGet]
    [Route(nameof(GoogleLogin))]
    [AllowAnonymous]
    public IActionResult GoogleLogin(string? returnUrl = null)
    {
        var provider = ExternalLoginConstants.Google;

        var redirectUrlBuilder = new StringBuilder(ExternalLoginCallback);

        if (!string.IsNullOrEmpty(returnUrl))
            redirectUrlBuilder.Append($"?returnUrl={Uri.EscapeDataString(returnUrl)}");

        var redirectUrl = redirectUrlBuilder.ToString();
        var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);


        return new ChallengeResult(provider, properties);
    }

    [HttpGet]
    [Route(nameof(FacebookLogin))]
    [AllowAnonymous]
    public IActionResult FacebookLogin(string? returnUrl = null)
    {
        var provider = ExternalLoginConstants.Facebook;

        var redirectUrlBuilder = new StringBuilder(ExternalLoginCallback);

        if (!string.IsNullOrEmpty(returnUrl))
            redirectUrlBuilder.Append($"?returnUrl={Uri.EscapeDataString(returnUrl)}");

        var redirectUrl = redirectUrlBuilder.ToString();
        var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

        return new ChallengeResult(provider, properties);
    }
    [HttpGet]
    [Route(nameof(MicrosoftLogin))]
    [AllowAnonymous]
    public IActionResult MicrosoftLogin(string? returnUrl = null)
    {
        var provider = ExternalLoginConstants.Microsoft;

        var redirectUrlBuilder = new StringBuilder(ExternalLoginCallback);

        if (!string.IsNullOrEmpty(returnUrl))
            redirectUrlBuilder.Append($"?returnUrl={Uri.EscapeDataString(returnUrl)}");

        var redirectUrl = redirectUrlBuilder.ToString();
        var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

        return new ChallengeResult(provider, properties);
    }


    [HttpGet]
    [Route(nameof(CallBack))]
    public async Task<IActionResult> CallBack(string? returnUrl = null)
    {
        if (string.IsNullOrEmpty(returnUrl))
        {
            return BadRequest(new ResponseBase
            {
                Message = localizer.GetString("ReturnUrlNotFound"),
                Status = HttpStatusCode.BadRequest
            });
        }
        var uriBuilder = new UriBuilder(returnUrl);
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);

        var externalLoginInfo = await signInManager.GetExternalLoginInfoAsync();

        if (externalLoginInfo == null)
        {
            return BadRequest(new ResponseBase()
            {
                Status = HttpStatusCode.BadRequest,
                Message = localizer.GetString("ExternalLoginNotFound")
            });
        }

        var externalLoginResult = await signInManager.ExternalLoginSignInAsync(
            externalLoginInfo.LoginProvider,
            externalLoginInfo.ProviderKey,
            false,
            true);

        var email = externalLoginInfo.Principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value!;

        ExternalLoginCacheInfo info;
        if (externalLoginResult.Succeeded)
        {
            var user = await userManager.FindByLoginAsync(externalLoginInfo.LoginProvider, externalLoginInfo.ProviderKey);
            info = await HandleExistingUser(user, email, externalLoginInfo);
        }
        else
        {
            var user = await userManager.FindByEmailAsync(email);
            info = await HandleExistingUser(user, email, externalLoginInfo);
        }

        var cacheKey = Guid.NewGuid().ToString();
        query[nameof(cacheKey)] = cacheKey;

        cacheService.Set(cacheKey, info, TimeSpan.FromMinutes(1));

        uriBuilder.Query = query.ToString();
        return Redirect(uriBuilder.ToString());
    }

    private async Task<ExternalLoginCacheInfo> HandleExistingUser(AppUser? user, string email, ExternalLoginInfo externalLoginInfo)
    {
        if (user == null) return HandleNewUser(email, externalLoginInfo);

        var externalLoginCache = new ExternalLoginCacheInfo
        {
            Token = await GenerateAndCacheToken(user)
        };
        return externalLoginCache;

    }

    private ExternalLoginCacheInfo HandleNewUser(string email, ExternalLoginInfo externalLoginInfo)
    {
        var userId = Guid.NewGuid();
        var fullName = externalLoginInfo.Principal.Identity!.Name ?? "No Name";
        var lastSpaceIndex = fullName.LastIndexOf(' ');
        var firstName = fullName[..lastSpaceIndex];
        var lastName = fullName[(lastSpaceIndex + 1)..];
        var userName = email[..email.IndexOf("@", StringComparison.OrdinalIgnoreCase)];

        var externalLoginCache = new ExternalLoginCacheInfo
        {
            UserId = userId,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            UserName = userName,
            LoginProvider = externalLoginInfo.LoginProvider,
            ProviderKey = externalLoginInfo.ProviderKey,
            ProviderName = externalLoginInfo.ProviderDisplayName ?? string.Empty,
            NeedRegister = true
        };

        return externalLoginCache;
    }

    [HttpGet]
    [Route("ExternalLoginInfo")]
    public Task<IActionResult> GetExternalLoginInfo(string cacheKey)
    {
        var info = cacheService.Get<ExternalLoginCacheInfo>(cacheKey);

        return Task.FromResult<IActionResult>(Ok(new ResponseBaseWithData<ExternalLoginCacheInfo>()
        {
            Status = HttpStatusCode.OK,
            Data = info
        }));
    }
    #endregion

    #region Support Method

    private async Task<AppUser?> GetUserFromEmailClaim()
    {
        var email = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value ?? string.Empty;
        return await userManager.FindByEmailAsync(email);
    }

    private async Task<TokenResponse> GenerateAndCacheToken(AppUser user)
    {
        var permissions = await permissionRepository.GetByUserId(user.Id);
        var expiredTime = user.RefreshTokenExpiryTime;
        var roleList = await userManager.GetRolesAsync(user);
        var jwtSecurityToken = jwtService.GenerateAccessToken(user, roleList, permissions);

        if (string.IsNullOrEmpty(user.RefreshToken) || expiredTime < DateTime.UtcNow)
        {
            user.RefreshToken = jwtService.GenerateRefreshToken(user);
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(jwtConfiguration.ExpiredRefreshTokenAfterDays);
            await userManager.UpdateAsync(user);
        }

        var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

        var cacheKey = jwtSecurityToken.Claims.First(x => x.Type == ClaimTypes.Sid).Value;
        var cacheTime = DateTimeOffset.UtcNow.AddSeconds((jwtSecurityToken.ValidTo - DateTime.UtcNow).TotalSeconds);


        user.CacheKey = cacheKey;
        await userManager.UpdateAsync(user);

        cacheService.Set(cacheKey, accessToken, cacheTime, PrefixCache);

        return new TokenResponse
        {
            AccessToken = accessToken,
            RefreshToken = user.RefreshToken
        };
    }

    #endregion
}