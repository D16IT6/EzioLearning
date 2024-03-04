using EzioLearning.Share.Models.Response;

namespace EzioLearning.Wasm.Services;

public interface ISnackBarService
{
    void ShowErrorFromResponse(ResponseBase response);
}