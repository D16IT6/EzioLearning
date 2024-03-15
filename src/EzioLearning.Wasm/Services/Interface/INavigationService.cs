using MudBlazor;

namespace EzioLearning.Wasm.Services.Interface
{
    public interface INavigationService : IServiceBase
    {
        public Task Navigate(string url, string? message, int delaySeconds = 0, bool forceLoad = false, Severity severity = Severity.Info,
            Action<SnackbarOptions>? options = null);
    }
}
