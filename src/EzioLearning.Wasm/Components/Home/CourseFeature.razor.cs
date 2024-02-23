using Microsoft.AspNetCore.Components;

using EzioLearning.Core.Dtos.Learning.Course;
using BlazorAnimate;


namespace EzioLearning.Wasm.Components.Home
{
    public partial class CourseFeature
    {
        [CascadingParameter] private List<CourseViewDto> CourseViewDtos { get; set; } = new();
        [Parameter] public string? CustomClass { get; set; }
        [CascadingParameter]
        private IAnimation? AnimationType { get; set; }
        [CascadingParameter]
        private TimeSpan AnimationDuration { get; set; }
    }
}
