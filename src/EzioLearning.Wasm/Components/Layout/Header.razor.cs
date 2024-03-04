using EzioLearning.Domain.Common;
using EzioLearning.Wasm.Common;
using EzioLearning.Wasm.Services.Interface;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;

namespace EzioLearning.Wasm.Components.Layout;

public partial class Header
{
    private string _headerPage = "header-page";
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
            _imageUrl =
                ApiConstants.BaseUrl +
                authenticationState.User.Claims
                    .FirstOrDefault(x => x.Type == CustomClaimTypes.Avatar)?.Value;
    }

    protected override void OnAfterRender(bool firstRender)
    {
        if (NavigationManager.Uri.Equals(NavigationManager.BaseUri)) _headerPage = string.Empty;
        base.OnAfterRender(firstRender);
    }

    private async Task Logout()
    {
        var data = await AuthService!.Logout();
        if (data != null)
        {
            Snackbar.Add("Đăng xuất thành công", Severity.Info);
            await Task.Delay(1000);
            NavigationManager.NavigateTo(NavigationManager.Uri, true);
        }
    }
}