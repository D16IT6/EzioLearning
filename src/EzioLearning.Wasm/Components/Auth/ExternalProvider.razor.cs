using EzioLearning.Wasm.Utils.Common;
using Microsoft.AspNetCore.Components;

namespace EzioLearning.Wasm.Components.Auth;

public partial class ExternalProvider
{
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;

    private string GoogleCallbackUrl { get; set; } = string.Empty;
    private string FacebookCallbackUrl { get; set; } = string.Empty;
    private string MicrosoftCallbackUrl { get; set; } = string.Empty;

    protected override void OnInitialized()
    {
        GoogleCallbackUrl =
            $"{ApiConstants.BaseUrl}api/Auth/GoogleLogin?returnUrl=" +
            NavigationManager.ToAbsoluteUri("/ExternalLogin").AbsoluteUri;

        FacebookCallbackUrl =
            $"{ApiConstants.BaseUrl}api/Auth/FacebookLogin?returnUrl=" +
            NavigationManager.ToAbsoluteUri("/ExternalLogin").AbsoluteUri;

        MicrosoftCallbackUrl =
            $"{ApiConstants.BaseUrl}api/Auth/MicrosoftLogin?returnUrl=" +
            NavigationManager.ToAbsoluteUri("/ExternalLogin").AbsoluteUri;
    }

    public void ExternalLogin(string externalProvider)
    {
        var uri = $"{ApiConstants.BaseUrl}api/Auth/{externalProvider}Login?returnUrl=" +
                  NavigationManager.ToAbsoluteUri("/ExternalLogin").AbsoluteUri;
        NavigationManager.NavigateTo(uri);
    }
}

public static class ExternalLoginProvider
{
    public static string Google = nameof(Google);
    public static string Facebook = nameof(Facebook);
    public static string Microsoft = nameof(Microsoft);
}