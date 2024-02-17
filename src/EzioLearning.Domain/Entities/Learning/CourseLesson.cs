
using System.ComponentModel.DataAnnotations.Schema;
using EzioLearning.Domain.Common;

namespace EzioLearning.Domain.Entities.Learning
{
    [Table(name: "CourseLessons", Schema = "Learning")]

    public class CourseLesson : AuditableEntity
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }

        public string? VideoPath { get; set; }
        public string? SlidePath { get; set; }
        public string? Attachment { get; set; }

        public int SortOrder { get; set; }
        public bool IsActive { get; set; }

        [ForeignKey(nameof(Course))]
        public Guid CourseId { get; set; }
        public required Course Course { get; set; }
    }
}
