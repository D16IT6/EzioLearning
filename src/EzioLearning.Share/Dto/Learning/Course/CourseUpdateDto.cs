using EzioLearning.Share.Dto.Learning.CourseCategory;
using EzioLearning.Share.Utils;

namespace EzioLearning.Share.Dto.Learning.Course
{
    public class CourseUpdateDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Content { get; set; } = string.Empty;
        public string? Poster { get; set; }
        public double Price { get; set; } = 0;
        public double PromotionPrice { get; set; }
        public CourseLevel Level { get; set; }
        public CourseStatus Status { get; set; }
        //public List<CourseSectionUpdateDto> Sections { get; set; } = [];

        public List<CourseCategoryViewDto> CourseCategories { get; set; } = [];
    }
}
