using Microsoft.AspNetCore.Components.Forms;

namespace EzioLearning.Share.Dto.Learning.CourseCategory;

public class CourseCategoryCreateDto
{
    public required string Name { get; init; }

    public IBrowserFile? Image { get; init; }
    public bool IsActive { get; init; }
    public Guid? ParentId { get; init; }

    
}