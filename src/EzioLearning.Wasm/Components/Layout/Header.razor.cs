using EzioLearning.Domain.Common;
using EzioLearning.Wasm.Common;
using EzioLearning.Wasm.Services.Interface;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using MudBlazor;

namespace EzioLearning.Wasm.Components.Layout;

public partial class Header
{
    private string _headerPage = "";
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

    protected override void OnInitialized()
    {
        UpdateHeaderClass();
        NavigationManager.LocationChanged += HandleLocationChanged;
    }

    public void Dispose()
    {
        NavigationManager.LocationChanged -= HandleLocationChanged;
    }

    private void HandleLocationChanged(object? _, LocationChangedEventArgs e)
    {
        UpdateHeaderClass();
    }
    private void UpdateHeaderClass()
    {
        _headerPage = NavigationManager.Uri.Equals(NavigationManager.BaseUri) ? "" : "header-page";
        StateHasChanged();
    }

    private async Task Logout()
    {
        var data = await AuthService!.Logout();
        if (data != null)
        {
            Snackbar.Add("Đăng xuất thành công", Severity.Info);
            await Task.Delay(1000);
            NavigationManager.NavigateTo(RouteConstants.Login, true);
        }
    }
}