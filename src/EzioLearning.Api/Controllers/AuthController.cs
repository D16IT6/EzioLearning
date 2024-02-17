using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using EzioLearning.Api.Filters;
using EzioLearning.Api.Models.Constants;
using EzioLearning.Api.Models.Response;
using EzioLearning.Api.Models.Token;
using EzioLearning.Api.Services;
using EzioLearning.Core.Dtos.Auth;
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
        IJwtService jwtService,
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
            var user = await userManager.FindByNameAsync(model.UserName);
            if (user == null)
                return BadRequest(new ResponseBase()
                {
                    StatusCode = 400,
                    Message = "Không tìm thấy tài khoản"
                });

            var signInResult = await signInManager
                .CheckPasswordSignInAsync(user, model.PassWord, true);


            if (signInResult.IsLockedOut)
                return BadRequest(new ResponseBase()
                {
                    Message = "Tài khoản bị khoá",
                    StatusCode = 400
                });
            if (signInResult.IsNotAllowed)
                return BadRequest(new ResponseBase()
                {
                    Message = "Tài khoản chưa xác thực",
                    StatusCode = 400
                });

            if (signInResult.RequiresTwoFactor)
                return BadRequest(new ResponseBase()
                {
                    Message = "Tài khoản cần xác thực 2 lớp",
                    StatusCode = 400
                });
            if (!signInResult.Succeeded)
                return BadRequest(new ResponseBase()
                {
                    Message = "Thông tin đăng nhập không chính xác",
                    StatusCode = 400
                });

            var memoryToken = cacheService.Get<string>(user.Id.ToString(), PrefixCache);

            if (!string.IsNullOrEmpty(memoryToken))
            {
                return Ok(new ResponseBase()
                {
                    Data = new
                    {
                        AccessToken = memoryToken,
                        user.RefreshToken
                    }
                });
            }

            var jwtToken = await GenerateAndCacheToken(user);

            return Ok(new ResponseBase()
            {
                Data = jwtToken,
                Message = "Đăng nhập thành công",
                StatusCode = 200
            });
        }


        [HttpPost("RevokeToken")]
        [Authorize]
        public Task<IActionResult> RevokeToken()
        {
            var sId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid)?.Value ?? string.Empty;
            cacheService.Remove(sId, PrefixCache);
            return Task.FromResult<IActionResult>(Ok(new ResponseBase()
            {
                StatusCode = 200,
                Message = "Revoked token!"
            }));
        }

        [HttpPost("RefreshToken")]
        [Authorize]
        public async Task<IActionResult> RefreshToken()
        {
            var sId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid)?.Value;
            if (sId == null)
            {
                return BadRequest(new ResponseBase()
                {
                    StatusCode = 400,
                    Message = "Fake request!"
                });
            }
            var item = cacheService.Get<string>(sId, prefix: PrefixCache);
            if (!string.IsNullOrEmpty(item))
            {
                cacheService.Remove(sId, PrefixCache);
            }

            var email = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value ?? string.Empty;

            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return BadRequest(new ResponseBase()
                {
                    StatusCode = 400,
                    Message = "Fake request!"
                });
            }

            var jwtToken = await GenerateAndCacheToken(user);

            return Ok(new ResponseBase()
            {
                Data = jwtToken,
                Message = "Refresh token thành công",
                StatusCode = 200
            });

        }

        private async Task<JwtResponse> GenerateAndCacheToken(AppUser user)
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

            cacheService.Set(cacheKey, accessToken, cacheTime, PrefixCache);

            return new JwtResponse()
            {
                AccessToken = accessToken,
                RefreshToken = user.RefreshToken
            };
        }
    }
}
