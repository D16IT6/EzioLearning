using EzioLearning.Domain.Common;
using EzioLearning.Wasm.Providers;
using EzioLearning.Wasm.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;

namespace EzioLearning.Wasm.Components.Layout
{
    public partial class Header
    {
        private string? _imageUrl;

        [Inject] protected NavigationManager NavigationManager { get; set; } = default!;

        [CascadingParameter] private Task<AuthenticationState>? AuthenticationStateTask { get; set; } = default;
        [Inject] private IAuthService? AuthService { get; set; }
        [Inject] private ISnackbar Snackbar { get; set; } = default!;


        protected override async Task OnInitializedAsync()
        {
            var authenticationState = await AuthenticationStateTask!;
            var isAuthenticated = authenticationState.User.Identity!.IsAuthenticated;

            if (isAuthenticated)
            {
                _imageUrl = 
                    ApiConstants.BaseUrl +
                    authenticationState.User.Claims
                        .FirstOrDefault(x => x.Type == CustomClaimTypes.Avatar)?.Value;
            }
        }

        private async Task Logout()
        {
            var data = await AuthService!.Logout();
            if (data != null)
            {
                Snackbar.Add("Đăng xuất thành công", Severity.Info);
                await Task.Delay(1000);
                NavigationManager.NavigateTo(NavigationManager.Uri,true);
            }
        }
    }
}
