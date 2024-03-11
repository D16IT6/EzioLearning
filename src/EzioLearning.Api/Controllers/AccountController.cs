using System.Net;
using System.Security.Claims;
using AutoMapper;
using EzioLearning.Api.Filters;
using EzioLearning.Domain.Entities.Identity;
using EzioLearning.Share.Dto.Account;
using EzioLearning.Share.Models.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EzioLearning.Api.Services;
using EzioLearning.Api.Utils;

namespace EzioLearning.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [VerifyToken]
    [Authorize]
    public class AccountController(UserManager<AppUser> userManager, IMapper mapper, RoleManager<AppRole> roleManager, FileService fileService)
        : ControllerBase
    {
        private static readonly string FolderPath = "Uploads/Images/Users/";

        [HttpGet(nameof(Info))]
        public async Task<IActionResult> Info()
        {
            var userName = HttpContext.User.Claims
                .FirstOrDefault(x => x.Type.Equals(ClaimTypes.NameIdentifier))?.Value;
            if (userName == null)
            {
                return BadRequest(new ResponseBase()
                {
                    Errors = new Dictionary<string, string[]>()
                        { { "NotFound", ["Không tìm thấy tài khoản"] } }
                });
            }

            var user = await userManager.FindByNameAsync(userName);

            if (user == null)
            {
                return BadRequest(new ResponseBase()
                {
                    Errors = new Dictionary<string, string[]>()
                        { { "NotFound", ["Không tìm thấy tài khoản"] } }
                });
            }

            var accountInfoDto = mapper.Map<AccountInfoDto>(user);
            var roleList = await roleManager.Roles.ToListAsync();
            var accountRoles = await userManager.GetRolesAsync(user);

            accountInfoDto.Roles = roleList
                .Where(x => x.Name != null && accountRoles.Contains(x.Name))
                .Select(x => x.DisplayName)
                .ToArray();

            return Ok(new ResponseBaseWithData<AccountInfoDto>()
            {
                Status = HttpStatusCode.OK,
                Message = "Lấy thông tin tài khoản thành công",
                Data = accountInfoDto
            });
        }

        [HttpPut(nameof(Info))]
        public async Task<IActionResult> UpdateInfo([FromBody] AccountInfoDto accountInfoDto)
        {
            var user = await userManager.FindByNameAsync(accountInfoDto.UserName!);
            if (user == null)
            {
                return BadRequest(new ResponseBase()
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = "Không tìm thấy tài khoản",
                    Errors = new Dictionary<string, string[]>()
                    {
                        { "NotFound", ["Không tìm thấy tài khoản"] }
                    }
                });
            }

            user.FirstName = accountInfoDto.FirstName!;
            user.LastName = accountInfoDto.LastName!;
            user.DateOfBirth = accountInfoDto.DateOfBirth;
            user.PhoneNumber = accountInfoDto.PhoneNumber!;

            var result = await userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Ok(new ResponseBaseWithData<AccountInfoDto>()
                {
                    Data = mapper.Map<AccountInfoDto>(user),
                    Status = HttpStatusCode.OK,
                    Message = "Cập nhật thông tin thành công"
                });
            }

            return BadRequest(new ResponseBase()
            {
                Status = HttpStatusCode.BadRequest,
                Message = "Cập nhật thông tin thất bại",
                Errors = (Dictionary<string, string[]>)result.Errors.Select(x =>
                    new KeyValuePair<string, string[]>
                    (
                        x.Code,
                        [x.Description]
                    )
            )
            });
        }



        [HttpGet(nameof(avatar))]
        public async Task<IActionResult> GetAvatar([FromForm] IFormFile? avatar)
        {
            var userId = HttpContext.User.Claims.First(x => x.Type.Equals(ClaimTypes.PrimarySid)).Value;

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return BadRequest(new ResponseBase()
                {
                    Errors = new Dictionary<string, string[]>()
                        { { "NotFound", ["Không tìm thấy tài khoản"] } }
                });
            }
            return Ok(new ResponseBaseWithData<string>()
            {
                Data = user.Avatar,
                Status = HttpStatusCode.OK,
                Message = "Lấy ảnh đại diện thành công"
            });
        }

        [HttpPut(nameof(avatar))]
        public async Task<IActionResult> UpdateAvatar([FromForm] IFormFile? avatar)
        {
            var userId = HttpContext.User.Claims.First(x => x.Type.Equals(ClaimTypes.PrimarySid)).Value;

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return BadRequest(new ResponseBase()
                {
                    Errors = new Dictionary<string, string[]>()
                        { { "NotFound", ["Không tìm thấy tài khoản"] } }
                });
            }

            var imagePath = ImageConstants.DefaultAvatarImage;
            if (avatar is { Length: > 0 })
            {
                if (!fileService.IsImageAccept(avatar.FileName))
                {
                    return BadRequest(new ResponseBase
                    {
                        Status = HttpStatusCode.BadRequest,
                        Message = "Ảnh đầu vào không hợp lệ, vui lòng chọn định dạng khác",
                        Errors = new Dictionary<string, string[]>{
                            {"Image", ["Ảnh đầu vào không hợp lệ, vui lòng chọn định dạng khác"]}
                        }
                    });
                }

                imagePath = await fileService.SaveFile(avatar, FolderPath, user.Id.ToString());
            }

            user.Avatar = imagePath;
            var result = await userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Ok(new ResponseBaseWithData<AccountInfoDto>()
                {
                    Data = mapper.Map<AccountInfoDto>(user),
                    Status = HttpStatusCode.OK,
                    Message = "Cập nhật thông tin thành công"
                });
            }

            return BadRequest(new ResponseBase()
            {
                Status = HttpStatusCode.BadRequest,
                Message = "Cập nhật thông tin thất bại",
                Errors = (Dictionary<string, string[]>)result.Errors.Select(x =>
                    new KeyValuePair<string, string[]>
                    (
                        x.Code,
                        [x.Description]
                    )
                )
            });
        }

    }
}
