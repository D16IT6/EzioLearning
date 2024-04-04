using System.ComponentModel.DataAnnotations.Schema;
using EzioLearning.Domain.Common;
using EzioLearning.Domain.Entities.Identity;

namespace EzioLearning.Domain.Entities.Learning;

[Table(nameof(Student) + "s", Schema = SchemaConstants.Learning)]
public class Student : AuditableTimeOnlyEntity
{
    public Guid Id { get; set; }

    public double Price { get; set; }

    [ForeignKey(nameof(Course))] public Guid CourseId { get; set; }

    public Course? Course { get; set; }

    [ForeignKey(nameof(User))] public Guid UserId { get; set; }

    public AppUser? User { get; set; }
}