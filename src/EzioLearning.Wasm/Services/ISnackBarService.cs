using EzioLearning.Core.Models.Response;

namespace EzioLearning.Wasm.Services
{
    public interface ISnackBarService
    {
        void ShowErrorFromResponse(ResponseBase response);
    }
}
