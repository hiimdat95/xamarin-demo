using KamooniHost.Helpers;
using KamooniHost.IServices;
using KamooniHost.Models;
using KamooniHost.Models.Result;
using KamooniHost.Utils;
using Stormlion.ImageCropper;
using System;
using System.ComponentModel;
using System.Net.Mail;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Extensions;
using Xamarin.Forms.TinyMVVM;

namespace KamooniHost.ViewModels
{
    public class CheckInNonFullUserViewModel : TinyViewModel
    {
        private readonly AppSettings AppSettings = Helpers.Settings.AppSettings;

        private readonly IGuestBookService guestBookService;

        private string guestToken;

        public GuestCheckIn guestCheckIn = new GuestCheckIn();
        public GuestCheckIn GuestCheckIn { get => guestCheckIn; set => SetProperty(ref guestCheckIn, value); }

        public GuestScanResult guestScanResult;
        public GuestScanResult GuestScanResult { get => guestScanResult; set => SetProperty(ref guestScanResult, value); }

        private string voteNote;
        public string VoteNote { get => voteNote; set => SetProperty(ref voteNote, value); }

        private bool voteNoteVisible;
        public bool VoteNoteVisible { get => voteNoteVisible; set => SetProperty(ref voteNoteVisible, value); }

        private bool nationalitySelectVisible;
        public bool NationalitySelectVisible { get => nationalitySelectVisible; set => SetProperty(ref nationalitySelectVisible, value); }

        public bool showScannedGuestProfile;
        public bool ShowScannedGuestProfile { get => showScannedGuestProfile; set => SetProperty(ref showScannedGuestProfile, value); }

        private bool isPassportImageTaked;
        public bool IsPassportImageTaked { get => isPassportImageTaked; set => SetProperty(ref isPassportImageTaked, value); }

        private bool isUserValidated;
        public bool IsUserValidated { get => isUserValidated; set => SetProperty(ref isUserValidated, value); }

        public bool TermsVisible { get; set; } = !string.IsNullOrWhiteSpace(Helpers.Settings.CurrentHost?.Terms);

        public ICommand ScanGuestCommand { get; private set; }
        public ICommand UpVoteGuestCommand { get; private set; }
        public ICommand DownVoteGuestCommand { get; private set; }
        public ICommand CloseVoteNoteCommand { get; private set; }
        public ICommand DownVoteGuestConfirmCommand { get; private set; }
        public ICommand GoHomeCommand { get; private set; }
        public ICommand ContinueCheckInCommand { get; private set; }
        public ICommand SelectNationalityCommand { get; private set; }
        public ICommand NationalitySelectedCommand { get; private set; }
        public ICommand TakePassportImageCommand { get; private set; }
        public ICommand ConfirmCheckInCommand { get; private set; }

        public CheckInNonFullUserViewModel(IGuestBookService guestBookService)
        {
            this.guestBookService = guestBookService;

            ScanGuestCommand = new AwaitCommand(ScanGuest);
            UpVoteGuestCommand = new AwaitCommand(UpVoteGuest);
            DownVoteGuestCommand = new AwaitCommand(DownVoteGuest);
            CloseVoteNoteCommand = new AwaitCommand(CloseVoteNote);
            DownVoteGuestConfirmCommand = new AwaitCommand(DownVoteGuestConfirm);
            GoHomeCommand = new AwaitCommand(GoHome);
            ContinueCheckInCommand = new AwaitCommand(ContinueCheckIn);
            SelectNationalityCommand = new AwaitCommand(SelectNationality);
            NationalitySelectedCommand = new AwaitCommand<Country>(NationalitySelected);
            TakePassportImageCommand = new Command(TakePassportImage);
            ConfirmCheckInCommand = new AwaitCommand(ConfirmCheckIn);

            GuestCheckIn.PropertyChanged += GuestCheckIn_PropertyChanged;
        }

        private void GuestCheckIn_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            IsUserValidated = (!string.IsNullOrWhiteSpace(GuestCheckIn.Name)
                                && !string.IsNullOrWhiteSpace(GuestCheckIn.Mobile)
                                && !string.IsNullOrWhiteSpace(GuestCheckIn.Email)
                                && !string.IsNullOrWhiteSpace(GuestCheckIn.Country)
                                && !string.IsNullOrWhiteSpace(GuestCheckIn.Passport)
                                && !string.IsNullOrWhiteSpace(GuestCheckIn.PassportImage));
        }

