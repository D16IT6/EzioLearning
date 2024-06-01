using System.ComponentModel.DataAnnotations.Schema;
using EzioLearning.Domain.Entities.Translation;
using EzioLearning.Share.Common;

namespace EzioLearning.Domain.Entities.Learning;

[Table("CourseCategories", Schema = SchemaConstants.Learning)]
public class CourseCategory : AuditableEntity
{
    public Guid Id { get; set; }
    public string? Image { get; set; }
    public bool IsActive { get; set; }

    [ForeignKey(nameof(Parent))] public Guid? ParentId { get; set; }

    public CourseCategory? Parent { get; set; }

    public virtual ICollection<Course> Courses { get; set; } = [];
    public virtual ICollection<CourseCategoryTranslation> CourseCategoryTranslations { get; set; } = [];
}