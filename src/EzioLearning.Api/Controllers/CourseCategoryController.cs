using System.Globalization;
using AutoMapper;
using EzioLearning.Api.Services;
using EzioLearning.Api.Utils;
using EzioLearning.Core.Repositories.Learning;
using EzioLearning.Core.SeedWorks;
using EzioLearning.Domain.Entities.Learning;
using EzioLearning.Share.Dto.Learning.CourseCategory;
using EzioLearning.Share.Models.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Net;
using EzioLearning.Core.Dto.Learning.CourseCategory;
using EzioLearning.Core.Repositories.System;
using EzioLearning.Domain.Entities.Translation;
using EzioLearning.Core.Dto.Translation;

namespace EzioLearning.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CourseCategoryController(
    IMapper mapper,
    ICourseCategoryRepository categoryRepository,
    ICultureRepository cultureRepository,
    IUnitOfWork unitOfWork,
    FileService fileService, IStringLocalizer<CourseCategoryController> localizer) : ControllerBase
{
    private static readonly string FolderPath = "Uploads/Images/CourseCategories/";

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var currentCulture = CultureInfo.CurrentCulture.Name;
        var data =
            await categoryRepository
                .GetAllWithDto<CourseCategoryViewDto>(
                    x => x.IsActive && x.CourseCategoryTranslations.Select(x=>x.CultureId).Contains(CultureInfo.CurrentCulture.Name),
                    [nameof(CourseCategory.CourseCategoryTranslations)]
                    );

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
            (await categoryRepository.GetAllWithDto<TopCourseCategoryDto>())
            .OrderBy(x => x.Name == CultureInfo.CurrentCulture.Name).Take(count);

        return Ok(new ResponseBaseWithList<TopCourseCategoryDto>
        {
            Data = data.ToList(),
            Message = localizer.GetString("CourseCategoryTopGetSuccess"),
            Status = HttpStatusCode.OK
        });
    }


    [HttpPut]
    //[VerifyToken]
    public async Task<IActionResult> Create([FromForm] CourseCategoryCreateApiDto courseCategoryCreateDto)
    {
        var newCourseCategory = mapper.Map<CourseCategory>(courseCategoryCreateDto);
        newCourseCategory.Id = Guid.NewGuid();

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

            imagePath = await fileService.SaveFile(image, FolderPath, newCourseCategory.Id.ToString());
        }


        newCourseCategory.Image = imagePath;


        var courseCategoryTranslation = new CourseCategoryTranslation()
        {
            Culture = (await cultureRepository.Find(x=> x.Id.Equals(CultureInfo.CurrentCulture.Name))).First(),
            Name = courseCategoryCreateDto.Name,
            CourseCategoryId = newCourseCategory.Id,
        };
        newCourseCategory.CourseCategoryTranslations.Add(courseCategoryTranslation);

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
            Message = result.ToString(),
            Errors = new Dictionary<string, string[]>()
            { { "CourseCategoryCreateFail",[localizer.GetString("CourseCategoryCreateFail")] }}
        });
    }
    
    [HttpPut("Translation")]
    //[VerifyToken]
    public async Task<IActionResult> CreateTranslation([FromForm] CourseCategoryTranslationCreateApiDto model)
    {
        var currentCourseCategory = (await categoryRepository.Find(x => x.Id == model.Id)).FirstOrDefault();

        if (currentCourseCategory == null || currentCourseCategory.IsActive == false)
        {
            return BadRequest(new ResponseBase()
            {
                Message = localizer.GetString("NotFoundCourseCategory"),
                Status = HttpStatusCode.BadRequest,
                Errors = new Dictionary<string, string[]>()
                {
                    { "NotFoundCourseCategory", [localizer.GetString("NotFoundCourseCategory")] }
                }
            });
        }
        var courseCategoryTranslation = new CourseCategoryTranslation()
        {
            Culture = (await cultureRepository.Find(x=> x.Id.Equals(model.Culture))).First(),
            Name = model.Name!,
            CourseCategoryId = currentCourseCategory.Id,
        };
        currentCourseCategory.CourseCategoryTranslations.Add(courseCategoryTranslation);

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
            Message = localizer.GetString("CourseCategoryCreateFail"),
            Errors = new Dictionary<string, string[]>()
                { { "CourseCategoryCreateFail",[localizer.GetString("CourseCategoryCreateFail")] }}
        });
    }
}