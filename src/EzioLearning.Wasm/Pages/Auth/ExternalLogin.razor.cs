using Blazored.LocalStorage;
using EzioLearning.Wasm.Utils.Common;
using EzioLearning.Wasm.Utils.Extensions;
using Microsoft.AspNetCore.Components;

namespace EzioLearning.Wasm.Pages.Auth;

public partial class ExternalLogin : AuthComponentBase
{
    [SupplyParameterFromQuery] public string CacheKey { get; set; } = string.Empty;
    [Inject] private ILocalStorageService LocalStorageService { get; set; } = default!;
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        var request = await AuthService.GetExternalLoginInfo(CacheKey);

        var cacheInfoData = request.Data;

        var token = cacheInfoData!.Token;
        if (!string.IsNullOrEmpty(token.AccessToken) && !string.IsNullOrEmpty(token.RefreshToken))
        {
            await LocalStorageService.SetItemAsStringAsync(LocalStorageConstants.AccessToken, token.AccessToken);
            await LocalStorageService.SetItemAsStringAsync(LocalStorageConstants.RefreshToken, token.RefreshToken);

            NavigationManager.NavigateTo(RouteConstants.Index, true);
        }

        if (cacheInfoData.BackToLogin) NavigationManager.NavigateTo(RouteConstants.Login);

        if (cacheInfoData.NeedRegister)
        {
            var queryStringParams = new Dictionary<string, object?>
            {
                { nameof(cacheInfoData.Email), cacheInfoData.Email },
                { nameof(cacheInfoData.FirstName), cacheInfoData.FirstName },
                { nameof(cacheInfoData.LastName), cacheInfoData.LastName },
                { nameof(cacheInfoData.UserName), cacheInfoData.UserName },
                { nameof(cacheInfoData.LoginProvider), cacheInfoData.LoginProvider },
                { nameof(cacheInfoData.ProviderKey), cacheInfoData.ProviderKey },
                { nameof(cacheInfoData.ProviderName), cacheInfoData.ProviderName }
            };
            var registerUrl = NavigationManager.GetUriWithQueryParameters(RouteConstants.Register, queryStringParams);

            NavigationManager.NavigateTo(registerUrl);

        }
    }
}