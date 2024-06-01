using System.ComponentModel.DataAnnotations.Schema;
using EzioLearning.Share.Common;
using EzioLearning.Share.Utils;

namespace EzioLearning.Domain.Entities.Resources
{
    [Table(nameof(Video) + "s", Schema = SchemaConstants.Resource)]
    public class Video
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string DefaultPath { get; set; } = string.Empty;

        public long Duration { get; set; }
        public VideoStatus Status { get; set; }

        public ICollection<VideoStream> VideoStreams { get; set; } = new List<VideoStream>();
    }

   
}
