using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using EzioLearning.Api.Filters;
using EzioLearning.Api.Models.Constants;
using EzioLearning.Api.Services;
using EzioLearning.Core.Dtos.Auth;
using EzioLearning.Core.Models.Response;
using EzioLearning.Core.Models.Token;
using EzioLearning.Domain.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
namespace EzioLearning.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        JwtService jwtService,
        JwtConfiguration jwtConfiguration,
        CacheService cacheService)
        : ControllerBase
    {
        private static readonly string PrefixCache = CacheConstant.AccessToken;

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
            //var sId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid)?.Value;
            //if (sId == null)
            //{
            //    return BadRequest(new ResponseBase()
            //    {
            //        StatusCode = HttpStatusCode.BadRequest,
            //        Message = "Fake request!"
            //    });
            //}
            //var item = cacheService.Get<string>(sId, prefix: PrefixCache);

            //if (!string.IsNullOrEmpty(item))
            //{
            //    cacheService.Remove(sId, PrefixCache);
            //}

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
    }
}
