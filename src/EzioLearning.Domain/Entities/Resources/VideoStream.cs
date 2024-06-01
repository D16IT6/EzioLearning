using System.ComponentModel.DataAnnotations.Schema;
using EzioLearning.Share.Common;
using EzioLearning.Share.Utils;

namespace EzioLearning.Domain.Entities.Resources
{
    [Table(nameof(VideoStream) + "s", Schema = SchemaConstants.Resource)]

    public class VideoStream
    {
        public Guid Id { get; set; }
        public string Path { get; set; } = string.Empty;

        public VideoQuality Quality { get; set; }

        [ForeignKey(nameof(Video))]
        public Guid VideoId { get; set; }
        public Video Video { get; set; } = null!;
    }
}
