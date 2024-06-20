using System.ComponentModel.DataAnnotations.Schema;
using EzioLearning.Domain.Entities.Resources;
using EzioLearning.Share.Common;
using EzioLearning.Share.Utils;

namespace EzioLearning.Domain.Entities.Learning;

[Table(nameof(CourseLecture) + "s", Schema = SchemaConstants.Learning)]
public class CourseLecture : AuditableEntity
{
    public Guid Id { get; set; }
    public required string Name { get; set; }

    public CourseLectureType LectureType { get; set; }


    public Video? Video { get; set; }
    public Document? Document { get; set; }

    public int SortOrder { get; set; }
    public bool IsActive { get; set; }

    [ForeignKey(nameof(CourseSection))] public Guid CourseSectionId { get; set; }

    public CourseSection? CourseSection { get; set; }
}