using System.Net;
using System.Security.Claims;
using AutoMapper;
using EzioLearning.Api.Filters;
using EzioLearning.Api.Services;
using EzioLearning.Api.Utils;
using EzioLearning.Core.Dto.Learning.Course;
using EzioLearning.Core.Repositories.Learning;
using EzioLearning.Core.SeedWorks;
using EzioLearning.Domain.Entities.Identity;
using EzioLearning.Domain.Entities.Learning;
using EzioLearning.Share.Common;
using EzioLearning.Share.Dto.Learning.Course;
using EzioLearning.Share.Dto.User;
using EzioLearning.Share.Models.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace EzioLearning.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CourseController(
    ICourseRepository courseRepository,
    IMapper mapper,
    IUnitOfWork unitOfWork,
    FileService fileService,
    ICourseCategoryRepository courseCategoryRepository,
    UserManager<AppUser> userManager, IStringLocalizer<CourseController> localizer) : ControllerBase
{
    private static readonly string FolderPath = "Uploads/Images/Courses/";

    [HttpGet("Count")]
    public async Task<IActionResult> CountCourse()
    {
        var count = await courseRepository.CountCourses();
        return Ok(new ResponseBaseWithData<int>
        {
            Status = HttpStatusCode.OK,
            Message = localizer.GetString("CourseCountSuccess"),
            Data = count
        });
    }

    [HttpGet("FeaturedInstructor/{take:int?}")]
    public Task<IActionResult> GetFeatureInstructors([FromRoute] int take = 12)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(take);

        var userList = userManager.Users
            .OrderByDescending(x => x.Students.Count)
            .ThenByDescending(x => x.Courses.Count)
            .Take(take);
        return Task.FromResult<IActionResult>(Ok(new ResponseBaseWithList<InstructorViewDto>
        {
            Status = HttpStatusCode.OK,
            Message = localizer.GetString("FeatureInstructorsGetSuccess"),
            Data = mapper.ProjectTo<InstructorViewDto>(userList)
        }));
    }


    [HttpGet("Feature/{take:int?}")]
    public async Task<IActionResult> GetFeaturedCourses([FromRoute] int take = 12)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(take);
        var data = (await courseRepository.GetFeaturedCourses(take)).AsQueryable();

        return Ok(new ResponseBaseWithList<CourseViewDto>
        {
            Status = HttpStatusCode.OK,
            Message = localizer.GetString("CourseFeatureGetSuccess"),
            Data = mapper.ProjectTo<CourseViewDto>(data)
        });
    }

    [HttpPost]
    [Authorize(Permissions.Courses.Create)]
    [VerifyToken]
    public async Task<IActionResult> CreateNewCourse([FromForm] CourseCreateApiDto model)
    {
        var userId = User.Claims.First(x => x.Type.Equals(ClaimTypes.PrimarySid)).Value;

        model.CreatedBy = Guid.Parse(userId);
        model.Id = Guid.NewGuid();

        var newCourse = mapper.Map<Course>(model);

        var imagePath = ImageConstants.DefaultCoursePoster;

        var poster = model.Poster;
        if (poster is { Length: > 0 })
        {
            if (!fileService.IsImageAccept(poster.FileName))
                return BadRequest(new ResponseBase
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = localizer.GetString("ImageExtensionNotAllow")
                });

            imagePath = await fileService.SaveFile(poster, FolderPath, newCourse.Id.ToString());
        }

        newCourse.Poster = imagePath;

        newCourse.Categories = await GetInsertCourseCategories(model);

        courseRepository.Add(newCourse);

        var result = await unitOfWork.CompleteAsync();

        if (result > 0)
            return Ok(new ResponseBaseWithData<CourseCreateDto>()
            {
                Message = localizer.GetString("CourseCreateSuccess"),
                Status = HttpStatusCode.OK,
                Data = model
            });

        return BadRequest(new ResponseBase
        {
            Message = localizer.GetString("CourseCreateFail"),
            Status = HttpStatusCode.BadRequest
        });
    }

    private async Task<ICollection<CourseCategory>> GetInsertCourseCategories(CourseCreateApiDto courseCreateDto)
    {

        var result =
            await courseCategoryRepository
                .Find(x =>
                    courseCreateDto.CourseCategoryIds.Contains(x.Id)
                    );
        return result.ToList();

    }
}