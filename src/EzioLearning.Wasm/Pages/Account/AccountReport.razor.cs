using Blazorise.DeepCloner;
using EzioLearning.Share.Dto.Learning.Course;
using EzioLearning.Wasm.Hubs;
using EzioLearning.Wasm.Services.Interface;
using EzioLearning.Wasm.Utils.Common;
using EzioLearning.Wasm.Utils.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;

namespace EzioLearning.Wasm.Pages.Account
{
    public partial class AccountReport : AccountComponentBase, IAsyncDisposable
    {
        [Inject] private HubConnectionManager HubConnectionManager { get; set; } = default!;
        [Inject] private IReportService ReportService { get; set; } = default!;
        [Inject] private ISnackbar Snackbar { get; set; } = default!;
        private List<MonthlyRevenueItem> CourseReportData { get; set; } = new();
        private HubConnection? ReportHubConnection { get; set; }

        private int SelectedYear { get; set; } = DateTime.UtcNow.Year;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            ReportHubConnection = await HubConnectionManager.GetHubConnectionAsync(HubConnectionEndpoints.Report, needAuthenticate: true);

            await GetReportData();

            await SubscribeReportFromHub();

        }

        //protected override void OnInitialized()
        //{
        //    for (int i = 1; i <= 12; ++i)
        //    {
        //        ChartRenders.Add(new ChartRender()
        //        {
        //            Month = i,
        //            MonthString = _months[i - 1]
        //        });
        //    }

        //}

        private readonly string[] _months = [
            "January", "February", "March",
            "April", "May", "June", "July",
            "August", "September", "October",
            "November", "December"];

        private List<ChartRender> ChartRenders { get; set; } = [];
        private async Task SubscribeReportFromHub()
        {
            if (ReportHubConnection == null)
                return;
            await ReportHubConnection.StartAsync();

            ReportHubConnection.On<IEnumerable<MonthlyRevenueItem>>("ReceiveMonthlyRevenue", async data =>
            {
                var dataList = data.ToList();
                CourseReportData.Clear();
                CourseReportData.AddRange(dataList);

                await MapDataToRenderList();
            });

            await ReportHubConnection.InvokeAsync("SendMonthlyRevenue");
        }


        private async Task GetReportData()
        {
            var response = await ReportService.GetCourseBetSellerReport(SelectedYear);
            if (response.IsSuccess)
            {
                CourseReportData = response.Data.ToList();
                StateHasChanged();

                await MapDataToRenderList();


            }
            else
            {
                Snackbar.Add("Không thể lấy dữ liệu báo cáo!", Severity.Error);
            }
        }

        private Task MapDataToRenderList()
        {
            var temp = ChartRenders.ShallowClone();
            ChartRenders.Clear();
            
            for (int i = 1; i <= 12; ++i)
            {

                var newItem = new ChartRender()
                {
                    MonthString = _months[i - 1],
                    Month = i,
                };
                var newData = CourseReportData.FirstOrDefault(x => x.Month == i);
                newItem.TotalPrice = (newData?.TotalPrice ?? 0) * 1.0 / 1e6;
                ChartRenders.Add(newItem);
            }
            StateHasChanged();
            return Task.CompletedTask;
        }


        public async ValueTask DisposeAsync()
        {
            if (ReportHubConnection != null)
            {
                await ReportHubConnection.StopAsync();

            }
        }
    }

    class ChartRender
    {
        public int Month { get; set; }
        public string? MonthString { get; set; }
        public double TotalPrice { get; set; }
    }
}
