using System.Linq.Expressions;
using System.Net;
using System.Security.Claims;
using AutoMapper;
using EzioLearning.Api.Filters;
using EzioLearning.Api.Services;
using EzioLearning.Api.Utils;
using EzioLearning.Core.Dto.Learning.Course;
using EzioLearning.Core.Repositories.Learning;
using EzioLearning.Core.Repositories.Resources;
using EzioLearning.Core.SeedWorks;
using EzioLearning.Domain.Entities.Identity;
using EzioLearning.Domain.Entities.Learning;
using EzioLearning.Domain.Entities.Resources;
using EzioLearning.Share.Common;
using EzioLearning.Share.Dto.Learning.Course;
using EzioLearning.Share.Dto.User;
using EzioLearning.Share.Models.Pages;
using EzioLearning.Share.Models.Request;
using EzioLearning.Share.Models.Response;
using EzioLearning.Share.Utils;
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
    VideoService videoService,
    ICourseCategoryRepository courseCategoryRepository,
    IVideoRepository videoRepository,
    IDocumentRepository documentRepository,
    UserManager<AppUser> userManager, IStringLocalizer<CourseController> localizer) : ControllerBase
{
    private static readonly string ImageFolderPath = "Uploads/Images/Courses/";
    private static readonly string VideoFolderPath = "Uploads/Videos/Courses/";
    private static readonly string DocumentFolderPath = "Uploads/Documents/Courses/";


    [HttpGet]
    public async Task<IActionResult> GetCourse([FromQuery] CourseListOptions options)
    {
        var pagedResult = await courseRepository.GetPageWithDto<CourseItemViewDto>(
            expression: c=> ((c.Price > 0 && options.PriceType == PriceType.Paid )
                            || (c.Price >= 0 && options.PriceType == PriceType.All)
                            || (c.Price == 0 && options.PriceType == PriceType.Free))
                            && (options.SearchText == null || c.Name.Contains(options.SearchText))
                            && (!options.CourseCategoryIds.Any() ||
                            options.CourseCategoryIds.Except(c.Categories.Select(x=> x.Id).ToList()).Count() < options.CourseCategoryIds.Count)
            ,
            pageNumber: options.PageNumber,
            pageSize: options.PageSize);


        return Ok(new ResponseBaseWithData<PageResult<CourseItemViewDto>>()
        {
            Data = pagedResult,
            Message = "Lấy danh sách thành công"
        });
    }

    [HttpGet("Detail")]
    public async Task<IActionResult> GetCourseDetail([FromQuery] Guid courseId)
    {
        var course = await courseRepository.GetCourseDetail(courseId);

        var courseDetailViewDto = mapper.Map<CourseDetailViewDto>(course);
        return Ok(new ResponseBaseWithData<CourseDetailViewDto>()
        {
            Data = courseDetailViewDto
        });
    }

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

            imagePath = await fileService.SaveFile(poster, ImageFolderPath, newCourse.Id.ToString());
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

    [HttpPost("Section")]
    [Authorize(Permissions.Courses.Create)]
    [VerifyToken]
    [RequestSizeLimit(long.MaxValue)]
    public async Task<IActionResult> CreateNewCourseSection([FromForm] CourseSectionCreateApiDto courseSectionCreateDto)
    {
        var userId = User.Claims.First(x => x.Type.Equals(ClaimTypes.Sid)).Value;

        courseSectionCreateDto.Id = Guid.NewGuid();

        var courseSection = mapper.Map<CourseSection>(courseSectionCreateDto);

        courseSection.CourseLectures.Clear();

        foreach (var courseLectureCreateDto in courseSectionCreateDto.CourseLectures)
        {
            courseLectureCreateDto.Id = Guid.NewGuid();

            var courseLecture = mapper.Map<CourseLecture>(courseLectureCreateDto);

            var file = courseLectureCreateDto.File;


            var saveFolder = courseLectureCreateDto.LectureType == CourseLectureType.Document ? DocumentFolderPath : VideoFolderPath;
            saveFolder = Path.Combine(saveFolder, userId);

            //Relative path
            var outputFilePath = await fileService.SaveFile(file, saveFolder, Guid.NewGuid().ToString());


            if (courseLectureCreateDto.LectureType == CourseLectureType.Video)
            {
                var video = new Video()
                {
                    Id = Guid.NewGuid(),
                    DefaultPath = outputFilePath,
                    Duration = await videoService.GetDurationFromVideo(outputFilePath),
                    Name = Path.GetFileName(outputFilePath),
                    Status = VideoStatus.Pending,
                };
                courseLecture.LectureType = CourseLectureType.Video;
                courseLecture.Video = video;
            }
            else
            {
                var document = new Document()
                {
                    Id = Guid.NewGuid(),
                    Name = Path.GetFileName(file.FileName),
                    Path = outputFilePath,
                };
                courseLecture.LectureType = CourseLectureType.Document;

                courseLecture.Document = document;
            }

            courseSection.CourseLectures.Add(courseLecture);
        }
        var addNewSectionResult = await courseRepository.AddNewSection(courseSection.CourseId, courseSection);


        var unitOfWorkResult = addNewSectionResult == 1 ? await unitOfWork.CompleteAsync() : addNewSectionResult;

        if (unitOfWorkResult > 0)
        {
            return Ok(new ResponseBaseWithData<CourseSectionCreateDto>()
            {
                Data = courseSectionCreateDto,
                Message = "Tạo phần khoá học thành công",
                Status = HttpStatusCode.OK
            });

        }
        return BadRequest(new ResponseBase
        {
            Message = localizer.GetString("CourseSectionCreateFail"),
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