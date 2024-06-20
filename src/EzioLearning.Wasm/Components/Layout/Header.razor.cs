using System.Globalization;
using Blazored.LocalStorage;
using EzioLearning.Share.Dto.Culture;
using EzioLearning.Wasm.Services.Interface;
using EzioLearning.Wasm.Utils.Common;
using EzioLearning.Wasm.Utils.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using MudBlazor;

namespace EzioLearning.Wasm.Components.Layout;

public partial class Header : AccountComponentBase
{
    private string _headerPage = "";

    private List<CultureViewDto> Cultures { get; set; } = new();
    private string SelectedCulture { get; set; } = string.Empty;
    [Inject] protected NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private IAuthService AuthService { get; set; } = default!;
    [Inject] private ICultureService CultureService { get; set; } = default!;
    [Inject] private ISnackbar Snackbar { get; set; } = default!;
    [Inject] private IStringLocalizer<Header> Localizer { get; set; } = default!;
    [Inject] private IJSRuntime JsRuntime { get; set; } = default!;
    [Inject] private ILogger<Header> Logger { get; set; } = default!;
    [Inject] private ILocalStorageService LocalStorageService { get; set; } = default!;
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
        var data = await AuthService.Logout();
        if (data != null)
        {
            Snackbar.Add("Đăng xuất thành công", Severity.Info);
            //await Task.Delay(1000);
            NavigationManager.NavigateTo(RouteConstants.Login, true);
        }
    }


    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync(isHeader: true);
        var cultures = await CultureService.GetCultures();
        if (cultures.Any())
        {
            SelectedCulture = cultures.First(x => x.Id.Equals(CultureInfo.CurrentCulture.Name)).Id;
            Cultures.AddRange(cultures.ToList());
        }

    }


    private async Task OnChangeCulture(string  selectedCulture)
    {
        await LocalStorageService.SetItemAsStringAsync(nameof(CultureInfo), selectedCulture);

        NavigationManager.NavigateTo(NavigationManager.Uri, forceLoad: true);
    }

}