using System.Net;
using System.Security.Claims;
using System.Text;
using System.Web;
using AutoMapper;
using EzioLearning.Api.Filters;
using EzioLearning.Api.Models.Auth;
using EzioLearning.Api.Services;
using EzioLearning.Api.Utils;
using EzioLearning.Core.Dto.Account;
using EzioLearning.Domain.Entities.Identity;
using EzioLearning.Share.Dto.Account;
using EzioLearning.Share.Models.Response;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using MailService = EzioLearning.Api.Services.MailService;

namespace EzioLearning.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[VerifyToken]
public class AccountController(
    UserManager<AppUser> userManager,
    IMapper mapper,
    RoleManager<AppRole> roleManager,
    FileService fileService,
    MailService mailService,
    CacheService cacheService,IStringLocalizer<AccountController> localizer)
    : ControllerBase
{
    private const string FolderPath = "Uploads/Images/Users/";

    #region Info

    [HttpGet(nameof(Info))]
    public async Task<IActionResult> Info()
    {
        var userName = HttpContext.User.Claims
            .FirstOrDefault(x => x.Type.Equals(ClaimTypes.NameIdentifier))?.Value;
        if (userName == null)
            return BadRequest(new ResponseBase
            {
                Errors = new Dictionary<string, string[]> { { "NotFound", [localizer.GetString("AccountNotFound")] } }
            });

        var user = await userManager.FindByNameAsync(userName);

        if (user == null)
            return BadRequest(new ResponseBase
            {
                Errors = new Dictionary<string, string[]> { { "NotFound", [localizer.GetString("AccountNotFound")] } }
            });

        var accountInfoDto = mapper.Map<AccountInfoDto>(user);
        var roleList = await roleManager.Roles.ToListAsync();
        var accountRoles = await userManager.GetRolesAsync(user);

        accountInfoDto.Roles = roleList
            .Where(x => x.Name != null && accountRoles.Contains(x.Name))
            .Select(x => x.DisplayName)
            .ToArray();

        return Ok(new ResponseBaseWithData<AccountInfoDto>
        {
            Status = HttpStatusCode.OK,
            Message = localizer.GetString("AccountInfoFound"),
            Data = accountInfoDto
        });
    }

    [HttpGet(nameof(MinimalInfo))]
    public async Task<IActionResult> MinimalInfo()
    {
        var userName = HttpContext.User.Claims
            .FirstOrDefault(x => x.Type.Equals(ClaimTypes.NameIdentifier))?.Value;
        if (userName == null)
            return BadRequest(new ResponseBase
            {
                Errors = new Dictionary<string, string[]> { { "NotFound", [localizer.GetString("AccountNotFound")] } }
            });

        var user = await userManager.FindByNameAsync(userName);

        if (user == null)
            return BadRequest(new ResponseBase
            {
                Errors = new Dictionary<string, string[]> { { "NotFound", [localizer.GetString("AccountNotFound")] } }
            });

        var accountInfoMinimalDto = mapper.Map<AccountInfoMinimalDto>(user);

        return Ok(new ResponseBaseWithData<AccountInfoMinimalDto>
        {
            Status = HttpStatusCode.OK,
            Message = localizer.GetString("AccountInfoFound"),
            Data = accountInfoMinimalDto
        });
    }

    [HttpPut(nameof(Info))]
    public async Task<IActionResult> UpdateInfo([FromBody] AccountInfoDto accountInfoDto)
    {
        var user = await userManager.FindByNameAsync(accountInfoDto.UserName!);
        if (user == null)
            return BadRequest(new ResponseBase
            {
                Status = HttpStatusCode.BadRequest,
                Message = localizer.GetString("AccountNotFound"),
                Errors = new Dictionary<string, string[]>
                {
                    { "NotFound", [localizer.GetString("AccountNotFound")] }
                }
            });

        user.FirstName = accountInfoDto.FirstName!;
        user.LastName = accountInfoDto.LastName!;
        user.DateOfBirth = accountInfoDto.DateOfBirth;
        user.PhoneNumber = accountInfoDto.PhoneNumber!;

        var result = await userManager.UpdateAsync(user);

        if (result.Succeeded)
            return Ok(new ResponseBaseWithData<AccountInfoDto>
            {
                Data = mapper.Map<AccountInfoDto>(user),
                Status = HttpStatusCode.OK,
                Message = localizer.GetString("AccountUpdateSuccess")
            });

        return BadRequest(new ResponseBase
        {
            Status = HttpStatusCode.BadRequest,
            Message = localizer.GetString("AccountUpdateFail"),
            Errors = (Dictionary<string, string[]>)result.Errors.Select(x =>
                new KeyValuePair<string, string[]>
                (
                    x.Code,
                    [localizer.GetString("AccountUpdateFail"),x.Description]
                )
            )
        });
    }

    #endregion

    #region Avatar

    [HttpGet(nameof(avatar))]
    public async Task<IActionResult> GetAvatar([FromForm] IFormFile? avatar)
    {
        var userId = HttpContext.User.Claims.First(x => x.Type.Equals(ClaimTypes.PrimarySid)).Value;

        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            return BadRequest(new ResponseBase
            {
                Errors = new Dictionary<string, string[]> { { "NotFound", [localizer.GetString("AccountNotFound")] } }
            });
        return Ok(new ResponseBaseWithData<string>
        {
            Data = user.Avatar,
            Status = HttpStatusCode.OK,
            Message = localizer.GetString("AvatarFound")
        });
    }

    [HttpPut(nameof(avatar))]
    public async Task<IActionResult> UpdateAvatar([FromForm] IFormFile? avatar)
    {
        var userId = HttpContext.User.Claims.First(x => x.Type.Equals(ClaimTypes.PrimarySid)).Value;

        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            return BadRequest(new ResponseBase
            {
                Errors = new Dictionary<string, string[]> { { "NotFound", [localizer.GetString("AccountNotFound")] } }
            });

        var imagePath = ImageConstants.DefaultAvatarImage;
        if (avatar is { Length: > 0 })
        {
            if (!fileService.IsImageAccept(avatar.FileName))
                return BadRequest(new ResponseBase
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = localizer.GetString("AvatarExtensionNotAllow"),
                    Errors = new Dictionary<string, string[]>
                    {
                        { "Image", [localizer.GetString("AvatarExtensionNotAllow")] }
                    }
                });

            imagePath = await fileService.SaveFile(avatar, FolderPath, user.Id.ToString());
        }

        user.Avatar = imagePath;
        var result = await userManager.UpdateAsync(user);

        if (result.Succeeded)
            return Ok(new ResponseBaseWithData<AccountInfoDto>
            {
                Data = mapper.Map<AccountInfoDto>(user),
                Status = HttpStatusCode.OK,
                Message = localizer.GetString("AvatarUpdateSuccess")
            });

        return BadRequest(new ResponseBase
        {
            Status = HttpStatusCode.BadRequest,
            Message = localizer.GetString("AvatarUpdateFail"),
            Errors = (Dictionary<string, string[]>)result.Errors.Select(x =>
                new KeyValuePair<string, string[]>
                (
                    x.Code,
                    [localizer.GetString("AvatarUpdateFail"),x.Description]
                )
            )
        });
    }

    #endregion

    #region Security

    [HttpPut(nameof(ChangeEmail))]
    public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailApiDto model)
    {
        var user = await userManager.FindByEmailAsync(model.CurrentEmail);
        if (user == null)
            return BadRequest(new ResponseBase
            {
                Errors = new Dictionary<string, string[]>
                {
                    {
                        "Email", [localizer.GetString("EmailNotFound")]
                    }
                }
            });

        user.EmailConfirmed = false;

        var verifyCode = await userManager.GenerateChangeEmailTokenAsync(user, model.NewEmail);

        var uriBuilder = new UriBuilder(model.ClientUrl);
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);

        query[nameof(ChangeEmailConfirmDto.UserId)] = user.Id.ToString();
        query[nameof(ChangeEmailConfirmDto.VerifyCode)] = verifyCode;
        query[nameof(ChangeEmailConfirmDto.Email)] = model.NewEmail;

        query[$"{nameof(ChangeEmailConfirmDto)}.{nameof(ChangeEmailConfirmDto.UserId)}"] = user.Id.ToString();
        query[$"{nameof(ChangeEmailConfirmDto)}.{nameof(ChangeEmailConfirmDto.VerifyCode)}"] = verifyCode;
        query[$"{nameof(ChangeEmailConfirmDto)}.{nameof(ChangeEmailConfirmDto.Email)}"] = model.NewEmail;

        uriBuilder.Query = query.ToString();
        var uri = uriBuilder.ToString();

        var bodyBuilder = new StringBuilder();
        bodyBuilder.AppendLine($"<h2>Xin Chào {user.FullName}!</h2>");
        bodyBuilder.AppendLine("<p>Gần đây bạn đã gửi yêu cầu thay đổi email từ chúng tôi!</p>");
        bodyBuilder.AppendLine($"<p>Địa chỉ email cũ: <strong>{model.CurrentEmail}</strong></p>");
        bodyBuilder.AppendLine($"<p>Địa chỉ email mới: <strong>{model.NewEmail}</strong></p>");
        bodyBuilder.AppendLine($"<p>Nếu bạn là người yêu cầu, hãy bấm vào " +
                               $"<a href=\"{uri}\">đây</a>" +
                               $" để cập nhật email mới. Link có tác dụng trong 2 giờ</p>");
        bodyBuilder.AppendLine("<p>Nếu không, xin hãy bỏ qua thư này!</p>");
        bodyBuilder.AppendLine("<h3>Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi!</h3>");

        var mailContent = new MailContent
        {
            Subject = "Thư xác thực thay đổi email",
            HtmlBody = bodyBuilder.ToString(),
            To = model.CurrentEmail
        };

        try
        {
            await mailService.SendMail(mailContent);

            return Ok(new ResponseBase
            {
                Message = localizer.GetString("EmailSendSuccess"),
                Status = HttpStatusCode.OK
            });
        }
        catch (Exception e)
        {
            return BadRequest(new ResponseBase
            {
                Message = e.Message,
                Status = HttpStatusCode.BadRequest,
                Errors = new Dictionary<string, string[]>
                {
                    {
                        "SendMail", [localizer.GetString("EmailSendFail"), e.Message]
                    }
                }
            });
        }
    }


    [HttpPut(nameof(ChangeEmailConfirm))]
    public async Task<IActionResult> ChangeEmailConfirm([FromBody] ChangeEmailConfirmApiDto model)
    {
        var userIdFromToken = HttpContext.User.Claims.First(x => x.Type.Equals(ClaimTypes.PrimarySid)).Value;

        if (userIdFromToken != model.UserId)
            return Ok(new ResponseBase
            {
                Errors = new Dictionary<string, string[]>
                {
                    { "FakeRequest", [localizer.GetString("FakeRequest")] }
                },
                Status = HttpStatusCode.BadRequest,
                Type = HttpResponseType.BadRequest
            });
        var user = await userManager.FindByIdAsync(model.UserId);

        var result = await userManager.ChangeEmailAsync(user!, model.Email, model.VerifyCode);
        if (result.Succeeded)
        {
            cacheService.Remove(HttpContext.User.Claims.First(x => x.Type.Equals(ClaimTypes.Sid)).Value,
                CacheConstant.AccessToken);
            return Ok(new ResponseBase
            {
                Message = localizer.GetString("ChangeEmailSuccess")
            });
        }

        return BadRequest(new ResponseBase
        {
            Status = HttpStatusCode.BadRequest,
            Type = HttpResponseType.BadRequest,
            Errors = result.Errors
                .Select(x => new KeyValuePair<string, string[]>(
                    x.Code, [localizer.GetString("VerifyCodeExpired"), x.Description]))
                .ToDictionary()
        });
    }

    [HttpPut(nameof(ChangePassword))]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto model)
    {
        var userId = HttpContext.User.Claims.First(x => x.Type.Equals(ClaimTypes.PrimarySid)).Value;

        var user = await userManager.FindByIdAsync(userId);

        if (user == null)
            return BadRequest(new ResponseBase
            {
                Errors = new Dictionary<string, string[]>
                {
                    {
                        "NotFound", [localizer.GetString("AccountNotFound")]
                    }
                }
            });

        var changeResult = await userManager.ChangePasswordAsync(user, model.Password, model.NewPassword);

        if (changeResult.Succeeded)
            return Ok(new ResponseBase
            {
                Message = localizer.GetString("ChangePasswordSuccess"),
                Status = HttpStatusCode.OK,
                Type = HttpResponseType.Ok
            });
        return BadRequest(new ResponseBase
        {
            Errors = changeResult.Errors
                .Select(x =>
                    new KeyValuePair<string, string[]>(x.Code, [localizer.GetString("ChangePasswordFail"), x.Description])
                )
                .ToDictionary(),
            Message = localizer.GetString("ChangePasswordFail"),
            Status = HttpStatusCode.BadRequest,
            Type = HttpResponseType.BadRequest
        });
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteAccount()
    {
        var userId = HttpContext.User.Claims.First(x => x.Type.Equals(ClaimTypes.PrimarySid)).Value;
        var user = await userManager.FindByIdAsync(userId);

        if (user == null)
            return BadRequest(new ResponseBase
            {
                Errors = new Dictionary<string, string[]>
                {
                    {
                        "NotFound", [localizer.GetString("AccountNotFound")]
                    }
                }
            });

        user.IsDeleted = true;
        var result = await userManager.UpdateAsync(user);

        if (result.Succeeded)
            return Ok(new ResponseBase
            {
                Message = localizer.GetString("DeleteAccountSuccess")
            });
        return BadRequest(new ResponseBase
        {
            Errors = result.Errors
                .Select(x =>
                    new KeyValuePair<string, string[]>(x.Code, [localizer.GetString("DeleteAccountFail"), x.Description])
                )
                .ToDictionary(),
            Message = localizer.GetString("DeleteAccountFail"),
            Status = HttpStatusCode.BadRequest,
            Type = HttpResponseType.BadRequest
        });
    }

    #endregion
}