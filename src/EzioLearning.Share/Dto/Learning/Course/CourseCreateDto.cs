using System.Text.Json.Serialization;
using EzioLearning.Share.Utils;
using Microsoft.AspNetCore.Components.Forms;

namespace EzioLearning.Share.Dto.Learning.Course;

public class CourseCreateDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;

    [JsonIgnore]
    public IBrowserFile? PosterImage { get; set; }

    public double Price { get; set; } = 0;
    public CourseLevel Level { get; set; }
    public CourseStatus Status { get; set; } = CourseStatus.Upcoming;
    public Guid[] CourseCategoryIds { get; set; } = [];
    public Guid CreatedBy { get; set; }
}