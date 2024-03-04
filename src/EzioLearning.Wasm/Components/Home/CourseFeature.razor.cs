using BlazorAnimate;
using EzioLearning.Share.Dto.Learning.Course;
using Microsoft.AspNetCore.Components;

namespace EzioLearning.Wasm.Components.Home;

public partial class CourseFeature
{
    [CascadingParameter] private List<CourseViewDto> CourseViewDto { get; set; } = new();
    [Parameter] public string? CustomClass { get; set; }

    [CascadingParameter] private IAnimation? AnimationType { get; set; }

    [CascadingParameter] private TimeSpan AnimationDuration { get; set; }
}