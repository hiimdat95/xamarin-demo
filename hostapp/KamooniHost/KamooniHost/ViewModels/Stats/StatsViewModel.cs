using KamooniHost.Helpers;
using KamooniHost.IServices;
using KamooniHost.Models;
using Microcharts;
using SkiaSharp;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Extensions;
using Xamarin.Forms.TinyMVVM;

namespace KamooniHost.ViewModels
{
    public class StatsViewModel : TinyViewModel
    {
        private readonly IStatsService statsService;

        private Revenue revenue;
        public Revenue Revenue { get => revenue; set => SetProperty(ref revenue, value); }

        private DonutChart revenuewDonutChart;
        public DonutChart RevenuewDonutChart { get => revenuewDonutChart; set => SetProperty(ref revenuewDonutChart, value); }

        public StatsViewModel(IStatsService statsService)
        {
            this.statsService = statsService;
        }

        public override void OnPageCreated()
        {
            base.OnPageCreated();

            GetRevenue();
        }

        private void GetRevenue()
        {
            if (IsBusy || !ConnectivityHelper.IsNetworkAvailable())
            {
                return;
            }

            IsBusy = true;
            Task.Run(async () =>
            {
                return await statsService.GetRevenue();
            }).ContinueWith(task => Device.BeginInvokeOnMainThread(() =>
            {
                IsBusy = false;
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    if (task.Result != null)
                    {
                        if (task.Result.Success)
                        {
                            if (task.Result.Revenue != null)
                            {
                                Revenue = task.Result.Revenue;

                                var entries = new[]
                                {
                                    new Microcharts.Entry((float)task.Result.Revenue.Paid)
                                    {
                                        Label = "Paid",
                                        ValueLabel = task.Result.Revenue.Paid.ToString("###,###,###,###.##"),
                                        Color = SKColor.Parse("#6bc5a9")
                                    },
                                    new Microcharts.Entry((float)task.Result.Revenue.Balance)
                                    {
                                        Label = "Balance",
                                        ValueLabel = task.Result.Revenue.Balance.ToString("###,###,###,###.##"),
                                        Color = SKColor.Parse("#835e7e")
                                    }
                                };

                                RevenuewDonutChart = new DonutChart()
                                {
                                    LabelTextSize = 32,
                                    LabelColor = SKColors.Black,
                                    Entries = entries
                                };
                            }
                        }
                        else if (!string.IsNullOrWhiteSpace(task.Result.Message))
                        {
                            CoreMethods.DisplayAlert("", task.Result.Message, TranslateExtension.GetValue("ok"));
                        }
                    }
                    else
                    {
                        CoreMethods.DisplayAlert("", TranslateExtension.GetValue("dialog_message_server_error"), TranslateExtension.GetValue("ok"));
                    }
                }
                else if (task.IsFaulted)
                {
                    CoreMethods.DisplayAlert(TranslateExtension.GetValue("error"), task.Exception?.GetBaseException().Message, TranslateExtension.GetValue("ok"));
                }
            }));
        }
    }
}