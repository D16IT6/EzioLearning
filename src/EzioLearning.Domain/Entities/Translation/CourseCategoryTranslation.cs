using System.ComponentModel.DataAnnotations.Schema;
using EzioLearning.Domain.Common;
using EzioLearning.Domain.Entities.Learning;
using EzioLearning.Domain.Entities.System;

namespace EzioLearning.Domain.Entities.Translation
{
    [Table(nameof(CourseCategoryTranslation) + "s", Schema = SchemaConstants.Translation)]
    public class CourseCategoryTranslation
    {
        public string Name { get; set; } = string.Empty;

        [ForeignKey(nameof(CourseCategory))]
        public Guid CourseCategoryId { get; set; }
        public CourseCategory CourseCategory { get; set; } = new();

        [ForeignKey(nameof(Culture))]
        public string CultureId { get; set; } = string.Empty;
        public Culture Culture { get; set; } = new();
    }
}
