using System.Net;
using AutoMapper;
using EzioLearning.Api.Filters;
using EzioLearning.Api.Services;
using EzioLearning.Api.Utils;
using EzioLearning.Core.Repositories;
using EzioLearning.Core.SeedWorks;
using EzioLearning.Domain.Entities.Learning;
using EzioLearning.Share.Dto.Learning.CourseCategory;
using EzioLearning.Share.Models.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using CourseCategoryCreateDto = EzioLearning.Core.Dto.Learning.CourseCategory.CourseCategoryCreateDto;

namespace EzioLearning.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CourseCategoryController(
    IMapper mapper,
    ICourseCategoryRepository categoryRepository,
    IUnitOfWork unitOfWork,
    FileService fileService, IStringLocalizer<CourseCategoryController> localizer) : ControllerBase
{
    private static readonly string FolderPath = "Uploads/Images/CourseCategories/";

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var data =
            await categoryRepository.GetAllWithDto<CourseCategoryViewDto>(x => x.IsActive);

        return Ok(new ResponseBaseWithList<CourseCategoryViewDto>
        {
            Data = data.ToList(),
            Message = localizer.GetString("CourseCategoryGetSuccess"),
            Status = HttpStatusCode.OK
        });
    }

    [HttpGet("Top/{count}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetTopCategories([FromRoute] int count)
    {
        var data =
            await categoryRepository.GetAllAsync();
        data = data
            .OrderByDescending(x => x.Courses.Count)
            .ThenBy(x => x.Name)
            .Take(count);

        var responseData = data.Select(x => new TopCourseCategoryDto
        {
            Name = x.Name,
            Image = x.Image
        });


        return Ok(new ResponseBaseWithList<TopCourseCategoryDto>
        {
            Data = responseData.ToList(),
            Message = localizer.GetString("CourseCategoryTopGetSuccess"),
            Status = HttpStatusCode.OK
        });
    }


    [HttpPut]
    [VerifyToken]
    public async Task<IActionResult> Create([FromForm] CourseCategoryCreateDto courseCategoryCreateDto)
    {
        var newCourseCategory = mapper.Map<CourseCategory>(courseCategoryCreateDto);

        var image = courseCategoryCreateDto.Image;

        var imagePath = ImageConstants.DefaultCourseCategoryImage;

        if (image is { Length: > 0 })
        {
            if (!fileService.IsImageAccept(image.FileName))
                return BadRequest(new ResponseBase
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = localizer.GetString("ImageExtensionNotAllow"),
                    Errors = new Dictionary<string, string[]>()
                    {
                        {"ImageExtensionNotAllow", [localizer.GetString("ImageExtensionNotAllow")]}
                    }
                });

            imagePath = await fileService.SaveFile(image, FolderPath, newCourseCategory.Name);
        }

        newCourseCategory.Id = Guid.NewGuid();

        newCourseCategory.Image = imagePath;

        categoryRepository.Add(newCourseCategory);

        var result = await unitOfWork.CompleteAsync();

        if (result > 0)
            return Ok(
                new ResponseBase
                {
                    Status = HttpStatusCode.OK,
                    Message = localizer.GetString("CourseCategoryCreateSuccess")
                });
        return BadRequest(new ResponseBase
        {
            Status = HttpStatusCode.BadRequest,
            Message = result.ToString()
        });
    }
}