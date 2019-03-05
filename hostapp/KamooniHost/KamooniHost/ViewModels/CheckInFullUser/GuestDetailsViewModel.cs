using KamooniHost.Helpers;
using KamooniHost.IServices;
using KamooniHost.Models;
using KamooniHost.Utils;
using Stormlion.ImageCropper;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Controls;
using Xamarin.Forms.Extensions;
using Xamarin.Forms.TinyMVVM;

namespace KamooniHost.ViewModels
{
    public class GuestDetailsViewModel : TinyViewModel
    {
        private readonly AppSettings AppSettings = Helpers.Settings.AppSettings;

        private readonly IGuestBookService guestBookService;
        private readonly ICheckInService checkInService;

        private CheckInFormItem selectedCheckInFormItem;

        private ToCheckInBooking guest;

        private int currentTab;
        public int CurrentTab { get => currentTab; set => SetProperty(ref currentTab, value); }

        public ObservableCollection<CheckInFormItem> ListCheckInFormItem { get; set; } = new ObservableCollection<CheckInFormItem>();

        public ObservableCollection<TabbedItem> ListTabbedItem { get; set; } = new ObservableCollection<TabbedItem>();

        public ObservableCollection<CheckInFormRoom> ListRoom { get; set; } = new ObservableCollection<CheckInFormRoom>();

        private string voteNote;
        public string VoteNote { get => voteNote; set => SetProperty(ref voteNote, value); }

        private bool voteNoteVisible;
        public bool VoteNoteVisible { get => voteNoteVisible; set => SetProperty(ref voteNoteVisible, value); }

        private bool roomSelectVisible;
        public bool RoomSelectVisible { get => roomSelectVisible; set => SetProperty(ref roomSelectVisible, value); }

        private bool nationalitySelectVisible;
        public bool NationalitySelectVisible { get => nationalitySelectVisible; set => SetProperty(ref nationalitySelectVisible, value); }

        private bool isAllSaved;
        public bool IsAllSaved { get => isAllSaved; set => SetProperty(ref isAllSaved, value); }

        public ICommand ScanTravellerQRCommand { get; private set; }
        public ICommand UpVoteGuestCommand { get; private set; }
        public ICommand DownVoteGuestCommand { get; private set; }
        public ICommand CloseVoteNoteCommand { get; private set; }
        public ICommand DownVoteGuestConfirmCommand { get; private set; }
        public ICommand SelectNationalityCommand { get; private set; }
        public ICommand NationalitySelectedCommand { get; private set; }
        public ICommand SelectRoomCommand { get; private set; }
        public ICommand CloseRoomSelectCommand { get; private set; }
        public ICommand RoomSelectedCommand { get; private set; }
        public ICommand TakePassportImageCommand { get; private set; }
        public ICommand PostGuestDetailsCommand { get; private set; }
        public ICommand ConfirmCheckInCommand { get; private set; }

        public GuestDetailsViewModel(IGuestBookService guestBookService, ICheckInService checkInService)
        {
            this.guestBookService = guestBookService;
            this.checkInService = checkInService;

            ScanTravellerQRCommand = new AwaitCommand(ScanTravellerQR);
            UpVoteGuestCommand = new AwaitCommand(UpVoteGuest);
            DownVoteGuestCommand = new AwaitCommand(DownVoteGuest);
            CloseVoteNoteCommand = new AwaitCommand(CloseVoteNote);
            DownVoteGuestConfirmCommand = new AwaitCommand(DownVoteGuestConfirm);
            SelectNationalityCommand = new AwaitCommand<CheckInFormItem>(SelectNationality);
            NationalitySelectedCommand = new AwaitCommand<Country>(NationalitySelected);
            SelectRoomCommand = new AwaitCommand<CheckInFormItem>(SelectRoom);
            CloseRoomSelectCommand = new AwaitCommand(CloseRoomSelect);
            RoomSelectedCommand = new AwaitCommand<CheckInFormRoom>(RoomSelected);
            TakePassportImageCommand = new AwaitCommand<CheckInFormItem>(TakePassportImage);
            PostGuestDetailsCommand = new AwaitCommand<CheckInFormItem>(PostGuestDetails);
            ConfirmCheckInCommand = new AwaitCommand(ConfirmCheckIn);
        }

        public override void Init(object data)
        {
            base.Init(data);

            guest = (ToCheckInBooking)data;
        }

        public override void OnPageCreated()
        {
            base.OnPageCreated();

            GetGuestDetails();
        }

