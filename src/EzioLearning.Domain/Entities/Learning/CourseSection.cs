using System.ComponentModel.DataAnnotations.Schema;
using EzioLearning.Share.Common;

namespace EzioLearning.Domain.Entities.Learning
{
    [Table(nameof(CourseSection) + "s", Schema = SchemaConstants.Learning)]

    public class CourseSection :AuditableEntity
    {
        public Guid Id { get; set; }
        public string? Name { get; set; } = string.Empty;
        public ICollection<CourseLecture> CourseLectures { get; set; } = [];

        [ForeignKey(nameof(Course))] public Guid CourseId { get; set; }
        public Course? Course { get; set; }


    }
}
