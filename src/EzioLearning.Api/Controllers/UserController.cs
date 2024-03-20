using System.Net;
using AutoMapper;
using EzioLearning.Api.Filters;
using EzioLearning.Api.Services;
using EzioLearning.Api.Utils;
using EzioLearning.Domain.Entities.Identity;
using EzioLearning.Share.Dto.User;
using EzioLearning.Share.Models.Response;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EzioLearning.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController(
    IMapper mapper,
    UserManager<AppUser> userManager,
    FileService fileService,
    PermissionService permissionService,
    RoleManager<AppRole> roleManager) : ControllerBase
{
    public static readonly string FolderPath = "Uploads/Images/Users/";


    [HttpGet("FeaturedInstructor/{take:int?}")]
    public async Task<IActionResult> GetFeatureInstructors([FromRoute] int take = 12)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(take);

        var userList = userManager.Users
            .OrderByDescending(x => x.Students.Count)
            .ThenByDescending(x => x.Courses.Count)
            .Take(take);
        var data = mapper.ProjectTo<InstructorViewDto>(userList);
        return Ok(new ResponseBaseWithList<InstructorViewDto>
        {
            Status = HttpStatusCode.OK,
            Message = "Lấy danh sách giảng viên nổi bật thành công",
            Data = await data.ToListAsync()
        });
    }

    [HttpPost]
    [ValidateModel]
    public async Task<IActionResult> CreateNewUser([FromForm] UserCreateDto model)
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
                    Message = "Ảnh đầu vào không hợp lệ, vui lòng chọn định dạng khác"
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
                Data = addToRoleResult.Errors.ToList(),
                Message = "Thêm quyền vào tài khoản thất bại, vui lòng xem lỗi"
            });

        await permissionService.AddPermissionsForNewUser(newUser);

        if (result.Succeeded)
            return Ok(new ResponseBase
            {
                Status = HttpStatusCode.OK,
                Message = "Thêm user mới thành công"
            });

        return BadRequest(new ResponseBaseWithList<IdentityError>
        {
            Status = HttpStatusCode.BadRequest,
            Data = result.Errors.ToList(),
            Message = "Tạo tài khoản thất bại, vui lòng xem lỗi"
        });
    }
}