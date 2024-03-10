using EzioLearning.Share.Models.Response;

namespace EzioLearning.Wasm.Services.Interface;

public interface ISnackBarService: IServiceBase
{
    void ShowErrorFromResponse(ResponseBase response);
}