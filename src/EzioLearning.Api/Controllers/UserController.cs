using System.Net;
using AutoMapper;
using EzioLearning.Api.Services;
using EzioLearning.Api.Utils;
using EzioLearning.Core.Dto.Auth;
using EzioLearning.Domain.Entities.Identity;
using EzioLearning.Share.Models.Response;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace EzioLearning.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController(
    IMapper mapper,
    UserManager<AppUser> userManager,
    FileService fileService,
    PermissionService permissionService,
    RoleManager<AppRole> roleManager, IStringLocalizer<UserController> localizer) : ControllerBase
{
    public static readonly string FolderPath = "Uploads/Images/Users/";
    
    [HttpPost]
    public async Task<IActionResult> CreateNewUser([FromForm] UserCreateApiDto model)
    {
        var newUser = mapper.Map<AppUser>(model);
        var image = model.Avatar;

        newUser.Id = Guid.NewGuid();

        var imagePath = ImageConstants.DefaultAvatarImage;

        if (image is { Length: > 0 })
        {
            if (!fileService.IsImageAccept(image.FileName))
                return BadRequest(new ResponseBase
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = localizer.GetString("ImageExtensionNotAllow")
                });

            imagePath = await fileService.SaveFile(image, FolderPath, newUser.Id.ToString());
        }

        newUser.Avatar = imagePath;


        var result = await userManager.CreateAsync(newUser, model.Password!);

        var roleList = await roleManager.Roles.Where(x => model.RoleIds.Contains(x.Id)).ToListAsync();
        var addToRoleResult = await userManager.AddToRolesAsync(newUser, roleList.Select(x => x.Name)!);

        if (!addToRoleResult.Succeeded)
            return BadRequest(new ResponseBaseWithList<IdentityError>
            {
                Status = HttpStatusCode.BadRequest,
                Errors = new Dictionary<string, string[]>()
                    {
                    {
                        "UserAddRoleFail", [localizer.GetString("UserAddRoleFail")]
                    }},
                Message = localizer.GetString("UserAddRoleFail")
            });

        await permissionService.AddPermissionsForNewUser(newUser);

        if (result.Succeeded)
            return Ok(new ResponseBase
            {
                Status = HttpStatusCode.OK,
                Message = localizer.GetString("UserCreateSuccess")
            });

        return BadRequest(new ResponseBaseWithList<IdentityError>
        {
            Status = HttpStatusCode.BadRequest,
            Data = result.Errors,
            Message = localizer.GetString("UserCreateFail")
        });
    }
}