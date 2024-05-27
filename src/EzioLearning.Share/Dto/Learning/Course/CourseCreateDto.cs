using EzioLearning.Share.Utils;
using Microsoft.AspNetCore.Components.Forms;

namespace EzioLearning.Share.Dto.Learning.Course;

public class CourseCreateDto
{
    public CourseStatus Status = CourseStatus.Upcoming;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    private IBrowserFile? Poster { get; set; }

    public string? Content { get; set; }

    public double Price { get; set; } = 0;
    public double PromotionPrice { get; set; }
    public int SortOrder { get; set; }
    public CourseLevel Level { get; set; }

    public Guid[] CourseCategoryIds { get; set; } = [];

    public Guid CreatedBy { get; set; }
}