using System.ComponentModel.DataAnnotations.Schema;
using EzioLearning.Domain.Entities.Identity;
using EzioLearning.Share.Common;
using EzioLearning.Share.Utils;

namespace EzioLearning.Domain.Entities.Learning;

[Table(nameof(Course) +"s", Schema = SchemaConstants.Learning)]
public class Course : AuditableEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Content { get; set; } = string.Empty;
    public string? Poster { get; set; }
    public double Price { get; set; } = 0;
    public double PromotionPrice { get; set; }
    public CourseLevel Level { get; set; }
    public CourseStatus Status { get; set; }

    public ICollection<CourseCategory> Categories { get; set; } = [];
    public ICollection<CourseSection> Sections { get; set; } = [];
    public ICollection<CourseRating> Ratings { get; set; } = [];
    public ICollection<Student> Students { get; set; } = [];

    [ForeignKey(nameof(CreatedBy))] public AppUser? User { get; set; }
}
