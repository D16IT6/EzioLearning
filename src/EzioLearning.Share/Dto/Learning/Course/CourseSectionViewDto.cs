namespace EzioLearning.Share.Dto.Learning.Course
{
    public class CourseSectionViewDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<CourseLectureViewDto> Lectures { get; set; } = [];
    }
}
