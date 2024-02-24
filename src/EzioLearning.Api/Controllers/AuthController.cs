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
using SlugGenerator;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Web;

namespace EzioLearning.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        JwtService jwtService,
        JwtConfiguration jwtConfiguration,
        CacheService cacheService, FileService fileService, IMapper mapper)
        : ControllerBase
    {
        private static readonly string PrefixCache = CacheConstant.AccessToken;
        private static readonly string ExternalLoginCallback = ExternalLoginConstants.CallBackPath;
        private static readonly string FolderPath = "Uploads/Images/Users/";


        #region Register

        [HttpPost("Register")]
        [ValidateModel]
        public async Task<IActionResult> CreateNewUser([FromForm] RegisterRequestDto model)
        {
            var newUser = mapper.Map<AppUser>(model);
            var image = model.Avatar;

            newUser.Id = Guid.NewGuid();

            var imagePath = ImageConstants.DefaultAvatarImage;

            if (image is { Length: > 0 })
            {
                if (!fileService.IsImageAccept(image.FileName))
                {
                    return BadRequest(new ResponseBase()
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        Message = "Ảnh đầu vào không hợp lệ, vui lòng chọn định dạng khác"
                    });
                }

                imagePath = await fileService.SaveFile(image, FolderPath, newUser.Id.ToString());
            }

            newUser.Avatar = imagePath;

            var addToRoleResult = await userManager.AddToRoleAsync(newUser, RoleConstants.User);

            if (!addToRoleResult.Succeeded)
            {
                return BadRequest(new ResponseBaseWithList<IdentityError>()
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Data = addToRoleResult.Errors.ToList(),
                    Message = "Thêm quyền vào tài khoản thất bại, vui lòng xem lỗi"
                });
            }

            var result = await userManager.CreateAsync(newUser, model.Password!);
            if (result.Succeeded)
                return Ok(new ResponseBase()
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = "Tạo tài khoản mới thành công"
                });

            return BadRequest(new ResponseBaseWithList<string>()
            {
                StatusCode = HttpStatusCode.BadRequest,
                Data = result.Errors.Select(x => x.Description).ToList(),
                Message = "Tạo tài khoản thất bại, vui lòng xem lỗi"
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
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "Không tìm thấy tài khoản"
                });

            var signInResult = await signInManager
                .CheckPasswordSignInAsync(user, model.PassWord!, true);


            if (signInResult.IsLockedOut)
                return BadRequest(new ResponseBase()
                {
                    Message = "Tài khoản bị khoá",
                    StatusCode = HttpStatusCode.BadRequest
                });
            if (signInResult.IsNotAllowed)
                return BadRequest(new ResponseBase()
                {
                    Message = "Tài khoản chưa xác thực",
                    StatusCode = HttpStatusCode.BadRequest
                });

            if (signInResult.RequiresTwoFactor)
                return BadRequest(new ResponseBase()
                {
                    Message = "Tài khoản cần xác thực 2 lớp",
                    StatusCode = HttpStatusCode.BadRequest
                });
            if (!signInResult.Succeeded)
                return BadRequest(new ResponseBase()
                {
                    Message = "Thông tin đăng nhập không chính xác",
                    StatusCode = HttpStatusCode.BadRequest
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
                StatusCode = HttpStatusCode.OK
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
                StatusCode = HttpStatusCode.OK,
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
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "Fake request!"
                });
            }

            var jwtToken = await GenerateAndCacheToken(user);

            return Ok(new ResponseBaseWithData<TokenResponse>()
            {
                Data = jwtToken,
                Message = "Refresh token thành công",
                StatusCode = HttpStatusCode.OK
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
                    StatusCode = HttpStatusCode.BadRequest
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

            var errorBuilder = new StringBuilder();

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

            var newUser = new AppUser()
            {
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                Id = userId,
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                UserName = fullName.GenerateSlug(),
                LockoutEnabled = false,
                Avatar = ImageConstants.DefaultAvatarImage

            };

            var newUserCreateResult = await userManager.CreateAsync(newUser);

            if (newUserCreateResult.Succeeded)
            {
                await userManager.AddToRoleAsync(newUser, RoleConstants.User);

                var addLoginInfoResult = await userManager.AddLoginAsync(newUser, info);

                if (addLoginInfoResult.Succeeded)
                {
                    query[nameof(userId)] = userId.ToString();
                    query[nameof(email)] = email;
                    query[nameof(firstName)] = firstName;
                    query[nameof(lastName)] = lastName;
                    query["NeedRegister"] = true.ToString();
                }
                else
                {
                    errorBuilder.AppendJoin(',', addLoginInfoResult.Errors.Select(x => x.Description));
                    query["BackToLogin"] = true.ToString();
                }
            }
            else
            {
                errorBuilder.AppendJoin(',', newUserCreateResult.Errors.Select(x => x.Description));
                query["BackToLogin"] = true.ToString();
            }

            query["Errors"] = errorBuilder.ToString();

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
