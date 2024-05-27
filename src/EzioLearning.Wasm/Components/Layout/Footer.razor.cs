using BlazorAnimate;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Localization;

namespace EzioLearning.Wasm.Components.Layout;

public partial class Footer
{
    [CascadingParameter] private IAnimation? AnimationType { get; set; }

    [CascadingParameter] private TimeSpan AnimationDuration { get; set; }
    [Inject] private IStringLocalizer<Footer> Localizer { get; set; } = default!;

    [CascadingParameter] private Task<AuthenticationState> AuthenticationState { get; set; } = default!;


    private bool IsAuthenticated { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var authenticationState = await AuthenticationState;

        if (authenticationState.User.Identity != null)
            IsAuthenticated = authenticationState.User.Identity.IsAuthenticated;
    }
}