using BlazorAnimate;
using EzioLearning.Share.Dto.Learning.CourseCategory;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace EzioLearning.Wasm.Components.Home;

public partial class HomeBanner
{
    [CascadingParameter] private List<CourseCategoryViewDto> CourseCategories { get; set; } = new();

    [CascadingParameter] private int CourseCount { get; set; }

    [CascadingParameter] private IAnimation? AnimationType { get; set; }

    [CascadingParameter] private TimeSpan AnimationDuration { get; set; }

    [Inject] private IStringLocalizer<HomeBanner> Localizer { get; set; } = default!;
}