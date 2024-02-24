using AutoMapper;
using EzioLearning.Api.Filters;
using EzioLearning.Api.Services;
using EzioLearning.Api.Utils;
using EzioLearning.Core.Dtos.User;
using EzioLearning.Core.Models.Response;
using EzioLearning.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using EzioLearning.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace EzioLearning.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IMapper mapper, UserManager<AppUser> userManager, FileService fileService) : ControllerBase
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
            return Ok(new ResponseBaseWithList<InstructorViewDto>()
            {
                StatusCode = HttpStatusCode.OK,
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
                    Message = "Thêm user mới thành công"
                });

            return BadRequest(new ResponseBaseWithList<IdentityError>()
            {
                StatusCode = HttpStatusCode.BadRequest,
                Data = result.Errors.ToList(),
                Message = "Tạo tài khoản thất bại, vui lòng xem lỗi"
            });
        }
    }
}
