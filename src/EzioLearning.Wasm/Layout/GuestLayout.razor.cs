using EzioLearning.Domain.Common;
using EzioLearning.Wasm.Common;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace EzioLearning.Wasm.Layout;

public partial class GuestLayout
{
    [CascadingParameter] public string? ImageUrl { get; set; }
    [CascadingParameter] private Task<AuthenticationState>? AuthenticationStateTask { get; set; }


    protected override async Task OnInitializedAsync()
    {
        var authenticationState = await AuthenticationStateTask!;
        var isAuthenticated = authenticationState.User.Identity!.IsAuthenticated;

        if (isAuthenticated)
            ImageUrl =
                ApiConstants.BaseUrl +
                authenticationState.User.Claims
                    .FirstOrDefault(x => x.Type == CustomClaimTypes.Avatar)?.Value;
    }
}