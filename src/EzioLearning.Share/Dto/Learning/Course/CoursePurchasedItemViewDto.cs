using EzioLearning.Share.Utils;

namespace EzioLearning.Share.Dto.Learning.Course
{
    public class CoursePurchasedItemViewDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Poster { get; set; } = string.Empty;
        public CourseLevel Level { get; set; }
        public int LessonCount { get; set; }
        public long Duration { get; set; } = 0;
    }
}
