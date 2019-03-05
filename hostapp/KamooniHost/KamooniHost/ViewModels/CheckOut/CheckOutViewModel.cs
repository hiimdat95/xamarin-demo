using KamooniHost.Constants;
using KamooniHost.Helpers;
using KamooniHost.IServices;
using KamooniHost.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Extensions;
using Xamarin.Forms.TinyMVVM;

namespace KamooniHost.ViewModels
{
    public class CheckOutViewModel : TinyViewModel
    {
        private readonly IBookingsService bookingsService;

        private CancellationTokenSource cts;

        private ObservableCollection<Booking> listCheckedInBooking = new ObservableCollection<Booking>();
        public ObservableCollection<Booking> ListCheckedInBooking { get => listCheckedInBooking; set => SetProperty(ref listCheckedInBooking, value); }

        private List<Booking> listSearchResultBooking = new List<Booking>();
        public List<Booking> ListSearchResultBooking { get => listSearchResultBooking; set => SetProperty(ref listSearchResultBooking, value); }

        private bool isLoading;
        public bool IsLoading { get => isLoading; set => SetProperty(ref isLoading, value); }

        private string searchKey;
        public string SearchKey { get => searchKey; set => SetProperty(ref searchKey, value); }

        private bool isSearching;
        public bool IsSearching { get => isSearching; set => SetProperty(ref isSearching, value); }

        private bool searchResultVisible;
        public bool SearchResultVisible { get => searchResultVisible; set => SetProperty(ref searchResultVisible, value); }

        private bool emptyMessageVisible;
        public bool EmptyMessageVisible { get => emptyMessageVisible; set => SetProperty(ref emptyMessageVisible, value); }

        private bool isCheckingOut;
        public bool IsCheckingOut { get => isCheckingOut; set => SetProperty(ref isCheckingOut, value); }

        public ICommand StartSearchBookingsCommand { get; private set; }
        public ICommand StopSearchBookingsCommand { get; private set; }
        public ICommand GetCheckedInBookingsCommand { get; private set; }
        public ICommand ViewBillsCommand { get; private set; }
        public ICommand AddExtrasCommand { get; private set; }
        public ICommand ProceedCommand { get; private set; }
        public ICommand PayNowCommand { get; private set; }
        public ICommand CheckOutCommand { get; private set; }

        public CheckOutViewModel(IBookingsService bookingsService)
        {
            this.bookingsService = bookingsService;

            StartSearchBookingsCommand = new AwaitCommand(StartSearchBookings);
            StopSearchBookingsCommand = new AwaitCommand(StopSearchBookings);
            GetCheckedInBookingsCommand = new Command(GetCheckedInBookings);
            ViewBillsCommand = new AwaitCommand<Booking>(ViewBills);
            AddExtrasCommand = new AwaitCommand<Booking>(AddExtras);
            ProceedCommand = new AwaitCommand<Booking>(Proceed);
            PayNowCommand = new AwaitCommand<Booking>(PayNow);
            CheckOutCommand = new AwaitCommand<Booking>(CheckOut);
        }

        public override void OnPageCreated()
        {
            base.OnPageCreated();

            MessagingCenter.Subscribe<ViewBillsViewModel>(this, MessageKey.BILL_CHANGED, OnBillsChanged);
            MessagingCenter.Subscribe<ExtraSelectGuestViewModel>(this, MessageKey.BILL_CHANGED, OnBillsChanged);
            MessagingCenter.Subscribe<PayNowViewModel>(this, MessageKey.BILL_CHANGED, OnBillsChanged);

            GetCheckedInBookings(null);

            this.WhenAny(OnSearchKeyChanged, vm => vm.SearchKey);
        }

        public override void OnPopped()
        {
            base.OnPopped();

            MessagingCenter.Unsubscribe<ViewBillsViewModel>(this, MessageKey.BILL_CHANGED);
            MessagingCenter.Unsubscribe<ExtraSelectGuestViewModel>(this, MessageKey.BILL_CHANGED);
            MessagingCenter.Unsubscribe<PayNowViewModel>(this, MessageKey.BILL_CHANGED);
        }

        private void OnBillsChanged(object sender)
        {
            GetCheckedInBookings(null);
        }

