using EzioLearning.Share.Dto.Account;
using EzioLearning.Wasm.Common;
using EzioLearning.Wasm.Services.Interface;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using System.Net;

namespace EzioLearning.Wasm.Pages.Account;

public partial class AccountHome
{
    [SupplyParameterFromForm] private AccountInfoDto AccountInfo { get; set; } = new();
    [Inject] private HttpClient HttpClient { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private ISnackbar Snackbar { get; set; } = default!;
    [Inject] private ISnackBarService SnackBarService { get; set; } = default!;
    [Inject] private IAccountService AccountService { get; set; } = default!;

    [CascadingParameter] private Task<AuthenticationState>? AuthenticationStateTask { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var authenticationState = await AuthenticationStateTask!;
        var isAuthenticated = authenticationState.User.Identity!.IsAuthenticated;

        if (isAuthenticated)
        {
            var accountDto = await AccountService.GetInfo();
            switch (accountDto.Status)
            {
                case HttpStatusCode.BadRequest:
                    SnackBarService.ShowErrorFromResponse(accountDto);
                    await NavigateToHome("Lỗi yêu cầu");
                    break;
                case HttpStatusCode.Unauthorized:
                    await NavigateToHome("Bạn chưa xác thực");
                    break;
                case HttpStatusCode.Forbidden:
                    await NavigateToHome("Bạn không có quyền truy cập chức năng này");
                    break;
                case HttpStatusCode.OK:
                    AccountInfo = accountDto.Data!;
                    break;
            }
        }
    }

    private async Task NavigateToHome(string message, int seconds = 0)
    {
        Snackbar.Add(message);
        await Task.Delay(TimeSpan.FromSeconds(seconds));
        NavigationManager.NavigateTo(RouteConstants.Home);
    }
}