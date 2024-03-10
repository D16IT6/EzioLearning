using System.Net;
using System.Security.Claims;
using AutoMapper;
using EzioLearning.Api.Filters;
using EzioLearning.Core.SeedWorks.Constants;
using EzioLearning.Domain.Entities.Identity;
using EzioLearning.Share.Dto.Account;
using EzioLearning.Share.Models.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EzioLearning.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Permissions.Dashboard.View)]
    [VerifyToken]
    public class AccountController(UserManager<AppUser> userManager, IMapper mapper, RoleManager<AppRole> roleManager) : ControllerBase
    {
        [HttpGet(nameof(Info))]
        public async Task<IActionResult> Info()
        {
            var userName = HttpContext.User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.NameIdentifier))?.Value;
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

    }
}
