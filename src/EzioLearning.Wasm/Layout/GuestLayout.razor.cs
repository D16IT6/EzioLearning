using EzioLearning.Wasm.Common;
using EzioLearning.Wasm.Services.Interface;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net;
using EzioLearning.Share.Dto.Account;

namespace EzioLearning.Wasm.Layout;

public partial class GuestLayout
{
    [CascadingParameter] public AccountInfoMinimalDto AccountInfoMinimal { get; set; } = new();
    [CascadingParameter] private Task<AuthenticationState>? AuthenticationStateTask { get; set; }
    [Inject] private ISnackBarService SnackBarService { get; set; } = default!;

    [Inject] private IAccountService AccountService { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        var authenticationState = await AuthenticationStateTask!;
        var isAuthenticated = authenticationState.User.Identity!.IsAuthenticated;

        if (isAuthenticated)
        {
            var response = await AccountService.GetMinimalInfo();
            if (response.Status == HttpStatusCode.OK)
            {
                response.Data!.Avatar = ApiConstants.BaseUrl + response.Data.Avatar + $"?t={Guid.NewGuid()}";
                
                AccountInfoMinimal = response.Data;
            }
            else
            {
                SnackBarService.ShowErrorFromResponse(response);
            }
        }
    }
}