        public void GetGuestDetails()
        {
            if (IsBusy || !ConnectivityHelper.IsNetworkAvailable())
            {
                return;
            }

            IsBusy = true;
            Task.Run(async () =>
            {
                return await checkInService.GetGuestDetails(guest.BookingId);
            }).ContinueWith(task => Device.BeginInvokeOnMainThread(async () =>
            {
                IsBusy = false;
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    ListCheckInFormItem.Clear();
                    ListTabbedItem.Clear();
                    ListRoom.Clear();

                    if (task.Result != null)
                    {
                        if (task.Result.Success)
                        {
                            for (int i = 0; i < task.Result.CheckInForm.Count; i++)
                            {
                                var item = task.Result.CheckInForm[i];
                                item.Title = TranslateExtension.GetValue("title_activity_guest") + " " + (i + 1);
                                var country = CountryUtil.GetCountryByISO(item.CountryISO);
                                if (country != null)
                                {
                                    item.CountryName = country.Name;
                                    item.CountryFlag = country.Flag;
                                }
                                item.IsSaved = item.IsValid;
                                ListCheckInFormItem.Add(item);

                                ListTabbedItem.Add(new TabbedItem()
                                {
                                    HeaderText = TranslateExtension.GetValue("title_activity_guest") + " " + (i + 1),
                                    BindingContext = item
                                });
                            }
                            foreach (var item in task.Result.CheckInForm)
                            {
                                ListRoom.Add(new CheckInFormRoom()
                                {
                                    CheckInFormItemId = item.Id,
                                    Name = item.Room
                                });
                            }

                            IsAllSaved = !ListCheckInFormItem.ToList().Any(c => !c.IsSaved);
                        }
                        else if (!string.IsNullOrWhiteSpace(task.Result?.Message))
                        {
                            await CoreMethods.DisplayAlert("", task.Result.Message, TranslateExtension.GetValue("ok"));

                            await CoreMethods.PopViewModel();
                        }
                    }
                    else
                    {
                        await CoreMethods.DisplayAlert("", TranslateExtension.GetValue("dialog_message_server_error"), TranslateExtension.GetValue("ok"));

                        await CoreMethods.PopViewModel();
                    }
                }
                else if (task.IsFaulted)
                {
                    await CoreMethods.DisplayAlert(TranslateExtension.GetValue("error"), task.Exception?.GetBaseException().Message, TranslateExtension.GetValue("ok"));
                }
            }));
        }

        private async void ScanTravellerQR(object sender, TaskCompletionSource<bool> tcs)
        {
            if (IsBusy || VoteNoteVisible || !(ListTabbedItem[CurrentTab].BindingContext is CheckInFormItem selectedGuest))
            {
                tcs.SetResult(true);
                return;
            }

            if (await CameraHelper.CheckCameraPermission())
            {
                selectedGuest.GuestToken = await BarcodeHelper.ScanQRCode(AppSettings.CheckInUseFrontCamera);
                //selectedGuest.GuestToken = "4FE45OU3GEHZVASQGQS";

                if (!string.IsNullOrWhiteSpace(selectedGuest.GuestToken))
                {
                    GetGuestDetailsFromQR(selectedGuest);
                }
            }
            tcs.SetResult(true);
        }

