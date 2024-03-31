using EzioLearning.Share.Dto.Account;
using EzioLearning.Wasm.Services.Interface;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net;
using EzioLearning.Wasm.Utils.Common;
using MudBlazor;

namespace EzioLearning.Wasm.Components.Account
{
    public class AccountComponentBase : ComponentBase
    {
        protected AccountInfoMinimalDto AccountInfoMinimal { get; set; } = new();
        [CascadingParameter] private Task<AuthenticationState>? AuthenticationStateTask { get; set; }

        protected AuthenticationState AuthenticationState { get; set; } = default!;
        [Inject] protected ISnackBarService SnackBarService { get; set; } = default!;

        [Inject] protected IAccountService AccountService { get; set; } = default!;
        [Inject] protected INavigationService NavigationService { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            AuthenticationState = await AuthenticationStateTask!;
            var isAuthenticated = AuthenticationState.User.Identity!.IsAuthenticated;

            if (isAuthenticated)
            {
                var response = await AccountService.GetMinimalInfo();
                if (response.Status == HttpStatusCode.OK)
                {
                    response.Data!.Avatar = ApiConstants.BaseUrl + response.Data.Avatar + $"?t={Guid.NewGuid()}";

                    AccountInfoMinimal = response.Data;
                    
                    StateHasChanged();
                }

                else
                {
                    SnackBarService.ShowErrorFromResponse(response);
                    await NavigationService.Navigate(RouteConstants.Index, response.Message,0,false,Severity.Error);

                }
            }
        }
    }
}
