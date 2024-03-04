using BlazorAnimate;
using Microsoft.AspNetCore.Components;

namespace EzioLearning.Wasm.Components.Layout;

public partial class Footer
{
    [CascadingParameter] private IAnimation? AnimationType { get; set; }

    [CascadingParameter] private TimeSpan AnimationDuration { get; set; }
}