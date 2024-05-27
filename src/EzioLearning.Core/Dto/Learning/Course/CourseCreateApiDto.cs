using EzioLearning.Share.Dto.Learning.Course;
using Microsoft.AspNetCore.Http;

namespace EzioLearning.Core.Dto.Learning.Course
{
    public class CourseCreateApiDto : CourseCreateDto
    {
        public IFormFile? Poster { get; set; }



    }
}
