using EzioLearning.Wasm.Services.Interface;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace EzioLearning.Wasm.Services.Implement
{
    public class NavigationService(ISnackbar snackbar,NavigationManager navigation) :INavigationService
    {

        public async Task Navigate(
            string url, 
            string? message, 
            int delaySeconds = 0, 
            bool forceLoad = false,
            Severity severity = Severity.Info,
            Action<SnackbarOptions>? options = null)
        {
            if (!string.IsNullOrEmpty(message))
                snackbar.Add(message, severity, options);

            await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
            navigation.NavigateTo(url,forceLoad:forceLoad);
        }
    }
}
