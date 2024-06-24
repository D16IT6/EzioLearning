using EzioLearning.Share.Utils;

namespace EzioLearning.Share.Dto.Learning.Course
{
    public class CourseDetailViewDto
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


        public Guid TeacherId { get; set; }
        public string TeacherName { get; set; } = string.Empty;
        public string TeacherAvatar { get; set; } = string.Empty;

        public double Rating { get; set; }
        public int RatingCount { get; set; }

        public int LessonCount { get; set; }

        public int Duration { get; set; }
        public int StudentCount { get; set; }

        public List<CourseSectionViewDto> Sections { get; set; } = [];

    }

    public class CourseSectionViewDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<CourseLectureViewDto> Lectures { get; set; } = [];
    }

    public class CourseLectureViewDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public CourseLectureType LectureType { get; set; }

        public string FileUrl { get; set; } = string.Empty;

        public bool IsPreview { get; set; }
        public long Duration { get; set; }
    }
}
