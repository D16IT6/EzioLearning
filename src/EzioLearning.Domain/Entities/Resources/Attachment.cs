using System.ComponentModel.DataAnnotations.Schema;
using EzioLearning.Share.Common;

namespace EzioLearning.Domain.Entities.Resources
{
    [Table(nameof(Attachment) + "s", Schema = SchemaConstants.Resource)]

    public class Attachment
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
    }
}
