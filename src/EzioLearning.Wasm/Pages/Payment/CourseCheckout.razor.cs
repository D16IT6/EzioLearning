using EzioLearning.Wasm.Utils.Common;
using EzioLearning.Wasm.Utils.Extensions;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace EzioLearning.Wasm.Pages.Payment
{
    public partial class CourseCheckout : AccountComponentBase
    {
        [SupplyParameterFromQuery] public Guid OrderId { get; set; }
        [SupplyParameterFromQuery] public bool Success { get; set; }

        [Inject] private ISnackbar Snackbar { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            if (Success)
            {
                Snackbar.Add("Thanh toán thành công, vào học ngay!", Severity.Success);
            }
            else
            {
                Snackbar.Add("Thanh toán thất bại, vui lòng thử lại!", Severity.Error);

            }
            await Task.Delay(2000);

            NavigationManager.NavigateTo(RouteConstants.Course);
            //NavigationManager.NavigateTo(CourseId != Guid.Empty
            //    ? Path.Combine(RouteConstants.CourseRoute.CourseDetailNoParam, CourseId.ToString())
            //    : RouteConstants.Course);
        }
    }
}
