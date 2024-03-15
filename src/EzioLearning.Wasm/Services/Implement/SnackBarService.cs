using EzioLearning.Share.Models.Response;
using EzioLearning.Wasm.Services.Interface;
using MudBlazor;

namespace EzioLearning.Wasm.Services.Implement;

public class SnackBarService(ISnackbar snackbar) : ISnackBarService
{
    public void ShowErrorFromResponse(ResponseBase response)
    {
        if (!response.Errors.Any()) return;

        var error =
            response.Errors.Where(dataError => dataError.Value.Any())
                .Select(dataError => dataError.Value[0]).First();
            snackbar.Add(error, Severity.Error, option => { option.ActionColor = Color.Error; });
    }
}