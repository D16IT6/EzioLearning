using EzioLearning.Share.Dto.Learning.Course;

namespace EzioLearning.Core.Dto.Learning.Course
{
    public class CourseSectionCreateApiDto : CourseSectionCreateDto
    {
        public List<CourseLectureCreateApiDto> CourseLectures { get; set; } = new();
    }
}
