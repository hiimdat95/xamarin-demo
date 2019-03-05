using KamooniHost.Helpers;
using KamooniHost.IServices;
using KamooniHost.Models;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Extensions;
using Xamarin.Forms.TinyMVVM;

namespace KamooniHost.ViewModels
{
    public class SignInDescriptionViewModel : TinyViewModel
    {
        private readonly IHostService databaseService;

        private string scannedToken;

        public ICommand QRScanCommand { get; private set; }

        public SignInDescriptionViewModel(IHostService databaseService)
        {
            this.databaseService = databaseService;

            QRScanCommand = new AwaitCommand(QRScan);
        }

        private async void QRScan(object sender, TaskCompletionSource<bool> tcs)
        {
            if (await CameraHelper.CheckCameraPermission())
            {
                scannedToken = await BarcodeHelper.ScanQRCode();

                if (scannedToken != null)
                {
                    GetHostDetail();
                }
            }

            tcs.SetResult(true);
        }

        private void GetHostDetail()
        {
            if (IsBusy || !ConnectivityHelper.IsNetworkAvailable())
            {
                return;
            }

            IsBusy = true;
            Task.Run(async () =>
            {
                return await databaseService.GetHostDetails(scannedToken);
            }).ContinueWith(task => Device.BeginInvokeOnMainThread(() =>
            {
                IsBusy = false;
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    if (task.Result != null)
                    {
                        if (task.Result.Success)
                        {
                            OnLogInSuccess(task.Result.Host);
                        }
                        else if (!string.IsNullOrWhiteSpace(task.Result.Message))
                        {
                            CoreMethods.DisplayAlert("", task.Result.Message, TranslateExtension.GetValue("ok"));
                        }
                        else
                        {
                            CoreMethods.DisplayAlert("Request Fail", "Property not found", TranslateExtension.GetValue("ok"));
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

        private void OnLogInSuccess(Host host)
        {
            if (Helpers.Settings.ListHost.Find(h => h.Id == host.Id) is Host result)
            {
                Helpers.Settings.ListHost.Remove(result);
            }

            Helpers.Settings.ListHost.Add(host);

            Helpers.Settings.CurrentHost = host;

            Helpers.Settings.HostID = host.Id;
            Helpers.Settings.LoggedIn = true;

            Application.Current.MainPage = new MainPage();
        }
    }
}