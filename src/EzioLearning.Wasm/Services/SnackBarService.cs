﻿using EzioLearning.Core.Models.Response;
using MudBlazor;

namespace EzioLearning.Wasm.Services
{
    public class SnackBarService(ISnackbar snackbar) : ISnackBarService
    {
        public void ShowErrorFromResponse(ResponseBase response)
        {
            if (!response!.Errors.Any()) return;

            foreach (var error in
                    response.Errors.Where(dataError => dataError.Value.Any())
                        .SelectMany(dataError => dataError.Value))
            {
                snackbar.Add(error, Severity.Error, option =>
                {
                    option.ActionColor = Color.Error;
                });
            }
        }
    }
}
