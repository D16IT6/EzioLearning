namespace EzioLearning.Share.Dto.Learning.Course
{
    public class CourseSectionCreateDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public Guid CourseId { get; set; }
    }
}
