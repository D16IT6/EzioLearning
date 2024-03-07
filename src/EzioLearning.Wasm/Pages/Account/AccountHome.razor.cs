using EzioLearning.Wasm.Common;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using System.Net;
using System.Security.Claims;

namespace EzioLearning.Wasm.Pages.Account;

public partial class AccountHome
{
    [Inject] private HttpClient HttpClient { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private ISnackbar Snackbar { get; set; } = default!;

    [CascadingParameter] private Task<AuthenticationState>? AuthenticationStateTask { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var authenticationState = await AuthenticationStateTask!;
        var isAuthenticated = authenticationState.User.Identity!.IsAuthenticated;

        if (isAuthenticated)
        {
            var id = authenticationState.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.PrimarySid)?.Value;
            var response = await HttpClient.GetAsync($"api/User/GetInfo/{id}");

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                Snackbar.Add("Lỗi server, vui lòng thử lại", Severity.Error);
                NavigationManager.NavigateTo(RouteConstants.Home,forceLoad:true);

            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                Snackbar.Add("Không có quyền truy cập", Severity.Error);
                NavigationManager.NavigateTo(RouteConstants.Error.UnAuthorized);
            }
            else
            {
                Snackbar.Add("Chào mừng trở lại!", Severity.Info);
            }
        }
    }
}