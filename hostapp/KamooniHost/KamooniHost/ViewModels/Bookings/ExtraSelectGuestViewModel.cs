using KamooniHost.Constants;
using KamooniHost.Helpers;
using KamooniHost.IServices;
using KamooniHost.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Extensions;
using Xamarin.Forms.TinyMVVM;

namespace KamooniHost.ViewModels
{
    public class ExtraSelectGuestViewModel : TinyViewModel
    {
        private readonly IBookingsService openBookingsService;

        private Booking booking;

        private List<GuestBill> listGuestBillOriginal = new List<GuestBill>();
        private List<GuestBill> listGuestBill = new List<GuestBill>();
        public List<GuestBill> ListGuestBill { get => listGuestBill; set => SetProperty(ref listGuestBill, value); }

        private bool isSearchingGuest;
        public bool IsSearchingGuest { get => isSearchingGuest; set => SetProperty(ref isSearchingGuest, value); }

        public ICommand StartSearchGuestCommand { get; private set; }
        public ICommand SearchGuestCommand { get; private set; }
        public ICommand CancelSearchGuestCommand { get; private set; }
        public ICommand GuestSelectedCommand { get; private set; }

        public ExtraSelectGuestViewModel(IBookingsService openBookingsService)
        {
            this.openBookingsService = openBookingsService;

            StartSearchGuestCommand = new AwaitCommand(StartSearchGuest);
            SearchGuestCommand = new AwaitCommand<string>(SearchGuest);
            CancelSearchGuestCommand = new AwaitCommand(CancelSearchGuest);
            GuestSelectedCommand = new AwaitCommand<GuestBill>(GuestSelected);
        }

        public override void Init(object data)
        {
            base.Init(data);

            booking = (Booking)data;
        }

        public override void OnPageCreated()
        {
            base.OnPageCreated();

            MessagingCenter.Unsubscribe<ExtraSummaryViewModel>(this, MessageKey.EXTRA_ITEMS_ADDED);
            MessagingCenter.Subscribe<ExtraSummaryViewModel>(this, MessageKey.EXTRA_ITEMS_ADDED, OnItemAdded);

            GetBills();
        }

        private void OnItemAdded(ExtraSummaryViewModel sender)
        {
            GetBills();
            MessagingCenter.Send(this, MessageKey.BILL_CHANGED);
        }

        private void GetBills()
        {
            if (IsBusy || !ConnectivityHelper.IsNetworkAvailable())
            {
                return;
            }

            IsBusy = true;
            Task.Run(async () =>
            {
                return await openBookingsService.GetBills(booking.Id);
            }).ContinueWith(task => Device.BeginInvokeOnMainThread(() =>
            {
                IsBusy = false;
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    if (task.Result?.Success ?? false)
                    {
                        listGuestBillOriginal = task.Result.GuestBills;
                        ListGuestBill = task.Result.GuestBills;
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

        private void StartSearchGuest(object sender, TaskCompletionSource<bool> tcs)
        {
            IsSearchingGuest = true;
            tcs.SetResult(true);
        }

        private void SearchGuest(string sender, TaskCompletionSource<bool> tcs)
        {
            if (string.IsNullOrWhiteSpace(sender))
            {
                ListGuestBill = new List<GuestBill>(listGuestBillOriginal);
            }
            else
            {
                ListGuestBill = new List<GuestBill>(listGuestBillOriginal.FindAll(b => b.GuestDetails.Name.UnSignContains(sender)));
            }
            tcs.SetResult(true);
        }

        private void CancelSearchGuest(object sender, TaskCompletionSource<bool> tcs)
        {
            IsSearchingGuest = false;
            tcs.SetResult(true);
        }

        private async void GuestSelected(GuestBill sender, TaskCompletionSource<bool> tcs)
        {
            var @params = new NavigationParameters()
            {
                { ContentKey.BOOKING, booking },
                { ContentKey.SELECTED_GUEST, sender?.DeepCopy() }
            };
            await CoreMethods.PushViewModel<ExtraAddViewModel>(@params);
            tcs.SetResult(true);
        }
    }
}