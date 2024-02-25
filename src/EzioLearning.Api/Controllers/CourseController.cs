using EzioLearning.Core.Dtos.Learning.Course;
using EzioLearning.Core.Models.Response;
using EzioLearning.Core.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using AutoMapper;
using EzioLearning.Core.SeedWorks;
using EzioLearning.Domain.Entities.Learning;
using EzioLearning.Api.Services;
using EzioLearning.Api.Utils;
using EzioLearning.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EzioLearning.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController(ICourseRepository courseRepository,IMapper mapper,IUnitOfWork unitOfWork,FileService fileService, ICourseCategoryRepository courseCategoryRepository,UserManager<AppUser> userManager) : ControllerBase
    {
        public static readonly string FolderPath = "Uploads/Images/Courses/";

        [HttpGet("Count")]
        public async Task<IActionResult> CountCourse()
        {
            var count = await courseRepository.CountCourses();
            return Ok(new ResponseBaseWithData<int>()
            {
                Status = HttpStatusCode.OK,
                Message = "Đếm số khoá học hiện có thành công",
                Data = count
            });
        }

        [HttpGet("Feature/{take:int?}")]
        public async Task<IActionResult> GetFeaturedCourses([FromRoute]int take = 12)
        {
            var data = await courseRepository.GetAllAsync();
            data = data.OrderByDescending(x => x.Students.Count).ThenByDescending(x => x.CreatedDate)
                .Take(take);

            var resultData = mapper.Map<List<Course>,List<CourseViewDto>>(data.ToList());

            await UpdateCourseViewDto(resultData);

            return Ok(new ResponseBaseWithList<CourseViewDto>()
            {
                Status = HttpStatusCode.OK,
                Message = "Lấy danh sách khoá học thịnh hành thành công",
                Data = resultData
            });
        }
        [HttpPost]
        public async Task<IActionResult> CreateNewCourse([FromForm] CourseCreateDto model)
        {
            var newCourse = mapper.Map<Course>(model);

            newCourse.Id = Guid.NewGuid();

            var imagePath = ImageConstants.DefaultCoursePoster;

            var poster = model.Poster;
            if (poster is { Length: > 0 })
            {
                if (!fileService.IsImageAccept(poster.FileName))
                {
                    return BadRequest(new ResponseBase()
                    {
                        Status = HttpStatusCode.BadRequest,
                        Message = "Ảnh đầu vào không hợp lệ, vui lòng chọn định dạng khác"
                    });
                }

                imagePath = await fileService.SaveFile(poster, FolderPath, newCourse.Id.ToString());
            }

            newCourse.Poster = imagePath;


            newCourse.Categories = (ICollection<CourseCategory>)await GetInsertCourseCategories(model);

            courseRepository.Add(newCourse);

            var result = await unitOfWork.CompleteAsync();
            if (result > 0)
                return Ok(new ResponseBase()
                {
                    Message = "Thêm mới khoá học thành công",
                    Status = HttpStatusCode.OK
                });
            return BadRequest(new ResponseBase()
            {
                Message = "Thêm mới khoá học thất bại, vui lòng thử lại",
                Status = HttpStatusCode.BadRequest
            });
        }

        private async Task UpdateCourseViewDto(IEnumerable<CourseViewDto> listCourseViewDtos)
        {
            foreach (var courseViewDto in listCourseViewDtos)
            {
                var trainer = await userManager.FindByIdAsync(courseViewDto.CreatedBy.ToString());
                if (trainer != null)
                {
                    courseViewDto.TeacherName = trainer.FullName;
                    courseViewDto.TeacherAvatar = trainer.Avatar;
                }
                
            }
        }
        private async Task<IEnumerable<CourseCategory>> GetInsertCourseCategories(
            CourseCreateDto courseCreateDto)
        {
            var result = new List<CourseCategory>();
            foreach (var courseCategoryId in courseCreateDto.CourseCategoryIds)
            {
                var insertItem = await courseCategoryRepository.GetByIdAsync(courseCategoryId);
                if (insertItem != null)
                    result.Add(insertItem);
            }
            return result;
        }
    }
}
