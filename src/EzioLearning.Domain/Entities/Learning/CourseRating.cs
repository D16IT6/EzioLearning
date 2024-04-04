using System.ComponentModel.DataAnnotations.Schema;
using EzioLearning.Domain.Common;

namespace EzioLearning.Domain.Entities.Learning;

[Table(nameof(CourseRating) +"s", Schema = SchemaConstants.Learning)]
public class CourseRating : AuditableEntity
{
    public Guid Id { get; set; }
    public double Point { get; set; }

    [ForeignKey(nameof(Course))] public Guid CourseId { get; set; }

    public required Course Course { get; set; }
}