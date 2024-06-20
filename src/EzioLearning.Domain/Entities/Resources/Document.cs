using System.ComponentModel.DataAnnotations.Schema;
using EzioLearning.Domain.Entities.Learning;
using EzioLearning.Share.Common;

namespace EzioLearning.Domain.Entities.Resources
{
    [Table(nameof(Document) + "s", Schema = SchemaConstants.Resource)]

    public class Document
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;

        public CourseLecture? Lecture { get; set; }

        [ForeignKey(nameof(Lecture))]
        public Guid LectureId { get; set; }
	}
}
