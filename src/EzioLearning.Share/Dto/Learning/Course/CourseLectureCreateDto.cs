using EzioLearning.Share.Utils;

namespace EzioLearning.Share.Dto.Learning.Course
{
    public class CourseLectureCreateDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public CourseLectureType LectureType { get; set; } = CourseLectureType.Video;
        public string? TempFileUrl { get; set; } = string.Empty;
        public Guid CourseSectionId { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
