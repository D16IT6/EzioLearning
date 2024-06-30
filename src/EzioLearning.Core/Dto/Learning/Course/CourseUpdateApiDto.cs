using EzioLearning.Share.Dto.Learning.Course;
using EzioLearning.Share.Dto.Learning.CourseCategory;
using Microsoft.AspNetCore.Http;

namespace EzioLearning.Core.Dto.Learning.Course
{
    public class CourseUpdateApiDto : CourseUpdateDto
    {
        public IFormFile? NewPoster { get; set; }

        public new List<CourseCategoryViewDto> CourseCategories { get; set; } = [];
        public List<CourseSectionUpdateApiDto> Sections { get; set; } = [];
    }

    public class CourseSectionUpdateApiDto: CourseSectionUpdateDto
    {
        public new List<CourseLectureUpdateApiDto> Lectures { get; set; } = [];

    }

    public class CourseLectureUpdateApiDto : CourseLectureUpdateDto
    {
        public IFormFile? File { get; set; }
    }
}