        private void GetCheckedInBookings(object sender)
        {
            if (IsBusy || !ConnectivityHelper.IsNetworkAvailable())
            {
                return;
            }

            EmptyMessageVisible = false;
            IsLoading = true;

            Task.Run(async () =>
            {
                return await bookingsService.GetCheckedInBookings();
            }).ContinueWith(task => Device.BeginInvokeOnMainThread(() =>
            {
                IsLoading = false;

                ListCheckedInBooking.Clear();

                if (task.Status == TaskStatus.RanToCompletion)
                {
                    if (task.Result != null)
                    {
                        if (task.Result.Success)
                        {
                            if (task.Result.ListBooking != null && task.Result.ListBooking.Count > 0)
                            {
                                ListCheckedInBooking = new ObservableCollection<Booking>(task.Result.ListBooking.Where(b => b.Depart.HasValue && b.Depart.Value.ToString("yyyy-MM-dd").Equals(DateTime.Today.ToString("yyyy-MM-dd"))).OrderBy(b => b.Name).ToList());
                            }
                        }
                        else if (!string.IsNullOrWhiteSpace(task.Result.Message))
                        {
                            CoreMethods.DisplayAlert("", task.Result.Message, TranslateExtension.GetValue("ok"));
                        }

                        EmptyMessageVisible = ListCheckedInBooking.Count == 0;
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

        private void StartSearchBookings(object sender, TaskCompletionSource<bool> tcs)
        {
            if (IsLoading)
            {
                tcs.SetResult(true);
                return;
            }

            IsSearching = true;
            SearchResultVisible = true;

            tcs.SetResult(true);
        }

        private void StopSearchBookings(object sender, TaskCompletionSource<bool> tcs)
        {
            SearchResultVisible = false;
            IsSearching = false;

            ListSearchResultBooking = new List<Booking>();
            SearchKey = "";

            tcs.SetResult(true);
        }

        private void OnSearchKeyChanged(string sender)
        {
            if (!IsSearching)
                return;

            if (cts != null)
                cts.Cancel();

            cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;

            Task.Run(async () =>
            {
                await Task.Delay(1500, token);

                if (string.IsNullOrWhiteSpace(SearchKey))
                    return new List<Booking>();
                else
                    return ListCheckedInBooking.ToList().FindAll(b => b.Name.ToUpper().Contains(SearchKey.ToUpper()));
            }, token).ContinueWith(task => Device.BeginInvokeOnMainThread(() =>
            {
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    ListSearchResultBooking = task.Result;
                }
            }));
        }

        private async void ViewBills(Booking sender, TaskCompletionSource<bool> tcs)
        {
            if (IsCheckingOut)
                return;

            await CoreMethods.PushViewModel<ViewBillsViewModel>(sender?.DeepCopy());

            tcs.SetResult(true);
        }

        private async void AddExtras(Booking sender, TaskCompletionSource<bool> tcs)
        {
            if (IsCheckingOut)
                return;

            await CoreMethods.PushViewModel<ExtraSelectGuestViewModel>(sender?.DeepCopy());

            tcs.SetResult(true);
        }

        private async void Proceed(Booking sender, TaskCompletionSource<bool> tcs)
        {
            if (sender.State != "Checked In")
            {
                tcs.SetResult(true);
                return;
            }

            if (sender.IsPaid)
            {
                if (await CoreMethods.DisplayAlert("", "Are you sure to Check-Out " + sender.Reference + " ?", "Yes", "No"))
                {
                    CheckOut(sender, tcs);
                    return;
                }
            }
            else
            {
                await CoreMethods.PushViewModel<PayNowViewModel>(sender?.DeepCopy());
            }

            tcs.SetResult(true);
        }

        private async void PayNow(Booking sender, TaskCompletionSource<bool> tcs)
        {
            if (IsCheckingOut)
                return;

            await CoreMethods.PushViewModel<PayNowViewModel>(sender?.DeepCopy());

            tcs.SetResult(true);
        }

        private void CheckOut(Booking sender, TaskCompletionSource<bool> tcs)
        {
            if (IsCheckingOut || sender.IsBusy || !ConnectivityHelper.IsNetworkAvailable())
            {
                return;
            }

            IsCheckingOut = true;
            sender.IsBusy = true;

            Task.Run(async () =>
            {
                return await bookingsService.Checkout(sender.Id);
            }).ContinueWith(task => Device.BeginInvokeOnMainThread(() =>
            {
                IsCheckingOut = false;
                sender.IsBusy = false;

                if (task.Status == TaskStatus.RanToCompletion)
                {
                    if (task.Result != null)
                    {
                        if (task.Result.Success)
                        {
                            CoreMethods.DisplayAlert("", "Checkout Successful", TranslateExtension.GetValue("ok"));

                            ListCheckedInBooking.Remove(sender);
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