using BlazorAnimate;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace EzioLearning.Wasm.Components.Layout;

public partial class Footer
{
    [CascadingParameter] private IAnimation? AnimationType { get; set; }

    [CascadingParameter] private TimeSpan AnimationDuration { get; set; }
    [Inject] private IStringLocalizer<Footer> Localizer { get; set; } = default!;
}