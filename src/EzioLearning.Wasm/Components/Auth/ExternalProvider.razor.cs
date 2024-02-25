using EzioLearning.Wasm.Providers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace EzioLearning.Wasm.Components.Auth
{
    public partial class ExternalProvider
    {
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;

        private string GoogleCallbackUrl { get; set; } = string.Empty;
        private string FacebookCallbackUrl { get; set; } = string.Empty;
        protected override void OnInitialized()
        {
            GoogleCallbackUrl =
                $"{ApiConstants.BaseUrl}api/Auth/GoogleLogin?returnUrl=" +
                NavigationManager.ToAbsoluteUri("/ExternalLogin").AbsoluteUri;

            FacebookCallbackUrl =
                $"{ApiConstants.BaseUrl}api/Auth/FacebookLogin?returnUrl=" +
                NavigationManager.ToAbsoluteUri("/ExternalLogin").AbsoluteUri;
        }
    }
}
