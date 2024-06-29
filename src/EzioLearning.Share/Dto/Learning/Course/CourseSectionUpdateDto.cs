namespace EzioLearning.Share.Dto.Learning.Course
{
    public class CourseSectionUpdateDto : CourseSectionViewDto
    {
        public bool Deleted { get; set; }
        public new List<CourseLectureUpdateDto> Lectures { get; set; } = [];

    }
}
