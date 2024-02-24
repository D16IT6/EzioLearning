
using System.ComponentModel.DataAnnotations.Schema;
using EzioLearning.Domain.Common;
using EzioLearning.Domain.Entities.Identity;

namespace EzioLearning.Domain.Entities.Learning
{
    [Table(name: "Courses", Schema = "Learning")]
    public class Course : AuditableEntity
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public string? Poster { get; set; }
        public string? Content { get; set; }
        public required double Price { get; set; } = 0;
        public required double PromotionPrice { get; set; }
        public int SortOrder { get; set; }
        public CourseLevel Level { get; set; }
        public CourseStatus Status { get; set; }

        public ICollection<CourseCategory> Categories { get; set; } = new List<CourseCategory>();
        public ICollection<CourseRating> Ratings { get; set; } = new List<CourseRating>();
        public ICollection<CourseLesson> Lessons { get; set; } = new List<CourseLesson>();
        public ICollection<Student> Students { get; set; } = new List<Student>();

        [ForeignKey(nameof(CreatedBy))] public AppUser? User { get; set; }
    }

    public enum CourseStatus
    {
        Ready,
        Upcoming,
        Cancelled
    }

    public enum CourseLevel
    {
        AllLevel,
        Beginner,
        Intermediate,
        Expert
    }
}
