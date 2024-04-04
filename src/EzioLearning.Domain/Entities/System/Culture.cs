using EzioLearning.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace EzioLearning.Domain.Entities.System
{
    [Table(nameof(Culture) + "s", Schema = SchemaConstants.System)]
    public class Culture
    {
        public string Id { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
        public int SortOrder { get; set; }
    }
}
