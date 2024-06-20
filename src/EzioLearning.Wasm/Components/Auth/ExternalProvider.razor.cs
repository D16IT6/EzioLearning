using EzioLearning.Wasm.Services.Interface;
using EzioLearning.Wasm.Utils.Common;
using Microsoft.AspNetCore.Components;

namespace EzioLearning.Wasm.Components.Auth;

public partial class ExternalProvider
{
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;

    [Inject] private IAuthService AuthService { get; set; } = default!;

    private string GoogleCallbackUrl { get; set; } = string.Empty;
    private string FacebookCallbackUrl { get; set; } = string.Empty;
    private string MicrosoftCallbackUrl { get; set; } = string.Empty;

    private string ReturnUrl { get; set; } = string.Empty;
    protected override async Task OnInitializedAsync()
    {
        ReturnUrl = NavigationManager.ToAbsoluteUri("/ExternalLogin").AbsoluteUri;

        var googleCallbackUrlTask = AuthService.GetExternalLoginUrl(ExternalLoginProvider.Google, ReturnUrl);
        var facebookCallbackUrlTask =  AuthService.GetExternalLoginUrl(ExternalLoginProvider.Facebook, ReturnUrl);
        var microsoftCallbackUrlTask = AuthService.GetExternalLoginUrl(ExternalLoginProvider.Microsoft, ReturnUrl);

        await Task.WhenAll(googleCallbackUrlTask, facebookCallbackUrlTask, microsoftCallbackUrlTask);

        FacebookCallbackUrl = await facebookCallbackUrlTask;
        GoogleCallbackUrl = await googleCallbackUrlTask;
        MicrosoftCallbackUrl = await microsoftCallbackUrlTask;
    }

    public void ExternalLogin(string externalProvider)
    {
        var redirectUrl = externalProvider switch
        {
            ExternalLoginProvider.Google => GoogleCallbackUrl,
            ExternalLoginProvider.Facebook => FacebookCallbackUrl,
            ExternalLoginProvider.Microsoft => MicrosoftCallbackUrl,
            _ => RouteConstants.Index
        };

        NavigationManager.NavigateTo(redirectUrl);
    }
}
public static class ExternalLoginProvider
{
    public const string Google = nameof(Google);
    public const string Facebook = nameof(Facebook);
    public const string Microsoft = nameof(Microsoft);
}