        private void GetGuestDetailsFromQR(CheckInFormItem sender)
        {
            if (IsBusy || !ConnectivityHelper.IsNetworkAvailable())
            {
                return;
            }

            IsBusy = true;
            Task.Run(async () =>
            {
                return await checkInService.ScanGuest(sender.GuestToken);
            }).ContinueWith(task => Device.BeginInvokeOnMainThread(() =>
            {
                IsBusy = false;
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    if (task.Result?.Success ?? false)
                    {
                        sender.Name = task.Result.GuestName;
                        sender.Email = task.Result.Email;
                        sender.Mobile = task.Result.Mobile;
                        sender.PassportId = task.Result.PassportId;
                        sender.CountryISO = task.Result.Country;
                        if (CountryUtil.GetCountryByISO(sender.CountryISO) is Country country)
                        {
                            sender.CountryName = country.Name;
                            sender.CountryFlag = country.Flag;
                        }
                        sender.PassportImage = task.Result.PassportImage;
                        sender.IsVerified = task.Result.IsVerified;
                        sender.IsScanned = true;
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(task.Result?.Message))
                            CoreMethods.DisplayAlert("", task.Result.Message, TranslateExtension.GetValue("ok"));
                        else
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

            if (!(ListTabbedItem[CurrentTab].BindingContext is CheckInFormItem selectedGuest))
            {
                tcs.SetResult(true);
                return;
            }

            IsBusy = true;
            Task.Run(async () =>
            {
                return await guestBookService.UpVoteGuest(selectedGuest.GuestToken);
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
                            selectedGuest.IsVerified = true;
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

            if (!(ListTabbedItem[CurrentTab].BindingContext is CheckInFormItem seletedGuest))
            {
                tcs.SetResult(true);
                return;
            }

            IsBusy = true;
            Task.Run(async () =>
            {
                return await guestBookService.DownVoteGuest(seletedGuest.GuestToken, VoteNote);
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
                            seletedGuest.IsVerified = true;
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

        private void SelectNationality(CheckInFormItem sender, TaskCompletionSource<bool> tcs)
        {
            selectedCheckInFormItem = sender;
            NationalitySelectVisible = true;
            tcs.SetResult(true);
        }

        private void NationalitySelected(Country sender, TaskCompletionSource<bool> tcs)
        {
            // Todo
            if (selectedCheckInFormItem != null)
            {
                selectedCheckInFormItem.CountryISO = sender.Code;
                selectedCheckInFormItem.CountryName = sender.Name;
                selectedCheckInFormItem.CountryFlag = sender.Flag;
            }
            NationalitySelectVisible = false;
            tcs.SetResult(true);
        }

        private void SelectRoom(CheckInFormItem sender, TaskCompletionSource<bool> tcs)
        {
            selectedCheckInFormItem = sender;
            RoomSelectVisible = true;
            tcs.SetResult(true);
        }

        private void CloseRoomSelect(object sender, TaskCompletionSource<bool> tcs)
        {
            RoomSelectVisible = false;
            tcs.SetResult(true);
        }

        private void RoomSelected(CheckInFormRoom sender, TaskCompletionSource<bool> tcs)
        {
            if (selectedCheckInFormItem != null)
            {
                selectedCheckInFormItem.Room = sender.Name;
            }
            RoomSelectVisible = false;
            tcs.SetResult(true);
        }

        private async void TakePassportImage(CheckInFormItem sender, TaskCompletionSource<bool> tcs)
        {
            if (sender == null)
            {
                tcs.SetResult(true);
                return;
            }

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
                                sender.PassportImage = Convert.ToBase64String(await StorageHelper.LoadImage(imageFile));
                            });
                        }
                    }.Show(CurrentPage, imagePath);
                }
                catch (Exception ex)
                {
                    await CoreMethods.DisplayAlert(TranslateExtension.GetValue("error"), ex.Message, TranslateExtension.GetValue("ok"));
                }
            }

            tcs.SetResult(true);
        }

        private void PostGuestDetails(CheckInFormItem sender, TaskCompletionSource<bool> tcs)
        {
            if (IsBusy || !ConnectivityHelper.IsNetworkAvailable())
            {
                return;
            }

            IsBusy = true;
            Task.Run(async () =>
            {
                return await checkInService.PostGuestDetails(sender);
            }).ContinueWith(task => Device.BeginInvokeOnMainThread(() =>
            {
                IsBusy = false;
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    if (task.Result?.Success ?? false)
                    {
                        sender.IsSaved = true;

                        IsAllSaved = !ListCheckInFormItem.ToList().Any(c => !c.IsSaved);
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(task.Result?.Message))
                            CoreMethods.DisplayAlert("", task.Result.Message, TranslateExtension.GetValue("ok"));
                        else
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

        private void ConfirmCheckIn(object sender, TaskCompletionSource<bool> tcs)
        {
            if (IsBusy || !ConnectivityHelper.IsNetworkAvailable())
            {
                return;
            }

            IsBusy = true;
            Task.Run(async () =>
            {
                return await checkInService.ConfirmCheckIn(guest.BookingId);
            }).ContinueWith(task => Device.BeginInvokeOnMainThread(async () =>
            {
                IsBusy = false;
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    if (task.Result?.Success ?? false)
                    {
                        await CoreMethods.DisplayAlert(TranslateExtension.GetValue("dialog_title_check_in"), TranslateExtension.GetValue("dialog_message_check_in_confirmed"), TranslateExtension.GetValue("ok"));

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
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(task.Result?.Message))
                            await CoreMethods.DisplayAlert("", task.Result.Message, TranslateExtension.GetValue("ok"));
                        else
                            await CoreMethods.DisplayAlert("", TranslateExtension.GetValue("dialog_message_server_error"), TranslateExtension.GetValue("ok"));
                    }
                }
                else if (task.IsFaulted)
                {
                    await CoreMethods.DisplayAlert(TranslateExtension.GetValue("error"), task.Exception?.GetBaseException().Message, TranslateExtension.GetValue("ok"));
                }
            }));
            tcs.SetResult(true);
        }
    }
}