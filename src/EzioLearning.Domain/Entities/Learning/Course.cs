using System.ComponentModel.DataAnnotations.Schema;
using EzioLearning.Domain.Common;
using EzioLearning.Domain.Entities.Identity;
using EzioLearning.Share.Utils;

namespace EzioLearning.Domain.Entities.Learning;

[Table(nameof(Course) +"s", Schema = nameof(Learning))]
public class Course : AuditableEntity
{
    public Guid Id { get; set; }
    public string? Poster { get; set; }
    public double Price { get; set; } = 0;
    public double PromotionPrice { get; set; }
    public int SortOrder { get; set; }
    public CourseLevel Level { get; set; }
    public CourseStatus Status { get; set; }

    public ICollection<CourseCategory> Categories { get; set; } = new List<CourseCategory>();
    public ICollection<CourseRating> Ratings { get; set; } = new List<CourseRating>();
    public ICollection<CourseLesson> Lessons { get; set; } = new List<CourseLesson>();
    public ICollection<Student> Students { get; set; } = new List<Student>();

    [ForeignKey(nameof(CreatedBy))] public AppUser? User { get; set; }
}
