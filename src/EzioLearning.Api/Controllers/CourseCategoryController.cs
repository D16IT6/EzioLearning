using System.Net;
using AutoMapper;
using EzioLearning.Api.Filters;
using EzioLearning.Api.Services;
using EzioLearning.Api.Utils;
using EzioLearning.Core.Dtos.Learning.CourseCategory;
using EzioLearning.Core.Models.Response;
using EzioLearning.Core.Repositories;
using EzioLearning.Core.SeedWorks;
using EzioLearning.Domain.Entities.Learning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace EzioLearning.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CourseCategoryController(IMapper mapper, ICourseCategoryRepository categoryRepository, IUnitOfWork unitOfWork, FileService fileService) : ControllerBase
	{
		private static readonly string FolderPath = "Uploads/Images/CourseCategories/";

		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> GetAll()
		{
			var data = await categoryRepository.GetAllWithDto();
			return Ok(new ResponseBaseWithList<CourseCategoryViewDto>()
			{
				Data = data,
				Message = "Lấy dữ liệu thành công",
				StatusCode = HttpStatusCode.OK
			});
		}

		[HttpPut]
		[ValidateModel]
		[VerifyToken]
		public async Task<IActionResult> Create([FromForm] CourseCategoryCreateDto courseCategoryCreateDto)
		{

			var newCourseCategory = mapper.Map<CourseCategory>(courseCategoryCreateDto);

			var image = courseCategoryCreateDto.Image;

			var imagePath = ImageConstants.DefaultCourseCategoryImage;

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

				var tempFilePath = Path.Combine(FolderPath, newCourseCategory.Name + Path.GetExtension(image.FileName));

				var actuallyFilePath = fileService.GenerateActuallyFilePath(Path.Combine(Environment.CurrentDirectory, "wwwroot", tempFilePath));

				await using var fileStream = new FileStream(actuallyFilePath, FileMode.OpenOrCreate);

				await image.CopyToAsync(fileStream);

				imagePath = Path.Combine(FolderPath, Path.GetFileName(actuallyFilePath));
			}
			newCourseCategory.Id = Guid.NewGuid();

			newCourseCategory.Image = imagePath;

			categoryRepository.Add(newCourseCategory);

			var result = await unitOfWork.CompleteAsync();

			if (result > 0) return Ok(
				new ResponseBase()
				{
					StatusCode = HttpStatusCode.OK,
					Message = "Tạo mới danh mục thành công"
				});
			return BadRequest(new ResponseBase()
			{
				StatusCode = HttpStatusCode.BadRequest,
				Message = result.ToString()
			});
		}
	}
}