using EzioLearning.Wasm.Utils.Common;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using EzioLearning.Wasm.Services.Interface;

namespace EzioLearning.Wasm.Utils.Extensions
{
    public class AuthComponentBase : ComponentBase
    {
        [CascadingParameter] protected Task<AuthenticationState> AuthenticationStateTask { get; set; } = default!;
        [Inject] protected IAuthService AuthService { get; set; } = default!;
        [Inject] protected NavigationManager NavigationManager { get; set; } = default!;
        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthenticationStateTask;
            if (authState.User.Identity is { IsAuthenticated: true })
            {
                NavigationManager.NavigateTo(RouteConstants.Index);
            }
        }
    }
}