        private async void ScanGuest(object sender, TaskCompletionSource<bool> tcs)
        {
            if (TermsVisible)
            {
                tcs.SetResult(true);
                return;
            }

            GuestScanResult = null;
            ShowScannedGuestProfile = false;

            var guestToken = await BarcodeHelper.ScanQRCode(AppSettings.CheckInUseFrontCamera);
            //var guestToken = "4FE45OU3GEHZVASQGQS";

            if (guestToken != null)
            {
                ScanGuest(guestToken);

                this.guestToken = guestToken;
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
                            var country = CountryUtil.GetCountryByISO(GuestScanResult.Country);
                            if (country != null)
                            {
                                GuestScanResult.CountryName = country.Name;

                                try
                                {
                                    if (country.Flag != null)
                                    {
                                        var Assembly = typeof(App).GetTypeInfo().Assembly;

                                        var strAssemblyName = Assembly.GetName().Name;
                                        var resource = "AppResources.Assets";
                                        GuestScanResult.CountryFlag = ImageSource.FromResource($"{strAssemblyName}.{resource}.{country.Flag}", Assembly);
                                    }
                                }
                                catch { }
                            }
                            ShowScannedGuestProfile = true;
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

        private void UpVoteGuest(object sender, TaskCompletionSource<bool> tcs)
        {
            if (IsBusy || !ConnectivityHelper.IsNetworkAvailable())
            {
                return;
            }

            IsBusy = true;
            Task.Run(async () =>
            {
                return await guestBookService.UpVoteGuest(guestToken);
            }).ContinueWith(task => Device.BeginInvokeOnMainThread(() =>
            {
                IsBusy = false;
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    if (task.Result != null)
                    {
                        if (task.Result.Success)
                        {
                            CoreMethods.DisplayAlert("", "Report Success", TranslateExtension.GetValue("ok"));
                            GuestScanResult.IsVerified = true;
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
                tcs.SetResult(true);
            }));
        }

        private void DownVoteGuest(object sender, TaskCompletionSource<bool> tcs)
        {
            VoteNote = "";
            VoteNoteVisible = true;

            tcs.SetResult(true);
        }

        private void CloseVoteNote(object sender, TaskCompletionSource<bool> tcs)
        {
            VoteNoteVisible = false;

            tcs.SetResult(true);
        }

        private void DownVoteGuestConfirm(object sender, TaskCompletionSource<bool> tcs)
        {
            if (IsBusy || !ConnectivityHelper.IsNetworkAvailable())
            {
                return;
            }

            IsBusy = true;
            Task.Run(async () =>
            {
                return await guestBookService.DownVoteGuest(guestToken, VoteNote);
            }).ContinueWith(task => Device.BeginInvokeOnMainThread(() =>
            {
                IsBusy = false;
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    if (task.Result != null)
                    {
                        if (task.Result.Success)
                        {
                            CoreMethods.DisplayAlert("", "Report Success", TranslateExtension.GetValue("ok"));
                            VoteNoteVisible = false;
                            GuestScanResult.IsVerified = true;
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
                tcs.SetResult(true);
            }));
        }

        private void GoHome(object sender, TaskCompletionSource<bool> tcs)
        {
            if (Application.Current.MainPage is MasterDetailPage masterDetailPage)
            {
                if (masterDetailPage.Detail is INavigationService detailPage1)
                {
                    detailPage1.NotifyChildrenPageWasPopped();
                }
                else if (masterDetailPage.Detail is NavigationPage detailPage2)
                {
                    detailPage2.NotifyAllChildrenPopped();
                }
            }
            Application.Current.MainPage = new MainPage();

            tcs.SetResult(true);
        }

        private void ContinueCheckIn(object sender, TaskCompletionSource<bool> tcs)
        {
            GuestScanResult = null;
            ShowScannedGuestProfile = false;

            GuestCheckIn = new GuestCheckIn();

            tcs.SetResult(true);
        }

        private void SelectNationality(object sender, TaskCompletionSource<bool> tcs)
        {
            NationalitySelectVisible = true;

            tcs.SetResult(true);
        }

        private void NationalitySelected(Country sender, TaskCompletionSource<bool> tcs)
        {
            GuestCheckIn.Country = sender.Code;
            GuestCheckIn.CountryName = sender.Name;
            GuestCheckIn.CountryFlag = sender.Flag;

            NationalitySelectVisible = false;

            tcs.SetResult(true);
        }

        private async void TakePassportImage(object sender)
        {
            string imagePath = await PhotoHelper.TakePhotoPathAsync();

            if (!string.IsNullOrWhiteSpace(imagePath))
            {
                try
                {
                    new ImageCropper()
                    {
                        PageTitle = "Passport",
                        AspectRatioX = 4,
                        AspectRatioY = 3,
                        Success = (imageFile) =>
                        {
                            Device.BeginInvokeOnMainThread(async () =>
                            {
                                GuestCheckIn.PassportImage = Convert.ToBase64String(await StorageHelper.LoadImage(imageFile));
                                IsPassportImageTaked = true;
                            });
                        }
                    }.Show(CurrentPage, imagePath);
                }
                catch (Exception ex)
                {
                    await CoreMethods.DisplayAlert(TranslateExtension.GetValue("error"), ex.Message, TranslateExtension.GetValue("ok"));
                }
            }
        }

        private async void ConfirmCheckIn(object sender, TaskCompletionSource<bool> tcs)
        {
            if (await CoreMethods.DisplayAlert("", "Are you sure to Check-In?", "Yes", "No"))
            {
                ManualGuestCheckIn();
            }

            tcs.SetResult(true);
        }

        private void ManualGuestCheckIn()
        {
            try
            {
                new MailAddress(GuestCheckIn.Email);
            }
            catch
            {
                CoreMethods.DisplayAlert("", "Email is invalid", TranslateExtension.GetValue("ok"));
                return;
            }

            if (IsBusy || !ConnectivityHelper.IsNetworkAvailable())
            {
                return;
            }

            IsBusy = true;
            Task.Run(async () =>
            {
                return await guestBookService.ManualCheckIn(GuestCheckIn);
            }).ContinueWith(task => Device.BeginInvokeOnMainThread(() =>
            {
                IsBusy = false;
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    if (task.Result != null)
                    {
                        if (task.Result.Success)
                        {
                            CoreMethods.DisplayAlert("", "Check In Success", TranslateExtension.GetValue("ok"));

                            GuestCheckIn = new GuestCheckIn();
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