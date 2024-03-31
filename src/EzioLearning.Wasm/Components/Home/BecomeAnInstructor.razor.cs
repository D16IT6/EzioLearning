using BlazorAnimate;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace EzioLearning.Wasm.Components.Home;

public partial class BecomeAnInstructor
{
    [CascadingParameter] private IAnimation? AnimationType { get; set; }

    [CascadingParameter] private TimeSpan AnimationDuration { get; set; }

    [Inject] private IStringLocalizer<BecomeAnInstructor> Localizer { get; set; } = default!;
} 