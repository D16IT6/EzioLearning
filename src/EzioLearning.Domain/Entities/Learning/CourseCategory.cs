using EzioLearning.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace EzioLearning.Domain.Entities.Learning
{
    [Table(name: "CourseCategories", Schema = "Learning")]

    public class CourseCategory : AuditableEntity
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public bool IsActive { get; set; }

        [ForeignKey(nameof(Parent))]
        public Guid? ParentId { get; set; }
        public CourseCategory? Parent { get; set; }

        public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

    }
}
