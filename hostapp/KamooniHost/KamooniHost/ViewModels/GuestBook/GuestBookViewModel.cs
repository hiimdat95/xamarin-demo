using KamooniHost.Helpers;
using KamooniHost.IServices;
using KamooniHost.Models;
using KamooniHost.Models.Result;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Extensions;
using Xamarin.Forms.TinyMVVM;

namespace KamooniHost.ViewModels
{
    public class GuestBookViewModel : TinyViewModel
    {
        private readonly IGuestBookService guestBookService;

        private List<GuestBook> listGuestOriginal = new List<GuestBook>();
        public ObservableCollection<GuestBook> ListGuest { get; set; } = new ObservableCollection<GuestBook>();

        public GuestScanResult guestScanResult;
        public GuestScanResult GuestScanResult { get => guestScanResult; set => SetProperty(ref guestScanResult, value); }

        public bool showScannedGuestProfile;
        public bool ShowScannedGuestProfile { get => showScannedGuestProfile; set => SetProperty(ref showScannedGuestProfile, value); }

        public ICommand ScanGuestCommand { get; private set; }
        public ICommand ReturnCommand { get; private set; }
        public ICommand ContinueScanningCommand { get; private set; }
        public ICommand ViewProfileCommand { get; private set; }
        public ICommand CheckOutCommand { get; private set; }

        public GuestBookViewModel(IGuestBookService guestBookService)
        {
            this.guestBookService = guestBookService;

            ScanGuestCommand = new AwaitCommand(ScanGuest);
            ReturnCommand = new AwaitCommand(Return);
            ContinueScanningCommand = new AwaitCommand(ContinueScanning);
            ViewProfileCommand = new AwaitCommand<GuestBook>(ViewProfile);
            CheckOutCommand = new AwaitCommand<GuestBook>(CheckOut);
        }

        public override void OnPageCreated()
        {
            base.OnPageCreated();

            if (!string.IsNullOrWhiteSpace(Helpers.Settings.CurrentHost.Url))
            {
                CurrentPage.ToolbarItems.Add(new ToolbarItem()
                {
                    Command = ScanGuestCommand,
                    Icon = "ic_scan_qr_white.png"
                });
            }

            GetListGuest();
        }

        private void GetListGuest()
        {
            if (IsBusy || !ConnectivityHelper.IsNetworkAvailable())
            {
                return;
            }

            IsBusy = true;
            Task.Run(async () =>
            {
                return await guestBookService.GetGuestBooks();
            }).ContinueWith(task => Device.BeginInvokeOnMainThread(() =>
            {
                IsBusy = false;
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    listGuestOriginal.Clear();
                    ListGuest.Clear();

                    if (task.Result != null)
                    {
                        if (task.Result.Success)
                        {
                            if (task.Result.GuestBooks != null && task.Result.GuestBooks.Count > 0)
                            {
                                listGuestOriginal = task.Result.GuestBooks.OrderByDescending(g => g.Date).ToList();
                                ListGuest.AddRange(listGuestOriginal);
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

        private async void ScanGuest(object sender, TaskCompletionSource<bool> tcs)
        {
            GuestScanResult = null;
            ShowScannedGuestProfile = false;

            var guestToken = await BarcodeHelper.ScanQRCode();

            if (guestToken != null)
            {
                ScanGuest(guestToken);
            }

            tcs.SetResult(true);
        }

        private void ScanGuest(string guestToken)
        {
            if (IsBusy || !ConnectivityHelper.IsNetworkAvailable())
            {
                return;
            }

            IsBusy = true;
            Task.Run(async () =>
            {
                return await guestBookService.ScanGuest(guestToken);
            }).ContinueWith(task => Device.BeginInvokeOnMainThread(() =>
            {
                IsBusy = false;
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    if (task.Result != null)
                    {
                        if (task.Result.Success)
                        {
                            GuestScanResult = task.Result;
                            ShowScannedGuestProfile = true;
                            GetListGuest();
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

        private void Return(object sender, TaskCompletionSource<bool> tcs)
        {
            GuestScanResult = null;
            ShowScannedGuestProfile = false;
            tcs.SetResult(true);
        }

        private async void ContinueScanning(object sender, TaskCompletionSource<bool> tcs)
        {
            GuestScanResult = null;
            ShowScannedGuestProfile = false;

            var guestToken = await BarcodeHelper.ScanQRCode();

            if (guestToken != null)
            {
                ScanGuest(guestToken);
            }

            tcs.SetResult(true);
        }

        private async void ViewProfile(GuestBook sender, TaskCompletionSource<bool> tcs)
        {
            var @params = new NavigationParameters
            {
                { Constants.ContentKey.GUEST_ID, sender.TravellerId },
                { Constants.ContentKey.GUEST_TOKEN, sender.VisitToken }
            };

            await CoreMethods.PushViewModel<GuestProfileViewModel>(@params);
            tcs.SetResult(true);
        }

        private void CheckOut(GuestBook sender, TaskCompletionSource<bool> tcs)
        {
            if (IsBusy || !ConnectivityHelper.IsNetworkAvailable())
            {
                return;
            }

            IsBusy = true;
            Task.Run(async () =>
            {
                return await guestBookService.GuestCheckOut(sender.VisitToken);
            }).ContinueWith(task => Device.BeginInvokeOnMainThread(() =>
            {
                IsBusy = false;
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    if (task.Result != null)
                    {
                        if (task.Result.Success)
                        {
                            GetListGuest();
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

            tcs.SetResult(true);
        }
    }
}