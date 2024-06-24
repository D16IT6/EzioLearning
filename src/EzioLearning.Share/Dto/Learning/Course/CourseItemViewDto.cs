using EzioLearning.Share.Utils;

namespace EzioLearning.Share.Dto.Learning.Course
{
	public class CourseItemViewDto
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public string? Poster { get; set; } = string.Empty;
		public double Price { get; set; } = 0;
		public double PromotionPrice { get; set; } = 0;
		public CourseLevel Level { get; set; }
		public Guid TeacherId { get; set; }
		public string? TeacherName { get; set; }
		public string? TeacherAvatar { get; set; }
		public int LessonCount { get; set; }
		public double Rating { get; set; } = 0;
		public int RatingCount { get; set; } = 0;
		public long Duration { get; set; } = 0;
	}
}
