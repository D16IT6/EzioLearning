using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using EzioLearning.Share.Dto.Account;

namespace EzioLearning.Wasm.Components.Account
{
    public partial class AccountSidebar
    {
        [CascadingParameter] public AccountInfoMinimalDto AccountInfoMinimal { get; set; } = new();
        [CascadingParameter] private Task<AuthenticationState>? AuthenticationStateTask { get; set; }
        private AuthenticationState? AuthenticationState { get; set; }

        protected override async Task OnInitializedAsync()
        {
            AuthenticationState = await AuthenticationStateTask!;
            //var isAuthenticated = AuthenticationState.User.Identity!.IsAuthenticated;

            //if (!isAuthenticated)
            //{
            //    var response = await AccountService.GetAvatar();
            //    if (response!.Status == HttpStatusCode.OK)
            //    {
            //        ImageUrl = ApiConstants.BaseUrl + response.Data;
            //    }
            //    else
            //    {
            //        SnackBarService.ShowErrorFromResponse(response);
            //    }
            //}
        }
    }
}
