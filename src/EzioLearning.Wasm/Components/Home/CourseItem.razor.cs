

using EzioLearning.Core.Dtos.Learning.Course;
using Microsoft.AspNetCore.Components;

namespace EzioLearning.Wasm.Components.Home
{
    public partial class CourseItem
    {
        [CascadingParameter] private CourseViewDto CourseViewDto { get; set; } = new();
        [Parameter] public string? CustomClass { get; set; }
    }
}
