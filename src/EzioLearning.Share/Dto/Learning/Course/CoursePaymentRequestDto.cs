namespace EzioLearning.Share.Dto.Learning.Course
{
    public class CoursePaymentRequestDto
    {
        public Guid CourseId { get; set; }
        public Guid UserId { get; set; }
        public double Price { get; set; }
    }
}
