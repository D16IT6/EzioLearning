using EzioLearning.Share.Utils;

namespace EzioLearning.Share.Dto.Learning.Course
{
    public class CourseLectureViewDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public CourseLectureType LectureType { get; set; }

        public string FileUrl { get; set; } = string.Empty;

        public bool IsPreview { get; set; }
        public long Duration { get; set; }

        public Guid CourseSectionId { get; set; }
    }
}
