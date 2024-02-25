using AutoMapper;
using EzioLearning.Api.Filters;
using EzioLearning.Api.Models.Constants;
using EzioLearning.Api.Services;
using EzioLearning.Api.Utils;
using EzioLearning.Core.Dtos.Auth;
using EzioLearning.Core.Models.Response;
using EzioLearning.Core.Models.Token;
using EzioLearning.Domain.Common;
using EzioLearning.Domain.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Web;
using EzioLearning.Api.Models.Auth;
using System;

namespace EzioLearning.Api.Controllers
{
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
        MailService mailService)
        : ControllerBase
    {
        private static readonly string PrefixCache = CacheConstant.AccessToken;
        private static readonly string ExternalLoginCallback = ExternalLoginConstants.CallBackPath;
        private static readonly string FolderPath = "Uploads/Images/Users/";

        #region Password Handler

        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto model)
        {
            var errors = new Dictionary<string, string[]>();

            var user = await userManager.FindByEmailAsync(model.Email!);
            if (user == null)
            {
                errors.Add("Email", ["Email không tồn tại trong hệ thống"]);
                return BadRequest(new ResponseBase()
                {
                    Status = HttpStatusCode.BadRequest,
                    Errors = errors,
                });
            }

            var verifyCode = await userManager.GeneratePasswordResetTokenAsync(user);

            if (string.IsNullOrEmpty(verifyCode))
            {
                errors.Add("VerifyCode", ["Không thể tạo VerifyCode, vui lòng thử lại"]);
                return BadRequest(new ResponseBase()
                {
                    Status = HttpStatusCode.BadRequest,
                    Errors = errors,
                });
            }

            var uriBuilder = new UriBuilder(model.ClientConfirmUrl!);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);

            query[nameof(verifyCode)] = verifyCode;
            query[nameof(user.Email)] = user.Email;

            uriBuilder.Query = query.ToString();

            var bodyBuilder = new StringBuilder();
            bodyBuilder.AppendLine($"<h2>Xin Chào {user.FullName}!</h2>");
            bodyBuilder.AppendLine($"<p>Gần đây bạn đã gửi yêu cầu khôi phục mật khẩu từ chúng tôi</p>");
            bodyBuilder.AppendLine($"<p>Nếu bạn là người yêu cầu, hãy bấm vào " +
                                   $"<a href=\"{uriBuilder.ToString()}\">đây</a>" +
                                   $" để khôi phục mật khẩu. Link có tác dụng trong 2 giờ</p>");
            bodyBuilder.AppendLine($"<p>Nếu không, xin hãy bỏ qua thư này!</p>");
            bodyBuilder.AppendLine("<h3>Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi!</h3>");

            var mailContent = new MailContent
            {
                Subject = "Thư xác thực khôi phục mật khẩu",
                HtmlBody = bodyBuilder.ToString(),
                To = user.Email
            };

            await mailService.SendMail(mailContent);
            return Ok(new ResponseBase()
            {
                Message = "Gửi email khôi phục thành công",
                Status = HttpStatusCode.OK
            });
        }

        [HttpPost(nameof(ConfirmPassword))]

        public async Task<IActionResult> ConfirmPassword(ConfirmPasswordDto model)
        {
            var errors = new Dictionary<string, string[]>();
            if (string.IsNullOrEmpty(model.VerifyCode))
            {
                errors.Add(nameof(model.VerifyCode), ["VerifyCode lỗi, không thể xác thực"]);
            }

            var user = await userManager.FindByEmailAsync(model.Email!);
            if (user == null)
            {
                errors.Add(nameof(model.Email), ["Email không tồn tại trên hệ thống"]);
            }

            var confirmPasswordResult =
                await userManager.ResetPasswordAsync(user!, model.VerifyCode!, model.ConfirmPassword!);

            errors.Add("ResetPassword", confirmPasswordResult.Errors.Select(x => x.Description).ToArray());

            if (errors.Any())
            {
                return Ok(new ResponseBase()
                {
                    Message = "Khôi phục mật khẩu thành công",
                    Status = HttpStatusCode.OK,
                });
            }

            return BadRequest(new ResponseBase()
            {
                Status = HttpStatusCode.BadRequest,
                Errors = errors
            });

        }

        #endregion
        #region Register

        [HttpPost("Register")]
        [ValidateModel]
        public async Task<IActionResult> CreateNewUser([FromForm] RegisterRequestDto model)
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
                    errors.Add("Image", ["Ảnh đầu vào không hợp lệ, vui lòng chọn định dạng khác"]);
                    return BadRequest(new ResponseBase()
                    {
                        Status = HttpStatusCode.BadRequest,
                        Message = "Ảnh đầu vào không hợp lệ, vui lòng chọn định dạng khác",
                        Errors = errors
                    });
                }

                imagePath = await fileService.SaveFile(image, FolderPath, newUser.Id.ToString());
            }

            newUser.Avatar = imagePath;



            var result = await userManager.CreateAsync(newUser, model.Password!);


            if (result.Succeeded)
            {
                var addToRoleResult = await userManager.AddToRoleAsync(newUser, RoleConstants.User);

                if (!addToRoleResult.Succeeded)
                {
                    foreach (var addToRole in addToRoleResult.Errors)
                    {
                        errors.Add(addToRole.Code, [addToRole.Description]);
                    }
                }

                if (!string.IsNullOrEmpty(model.ProviderKey) && !string.IsNullOrEmpty(model.LoginProvider))
                {
                    var addToLoginResult = await userManager.AddLoginAsync(newUser,
                        new UserLoginInfo(model.LoginProvider, model.ProviderKey, model.ProviderName));

                    if (addToLoginResult.Succeeded)
                    {
                        return Ok(new ResponseBaseWithData<TokenResponse>()
                        {
                            Status = HttpStatusCode.OK,
                            Message = $"Tạo tài khoản mới và liên kết tới {model.ProviderName} thành công",
                            Data = await GenerateAndCacheToken(newUser)
                        });
                    }

                    foreach (var error in addToLoginResult.Errors)
                    {
                        errors.Add(error.Code, [error.Description]);
                    }

                }
                return Ok(new ResponseBaseWithData<TokenResponse>()
                {
                    Status = HttpStatusCode.OK,
                    Message = $"Tạo tài khoản mới thành công!",
                    Data = await GenerateAndCacheToken(newUser)
                });
            }

            return BadRequest(new ResponseBase()
            {
                Status = HttpStatusCode.BadRequest,
                Message = "Tạo tài khoản thất bại, vui lòng xem lỗi",
                Errors = errors
            });
        }

        #endregion


        #region Token Handler

        [HttpPost("Login")]
        [ValidateModel]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginRequestDto model)
        {
            var user = await userManager.FindByNameAsync(model.UserName!);
            if (user == null)
                return BadRequest(new ResponseBase()
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = "Không tìm thấy tài khoản"
                });

            var signInResult = await signInManager
                .CheckPasswordSignInAsync(user, model.Password!, true);


            if (signInResult.IsLockedOut)
                return BadRequest(new ResponseBase()
                {
                    Message = "Tài khoản bị khoá",
                    Status = HttpStatusCode.BadRequest
                });
            if (signInResult.IsNotAllowed)
                return BadRequest(new ResponseBase()
                {
                    Message = "Tài khoản chưa xác thực",
                    Status = HttpStatusCode.BadRequest
                });

            if (signInResult.RequiresTwoFactor)
                return BadRequest(new ResponseBase()
                {
                    Message = "Tài khoản cần xác thực 2 lớp",
                    Status = HttpStatusCode.BadRequest
                });
            if (!signInResult.Succeeded)
                return BadRequest(new ResponseBase()
                {
                    Message = "Thông tin đăng nhập không chính xác",
                    Status = HttpStatusCode.BadRequest
                });

            var memoryToken = cacheService.Get<string>(user.CacheKey ?? "", PrefixCache);

            if (!string.IsNullOrEmpty(memoryToken))
            {
                return Ok(new ResponseBaseWithData<TokenResponse>()
                {
                    Data = new TokenResponse()
                    {
                        AccessToken = memoryToken,
                        RefreshToken = user.RefreshToken
                    }
                });
            }

            var jwtToken = await GenerateAndCacheToken(user);

            return Ok(new ResponseBaseWithData<TokenResponse>()
            {
                Data = jwtToken,
                Message = "Đăng nhập thành công",
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
            return Ok(new ResponseBase()
            {
                Status = HttpStatusCode.OK,
                Message = "Revoked token!"
            });
        }

        [HttpPost("RefreshToken")]
        //[Authorize]
        public async Task<IActionResult> RefreshToken([FromBody] RequestNewTokenDto model)
        {

            var user = await userManager.FindByNameAsync(model.UserName);

            if (user == null || user.RefreshToken != model.RefreshToken)
            {
                return BadRequest(new ResponseBase()
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = "Fake request!"
                });
            }

            var jwtToken = await GenerateAndCacheToken(user);

            return Ok(new ResponseBaseWithData<TokenResponse>()
            {
                Data = jwtToken,
                Message = "Refresh token thành công",
                Status = HttpStatusCode.OK
            });

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
            {
                redirectUrlBuilder.Append($"?returnUrl={Uri.EscapeDataString(returnUrl)}");
            }

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
            {
                redirectUrlBuilder.Append($"?returnUrl={Uri.EscapeDataString(returnUrl)}");
            }

            var redirectUrl = redirectUrlBuilder.ToString();
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

            return new ChallengeResult(provider, properties);
        }


        [HttpGet]
        [Route(nameof(CallBack))]
        public async Task<IActionResult> CallBack(string? returnUrl = null)
        {
            if (string.IsNullOrEmpty(returnUrl))
                return BadRequest(new ResponseBase()
                {
                    Message = "Không tìm được return url",
                    Status = HttpStatusCode.BadRequest
                });

            var uriBuilder = new UriBuilder(returnUrl);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);

            //Không tìm thấy thông tin đăng nhập từ dịch vụ bên ngoài
            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                query["BackToLogin"] = true.ToString();
            }

            //Kiểm tra thông tin trong db, không tự tạo user
            var externalLoginResult = await signInManager
                .ExternalLoginSignInAsync(
                    info!.LoginProvider,
                    info.ProviderKey,
                    isPersistent: false,
                    bypassTwoFactor: true
                    );

            var claims = info.Principal.Claims.Select(x => new { x.Type, x.Value });

            var fullName = info.Principal.Identity!.Name;

            var email = claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)!.Value;

            //đã có user
            if (externalLoginResult.Succeeded)
            {
                var user = await userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
                if (user != null)
                {
                    var token = await GenerateAndCacheToken(user);
                    query[nameof(token.AccessToken)] = token.AccessToken;
                    query[nameof(token.RefreshToken)] = token.RefreshToken;

                    query["UserId"] = user.Id.ToString();

                    uriBuilder.Query = query.ToString();
                    return Redirect(uriBuilder.ToString());
                }
            }
            else
            {
                var user = await userManager.FindByEmailAsync(email);
                if (user != null)
                {
                    var token = await GenerateAndCacheToken(user);
                    query[nameof(token.AccessToken)] = token.AccessToken;
                    query[nameof(token.RefreshToken)] = token.RefreshToken;
                    await userManager.AddLoginAsync(user, info);

                    uriBuilder.Query = query.ToString();
                    return Redirect(uriBuilder.ToString());
                }
            }

            //chưa có user
            var userId = Guid.NewGuid();

            int lastSpaceIndex = fullName!.LastIndexOf(' ');

            string firstName = fullName.Substring(0, lastSpaceIndex);

            string lastName = fullName.Substring(lastSpaceIndex + 1);

            string userName = email.Substring(0, email.IndexOf("@", StringComparison.Ordinal));

            var loginProvider = info.LoginProvider;
            var providerKey = info.ProviderKey;
            var providerName = info.ProviderDisplayName;



            query[nameof(userId)] = userId.ToString();
            query[nameof(email)] = email;
            query[nameof(firstName)] = firstName;
            query[nameof(lastName)] = lastName;
            query[nameof(userName)] = userName;

            query[nameof(loginProvider)] = loginProvider;
            query[nameof(providerKey)] = providerKey;
            query[nameof(providerName)] = providerName;

            query["NeedRegister"] = true.ToString();



            uriBuilder.Query = query.ToString();
            return Redirect(uriBuilder.ToString());
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
            var expiredTime = user.RefreshTokenExpiryTime;
            var roleList = await userManager.GetRolesAsync(user);
            var jwtSecurityToken = jwtService.GenerateAccessToken(user, roleList.ToList());

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

            return new TokenResponse()
            {
                AccessToken = accessToken,
                RefreshToken = user.RefreshToken
            };
        }

        #endregion


    }
}
