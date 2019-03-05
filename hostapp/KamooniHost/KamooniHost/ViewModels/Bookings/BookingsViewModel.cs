using KamooniHost.Constants;
using KamooniHost.Helpers;
using KamooniHost.IServices;
using KamooniHost.Models;
using KamooniHost.Models.Control;
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
    public class BookingsViewModel : TinyViewModel
    {
        private readonly IBookingsService openBookingsService;

        private CancellationTokenSource cts;

        public TabButton ToArriveTabButton { get; set; } = new TabButton();

        public TabButton CheckedInTabButton { get; set; } = new TabButton();

        public TabButton CheckedOutTabButton { get; set; } = new TabButton();

        private bool tabToArriveVisible;
        public bool TabToArriveVisible { get => tabToArriveVisible; set => SetProperty(ref tabToArriveVisible, value); }

        private bool tabCheckedInVisible;
        public bool TabCheckedInVisible { get => tabCheckedInVisible; set => SetProperty(ref tabCheckedInVisible, value); }

        private bool tabCheckedOutVisible;
        public bool TabCheckedOutVisible { get => tabCheckedOutVisible; set => SetProperty(ref tabCheckedOutVisible, value); }

        private ObservableCollection<Booking> listToArriveBooking = new ObservableCollection<Booking>();
        public ObservableCollection<Booking> ListToArriveBooking { get => listToArriveBooking; set => SetProperty(ref listToArriveBooking, value); }

        private ObservableCollection<Booking> listCheckedInBooking = new ObservableCollection<Booking>();
        public ObservableCollection<Booking> ListCheckedInBooking { get => listCheckedInBooking; set => SetProperty(ref listCheckedInBooking, value); }

        private ObservableCollection<Booking> listCheckedOutBooking = new ObservableCollection<Booking>();
        public ObservableCollection<Booking> ListCheckedOutBooking { get => listCheckedOutBooking; set => SetProperty(ref listCheckedOutBooking, value); }

        private List<Booking> listSearchResultBooking = new List<Booking>();
        public List<Booking> ListSearchResultBooking { get => listSearchResultBooking; set => SetProperty(ref listSearchResultBooking, value); }

        private bool isLoading1;
        public bool IsLoading1 { get => isLoading1; set => SetProperty(ref isLoading1, value); }

        private bool isLoading2;
        public bool IsLoading2 { get => isLoading2; set => SetProperty(ref isLoading2, value); }

        private bool isLoading3;
        public bool IsLoading3 { get => isLoading3; set => SetProperty(ref isLoading3, value); }

        private string searchKey;
        public string SearchKey { get => searchKey; set => SetProperty(ref searchKey, value); }

        private bool isSearching;
        public bool IsSearching { get => isSearching; set => SetProperty(ref isSearching, value); }

        private bool searchResultVisible;
        public bool SearchResultVisible { get => searchResultVisible; set => SetProperty(ref searchResultVisible, value); }

        private bool isCheckingOut;
        public bool IsCheckingOut { get => isCheckingOut; set => SetProperty(ref isCheckingOut, value); }

        public ICommand StartSearchBookingsCommand { get; private set; }
        public ICommand StopSearchBookingsCommand { get; private set; }
        public ICommand TabToArriveSelectedCommand { get; private set; }
        public ICommand TabCheckedInSelectedCommand { get; private set; }
        public ICommand TabCheckedOutSelectedCommand { get; private set; }
        public ICommand GetToArriveBookingsCommand { get; private set; }
        public ICommand GetCheckedInBookingsCommand { get; private set; }
        public ICommand GetCheckedOutBookingsCommand { get; private set; }
        public ICommand ViewBillsCommand { get; private set; }
        public ICommand AddExtrasCommand { get; private set; }
        public ICommand ProceedCommand { get; private set; }
        public ICommand PayNowCommand { get; private set; }
        public ICommand CheckOutCommand { get; private set; }

        public BookingsViewModel(IBookingsService openBookingsService)
        {
            this.openBookingsService = openBookingsService;

            StartSearchBookingsCommand = new AwaitCommand(StartSearchBookings);
            StopSearchBookingsCommand = new AwaitCommand(StopSearchBookings);
            TabToArriveSelectedCommand = new AwaitCommand(TabToArriveSelected);
            TabCheckedInSelectedCommand = new AwaitCommand(TabCheckedInSelected);
            TabCheckedOutSelectedCommand = new AwaitCommand(TabCheckedOutSelected);
            GetToArriveBookingsCommand = new Command(GetToArriveBookings);
            GetCheckedInBookingsCommand = new Command(GetCheckedInBookings);
            GetCheckedOutBookingsCommand = new Command(GetCheckedOutBookings);
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

            TabCheckedInSelected(null, null);

            GetToArriveBookings(null);
            GetCheckedInBookings(null);
            GetCheckedOutBookings(null);

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
            GetToArriveBookings(null);
            GetCheckedInBookings(null);
            GetCheckedOutBookings(null);
        }

        private void TabToArriveSelected(object sender, TaskCompletionSource<bool> tcs)
        {
            ActiveTabButton(ToArriveTabButton);
            DeactiveTabButton(CheckedInTabButton);
            DeactiveTabButton(CheckedOutTabButton);

            TabToArriveVisible = true;
            TabCheckedInVisible = false;
            TabCheckedOutVisible = false;

            tcs?.SetResult(true);
        }

        private void TabCheckedInSelected(object sender, TaskCompletionSource<bool> tcs)
        {
            DeactiveTabButton(ToArriveTabButton);
            ActiveTabButton(CheckedInTabButton);
            DeactiveTabButton(CheckedOutTabButton);

            TabToArriveVisible = false;
            TabCheckedInVisible = true;
            TabCheckedOutVisible = false;

            tcs?.SetResult(true);
        }

        private void TabCheckedOutSelected(object sender, TaskCompletionSource<bool> tcs)
        {
            DeactiveTabButton(ToArriveTabButton);
            DeactiveTabButton(CheckedInTabButton);
            ActiveTabButton(CheckedOutTabButton);

            TabToArriveVisible = false;
            TabCheckedInVisible = false;
            TabCheckedOutVisible = true;

            tcs?.SetResult(true);
        }

        private void ActiveTabButton(TabButton tabButton)
        {
            tabButton.BorderColor = Color.DarkGreen;
            tabButton.BackgroundColor = Color.Green;
            tabButton.TextColor = Color.White;
        }

        private void DeactiveTabButton(TabButton tabButton)
        {
            tabButton.BorderColor = Color.White;
            tabButton.BackgroundColor = Color.White;
            tabButton.TextColor = Color.Green;
        }

        private void GetToArriveBookings(object sender)
        {
            if (IsBusy || !ConnectivityHelper.IsNetworkAvailable())
            {
                return;
            }

            IsLoading1 = true;

            Task.Run(async () =>
            {
                return await openBookingsService.GetToArriveBookings();
            }).ContinueWith(task => Device.BeginInvokeOnMainThread(() =>
            {
                IsLoading1 = false;

                ListToArriveBooking.Clear();

                if (task.Status == TaskStatus.RanToCompletion)
                {
                    if (task.Result != null)
                    {
                        if (task.Result.Success)
                        {
                            if (task.Result.ListBooking != null && task.Result.ListBooking.Count > 0)
                            {
                                ListToArriveBooking = new ObservableCollection<Booking>(task.Result.ListBooking.OrderByDescending(b => b.Reference).ToList());
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

        private void GetCheckedInBookings(object sender)
        {
            if (IsBusy || !ConnectivityHelper.IsNetworkAvailable())
            {
                return;
            }

            IsLoading2 = true;

            Task.Run(async () =>
            {
                return await openBookingsService.GetCheckedInBookings();
            }).ContinueWith(task => Device.BeginInvokeOnMainThread(() =>
            {
                IsLoading2 = false;

                ListCheckedInBooking.Clear();

                if (task.Status == TaskStatus.RanToCompletion)
                {
                    if (task.Result != null)
                    {
                        if (task.Result.Success)
                        {
                            if (task.Result.ListBooking != null && task.Result.ListBooking.Count > 0)
                            {
                                ListCheckedInBooking = new ObservableCollection<Booking>(task.Result.ListBooking.OrderByDescending(b => b.Reference).ToList());
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

        private void GetCheckedOutBookings(object sender)
        {
            if (IsBusy || !ConnectivityHelper.IsNetworkAvailable())
            {
                return;
            }

            IsLoading3 = true;

            Task.Run(async () =>
            {
                return await openBookingsService.GetCheckedOutBookings();
            }).ContinueWith(task => Device.BeginInvokeOnMainThread(() =>
            {
                IsLoading3 = false;

                ListCheckedOutBooking.Clear();

                if (task.Status == TaskStatus.RanToCompletion)
                {
                    if (task.Result != null)
                    {
                        if (task.Result.Success)
                        {
                            if (task.Result.ListBooking != null && task.Result.ListBooking.Count > 0)
                            {
                                ListCheckedOutBooking = new ObservableCollection<Booking>(task.Result.ListBooking.OrderByDescending(b => b.Reference).ToList());
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

        private void StartSearchBookings(object sender, TaskCompletionSource<bool> tcs)
        {
            if (IsBusy || IsLoading1 || IsLoading2 || IsLoading3)
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
                    return (ListToArriveBooking.Union(ListCheckedInBooking.Union(ListCheckedOutBooking))).ToList().FindAll(b => b.Name.ToUpper().Contains(SearchKey.ToUpper()));
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

            tcs.TrySetResult(true);
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
                return await openBookingsService.Checkout(sender.Id);
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

                            ListToArriveBooking.Remove(sender);
                            ListCheckedInBooking.Remove(sender);
                            ListCheckedOutBooking.Remove(sender);
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