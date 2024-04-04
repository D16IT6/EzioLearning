using Microsoft.AspNetCore.Http;

namespace EzioLearning.Core.Dto.Learning.CourseCategory
{
    public class CourseCategoryCreateApiDto
    {
        public required string Name { get; init; }

        public IFormFile? Image { get; init; }
        public bool IsActive { get; init; }
        public Guid? ParentId { get; init; }
    }
}
