using AutoMapper;
using EzioLearning.Api.Filters;
using EzioLearning.Api.Models.Response;
using EzioLearning.Core.Dtos.Learning.CourseCategory;
using EzioLearning.Core.Repositories;
using EzioLearning.Core.SeedWorks;
using EzioLearning.Domain.Entities.Learning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace EzioLearning.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [VerifyToken]
    public class CourseCategoryController(IMapper mapper, ICourseCategoryRepository categoryRepository, IUnitOfWork unitOfWork) : ControllerBase
    {

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var data = await categoryRepository.GetAllAsync();
            return Ok(new ResponseBase()
            {
                Data = data,
                Message = "Lấy dữ liệu thành công",
                StatusCode = 200
            });
        }

        [HttpPut]
        [ValidateModel]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CourseCategoryCreateDto courseCategoryCreateDto)
        {
            var user = HttpContext.User;
            var newCourseCategory = mapper.Map<CourseCategory>(courseCategoryCreateDto);
            newCourseCategory.Id = Guid.NewGuid();

            categoryRepository.Add(newCourseCategory);

            var result = await unitOfWork.CompleteAsync();

            if (result > 0) return Ok(
                new ResponseBase()
                {
                    StatusCode = 200,
                    Message = "Tạo mới danh mục thành công"
                });
            return BadRequest(new ResponseBase()
            {
                StatusCode = 400,
                Message = result.ToString()
            });
        }
    }
}