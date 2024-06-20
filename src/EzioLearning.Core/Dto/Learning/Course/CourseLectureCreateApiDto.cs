using EzioLearning.Share.Dto.Learning.Course;
using Microsoft.AspNetCore.Http;

namespace EzioLearning.Core.Dto.Learning.Course
{
    public class CourseLectureCreateApiDto : CourseLectureCreateDto
    {
        public IFormFile File { get; set; } = default!;
    }
}
