using System.Net;
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
    public string? ImageUrl { get; set; }

    [Inject] protected NavigationManager NavigationManager { get; set; } = default!;

    [Inject] private IAuthService? AuthService { get; set; }
    [Inject] private ISnackbar Snackbar { get; set; } = default!;
    [Inject] private IAccountService AccountService { get; set; } = default!;
    [Inject] private ISnackBarService SnackBarService { get; set; } = default!;

    private AuthenticationState? AuthenticationState { get; set; }
    [CascadingParameter] private Task<AuthenticationState> AuthenticationStateTask { get; set; } = default!;
    protected override async Task OnInitializedAsync()
    {
        AuthenticationState = await AuthenticationStateTask;
        if (AuthenticationState.User.Identity!.IsAuthenticated)
        {
            var response = await AccountService.GetAvatar();
            if (response!.Status == HttpStatusCode.OK)
            {
                ImageUrl = ApiConstants.BaseUrl + response.Data;
            }
            else
            {
                SnackBarService.ShowErrorFromResponse(response);
            }
        }
    }

    protected override void OnInitialized()
    {
        UpdateHeaderClass();
        NavigationManager.LocationChanged += HandleLocationChanged;
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