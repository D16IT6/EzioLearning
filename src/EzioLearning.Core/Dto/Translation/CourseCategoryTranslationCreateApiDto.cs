namespace EzioLearning.Core.Dto.Translation
{
    public class CourseCategoryTranslationCreateApiDto
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; } = string.Empty;
        public string? Culture { get; set; } = "vi-VN";
    }
}
