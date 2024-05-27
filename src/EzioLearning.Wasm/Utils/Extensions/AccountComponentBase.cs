using EzioLearning.Share.Dto.Account;
using EzioLearning.Wasm.Services.Interface;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using EzioLearning.Wasm.Utils.Common;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace EzioLearning.Wasm.Utils.Extensions
{
    public class AccountComponentBase : ComponentBase
    {
        private static AccountInfoMinimalDto? _accountInfoMinimal;
        protected AccountInfoMinimalDto AccountInfoMinimal { get; set; } = new();
        [CascadingParameter] private Task<AuthenticationState> AuthenticationStateTask { get; set; } = default!;
        protected AuthenticationState AuthenticationState { get; set; } = default!;
        [Inject] protected ISnackBarService SnackBarService { get; set; } = default!;
        [Inject] protected IAccountService AccountService { get; set; } = default!;
        [Inject] protected INavigationService NavigationService { get; set; } = default!;
        [Inject] private IStringLocalizer<AccountComponentBase> Localizer { get; set; } = default!;

        protected virtual async Task OnInitializedAsync(bool isHeader = false)
        {
            AuthenticationState = await AuthenticationStateTask;
            var isAuthenticated = AuthenticationState.User.Identity?.IsAuthenticated ?? false;

            if (!isAuthenticated)
            {
                if (isHeader) return;

                await NavigateUnAuthorized(Localizer.GetString("UnAuthorized"));
                return;
            }

            if (_accountInfoMinimal != null)
            {
                AccountInfoMinimal = _accountInfoMinimal;
                return;
            }

            var response = await AccountService.GetMinimalInfo();
            if (response is { IsSuccess: true, Data: not null })
            {
                AccountInfoMinimal = _accountInfoMinimal = response.Data;
            }
            else
            {
                await NavigationService.Navigate(RouteConstants.Login, response.Message, 0, false, Severity.Error);
            }
            StateHasChanged();
        }
        private async Task NavigateUnAuthorized(string message = "")
        {
            await NavigationService.Navigate(
                url: RouteConstants.Index,
                message: message,
                delaySeconds: 0,
                forceLoad: false,
                severity: Severity.Error);
        }
    }
}
