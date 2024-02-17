using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EzioLearning.Domain.Common;
using EzioLearning.Domain.Entities.Identity;

namespace EzioLearning.Domain.Entities.Learning
{
    [Table("Trainers", Schema = "Learning")]
    public class Trainer : AuditableEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string? Name { get; set; }

        public string? Avatar { get; set; }
        public string? Description { get; set; }
        public string? Bio { get; set; }

        [ForeignKey(nameof(User))]
        public Guid? UserId { get; set; }
        public AppUser? User { get; set; }

        public ICollection<Course> Courses { get; set; } = new List<Course>();

    }
}
