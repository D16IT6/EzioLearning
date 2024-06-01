using System.ComponentModel.DataAnnotations.Schema;
using EzioLearning.Domain.Entities.Resources;
using EzioLearning.Share.Common;
using Attachment = EzioLearning.Domain.Entities.Resources.Attachment;

namespace EzioLearning.Domain.Entities.Learning;

[Table(nameof(CourseLecture) + "s", Schema = SchemaConstants.Learning)]
public class CourseLecture : AuditableEntity
{
    public Guid Id { get; set; }
    public required string Name { get; set; }

    [ForeignKey(nameof(Video))]
    public Guid? VideoId { get; set; }
    public Video? Video { get; set; }

    [ForeignKey(nameof(Attachment))]
    public Guid? AttachmentId { get; set; }
    public Attachment? Attachment { get; set; }


    public int SortOrder { get; set; }
    public bool IsActive { get; set; }

    [ForeignKey(nameof(CourseSection))] public Guid CourseSectionId { get; set; }

    public CourseSection? CourseSection { get; set; }
}