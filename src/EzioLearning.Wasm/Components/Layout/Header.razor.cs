using EzioLearning.Share.Dto.Account;
using EzioLearning.Wasm.Common;
using EzioLearning.Wasm.Components.Account;
using EzioLearning.Wasm.Services.Interface;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using MudBlazor;

namespace EzioLearning.Wasm.Components.Layout;

public partial class Header :AccountComponentBase
{
    private string _headerPage = "";

    [Inject] protected NavigationManager NavigationManager { get; set; } = default!;

    [Inject] private IAuthService AuthService { get; set; } = default!;
    [Inject] private ISnackbar Snackbar { get; set; } = default!;

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
        _headerPage = NavigationManager.Uri.Equals(NavigationManager.BaseUri) || NavigationManager.Uri.Equals(NavigationManager.BaseUri + "/#")
            ? "" : "header-page";
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