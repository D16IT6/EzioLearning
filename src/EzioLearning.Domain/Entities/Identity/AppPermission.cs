
using System.ComponentModel.DataAnnotations.Schema;
using EzioLearning.Share.Common;

namespace EzioLearning.Domain.Entities.Identity
{
    [Table(nameof(AppPermission) + "s", Schema = SchemaConstants.Auth)]
    public class AppPermission
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;

        public ICollection<AppUser> Users { get; set; } = new List<AppUser>();
    }
}
