using System.ComponentModel.DataAnnotations.Schema;

namespace EzioLearning.Domain.Entities.Translation
{
    [Table(nameof(CourseCategoryTranslation) + "s", Schema = nameof(Translation))]
    public class CourseCategoryTranslation
    {
        public string Name { get; set; } = string.Empty;
    }
}
