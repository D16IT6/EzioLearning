using System.ComponentModel.DataAnnotations.Schema;
using EzioLearning.Domain.Common;

namespace EzioLearning.Domain.Entities.Learning;

[Table("CourseCategories", Schema = "Learning")]
public class CourseCategory : AuditableEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Image { get; set; }
    public bool IsActive { get; set; }

    [ForeignKey(nameof(Parent))] public Guid? ParentId { get; set; }

    public CourseCategory? Parent { get; set; }

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
}