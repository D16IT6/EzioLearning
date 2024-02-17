using EzioLearning.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace EzioLearning.Domain.Entities.Learning
{
    [Table(name: "LessonComments", Schema = "Learning")]

    public class LessonComment : AuditableEntity
    {
        public Guid Id { get; set; }
        public required string Content { get; set; }

        public int Report { get; set; }

        [ForeignKey(nameof(Course))]
        public Guid CourseId { get; set; }
        public required Course Course { get; set; }
    }
}
