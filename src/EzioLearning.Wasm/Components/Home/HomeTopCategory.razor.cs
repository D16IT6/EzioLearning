using BlazorAnimate;
using EzioLearning.Share.Dto.Learning.CourseCategory;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;

namespace EzioLearning.Wasm.Components.Home;

public partial class HomeTopCategory
{
    [CascadingParameter] private List<TopCourseCategoryDto> TopCourseCategories { get; set; } = new();

    [CascadingParameter] private IAnimation? AnimationType { get; set; }

    [CascadingParameter] private TimeSpan AnimationDuration { get; set; }

    [Inject] private IJSRuntime JsRuntime { get; set; } = default!;

    [Inject] private IJSRuntime JsRunTime { get; set; } = default!;
    [Inject] private IStringLocalizer<HomeTopCategory> Localizer { get; set; } = default!;
    private IJSObjectReference JsObjectReference { get; set; } = default!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
            JsObjectReference =
                await JsRunTime.InvokeAsync<IJSObjectReference>("import",
                    $"/Components/Home/{nameof(HomeTopCategory)}.razor.js");
        else
            await JsObjectReference.InvokeVoidAsync("loadSlider");
    }
}