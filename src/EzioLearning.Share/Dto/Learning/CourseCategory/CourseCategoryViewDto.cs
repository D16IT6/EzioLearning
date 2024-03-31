
namespace EzioLearning.Share.Dto.Learning.CourseCategory;

public class CourseCategoryViewDto
{
    public Guid Id { get; init; }
    public string? Name { get; init; }
    public Guid? ParentId { get; init; }
    public string? ParentName { get; init; }


}