namespace EzioLearning.Share.Dto.Learning.Course
{
    public class CourseLectureUpdateDto : CourseLectureViewDto
    {
        public int SortOrder { get; set; }
        public bool Deleted { get; set; }
    }
}
