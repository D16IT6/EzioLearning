using System.ComponentModel.DataAnnotations.Schema;
using EzioLearning.Domain.Entities.Learning;
using EzioLearning.Domain.Entities.System;

namespace EzioLearning.Domain.Entities.Translation
{
    [Table(nameof(CourseTranslation) + "s", Schema = nameof(Translation))]
    public class CourseTranslation
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Poster { get; set; }
        public string? Content { get; set; }

        [ForeignKey(nameof(Course))]
        public Guid CourseId { get; set; }
        public Course Course { get; set; } = new();

        [ForeignKey(nameof(Culture))] public string CultureId { get; set; } = string.Empty;
        public Culture Culture { get; set; } = new();
    }
}
