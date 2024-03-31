using BlazorAnimate;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace EzioLearning.Wasm.Components.Home;

public partial class ShareKnowledge
{
    [CascadingParameter] private IAnimation? AnimationType { get; set; }

    [CascadingParameter] private TimeSpan AnimationDuration { get; set; }
    [Inject] private IStringLocalizer<ShareKnowledge> Localizer { get; set; } = default!;
}