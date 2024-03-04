using BlazorAnimate;
using Microsoft.AspNetCore.Components;

namespace EzioLearning.Wasm.Components.Home;

public partial class UserLove
{
    [CascadingParameter] private IAnimation? AnimationType { get; set; }

    [CascadingParameter] private TimeSpan AnimationDuration { get; set; }
}