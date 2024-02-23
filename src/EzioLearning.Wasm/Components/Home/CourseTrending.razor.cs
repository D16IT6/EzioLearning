using Microsoft.AspNetCore.Components;
using EzioLearning.Core.Dtos.Learning.Course;
using EzioLearning.Core.Dtos.User;
using BlazorAnimate;
using Microsoft.JSInterop;


namespace EzioLearning.Wasm.Components.Home
{
	public partial class CourseTrending
    {
        [CascadingParameter] private List<CourseViewDto> CourseTrends { get; set; } = new();
        [CascadingParameter] private List<InstructorViewDto> FeatureInstructors { get; set; } = new();

        [CascadingParameter]
        private IAnimation? AnimationType { get; set; }
        [CascadingParameter]
        private TimeSpan AnimationDuration { get; set; }

        [Inject] private IJSRuntime JsRunTime { get; set; } = default!;
        private IJSObjectReference JsObjectReference { get; set; } = default!;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                JsObjectReference =
                    await JsRunTime.InvokeAsync<IJSObjectReference>
                        ("import", $"/Components/Home/{nameof(CourseTrending)}.razor.js");
            }
            else
            {
                await JsObjectReference.InvokeVoidAsync("loadSlider");
            }
        }
    }
}
