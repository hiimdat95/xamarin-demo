using KamooniHost.Helpers;
using KamooniHost.IServices;
using KamooniHost.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Extensions;
using Xamarin.Forms.TinyMVVM;

namespace KamooniHost.ViewModels
{
    public class CheckInViewModel : TinyViewModel
    {
        private readonly AppSettings AppSettings = Helpers.Settings.AppSettings;

        private readonly ICheckInService checkInService;

        private List<ToCheckInBooking> toDayBookingsOriginal = new List<ToCheckInBooking>();
        public ObservableCollection<ToCheckInBooking> ToDayBookings { get; set; } = new ObservableCollection<ToCheckInBooking>();

        private List<ToCheckInBooking> noShowsBookingsOriginal = new List<ToCheckInBooking>();
        public ObservableCollection<ToCheckInBooking> NoShowsBookings { get; set; } = new ObservableCollection<ToCheckInBooking>();

        private List<ToCheckInBooking> earlyBookingsOriginal = new List<ToCheckInBooking>();
        public ObservableCollection<ToCheckInBooking> EarlyBookings { get; set; } = new ObservableCollection<ToCheckInBooking>();

        private bool noBooking1;
        public bool NoBooking1 { get => noBooking1; set => SetProperty(ref noBooking1, value); }

        private bool noBooking2;
        public bool NoBooking2 { get => noBooking2; set => SetProperty(ref noBooking2, value); }

        private bool noBooking3;
        public bool NoBooking3 { get => noBooking3; set => SetProperty(ref noBooking3, value); }

        private bool isRefreshing1;
        public bool IsRefreshing1 { get => isRefreshing1; set => SetProperty(ref isRefreshing1, value); }

        private bool isRefreshing2;
        public bool IsRefreshing2 { get => isRefreshing2; set => SetProperty(ref isRefreshing2, value); }

        private bool isRefreshing3;
        public bool IsRefreshing3 { get => isRefreshing3; set => SetProperty(ref isRefreshing3, value); }

        public ICommand GetToDayBookingsCommand { get; private set; }
        public ICommand SearchToDayBookingsCommand { get; private set; }
        public ICommand GetNoShowsBookingsCommand { get; private set; }
        public ICommand SearchNoShowsBookingsCommand { get; private set; }
        public ICommand GetEarlyBookingsCommand { get; private set; }
        public ICommand SearchEarlyBookingsCommand { get; private set; }
        public ICommand GuestCheckInCommand { get; private set; }
        public ICommand AddBookingCommand { get; private set; }

        public CheckInViewModel(ICheckInService checkInService)
        {
            this.checkInService = checkInService;

            GetToDayBookingsCommand = new Command(GetToDayBookings);
            SearchToDayBookingsCommand = new AwaitCommand<string>(SearchToDayBookings);
            GetNoShowsBookingsCommand = new Command(GetNoShowsBookings);
            SearchNoShowsBookingsCommand = new AwaitCommand<string>(SearchNoShowsBookings);
            GetEarlyBookingsCommand = new Command(GetEarlyBookings);
            SearchEarlyBookingsCommand = new AwaitCommand<string>(SearchEarlyBookings);
            GuestCheckInCommand = new AwaitCommand<ToCheckInBooking>(GuestCheckIn);
            AddBookingCommand = new AwaitCommand(AddBooking);
        }

        public override void OnPageCreated()
        {
            base.OnPageCreated();

            GetToDayBookings(null);
            GetNoShowsBookings(null);
            GetEarlyBookings(null);
        }

        private void GetToDayBookings(object sender)
        {
            if (!ConnectivityHelper.IsNetworkAvailable())
            {
                return;
            }

            IsRefreshing1 = true;
            Task.Run(async () =>
            {
                return await checkInService.GetCheckInBookings(DateTime.Today, DateTime.Today);
            }).ContinueWith(task => Device.BeginInvokeOnMainThread(() =>
            {
                IsRefreshing1 = false;
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    ToDayBookings.Clear();

                    if (task.Result != null)
                    {
                        if (task.Result.Bookings?.Count > 0)
                        {
                            NoBooking1 = false;
                            toDayBookingsOriginal = task.Result.Bookings;
                            ToDayBookings.AddRange(task.Result.Bookings);
                        }
                        else
                        {
                            NoBooking1 = true;
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

        private void SearchToDayBookings(string searchText, TaskCompletionSource<bool> tcs)
        {
            ToDayBookings.Clear();
            if (string.IsNullOrWhiteSpace(searchText))
            {
                ToDayBookings.AddRange(toDayBookingsOriginal);
            }
            else
            {
                ToDayBookings.AddRange(toDayBookingsOriginal.FindAll(b => b.Customer.Name.UnSignContains(searchText) || b.Customer.Mobile.Contains(searchText) || b.Customer.PassportId.Contains(searchText)));
            }
            tcs.SetResult(true);
        }

        private void GetNoShowsBookings(object sender)
        {
            if (!ConnectivityHelper.IsNetworkAvailable())
            {
                return;
            }

            IsRefreshing2 = true;
            Task.Run(async () =>
            {
                return await checkInService.GetCheckInBookings(DateTime.Today.Subtract(TimeSpan.FromDays(14)), DateTime.Today.Subtract(TimeSpan.FromDays(1)));
            }).ContinueWith(task => Device.BeginInvokeOnMainThread(() =>
            {
                IsRefreshing2 = false;
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    NoShowsBookings.Clear();

                    if (task.Result != null)
                    {
                        if (task.Result.Bookings?.Count > 0)
                        {
                            NoBooking2 = false;
                            noShowsBookingsOriginal = task.Result.Bookings;
                            NoShowsBookings.AddRange(task.Result.Bookings);
                        }
                        else
                        {
                            NoBooking2 = true;
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

        private void SearchNoShowsBookings(string searchText, TaskCompletionSource<bool> tcs)
        {
            NoShowsBookings.Clear();
            if (string.IsNullOrWhiteSpace(searchText))
            {
                NoShowsBookings.AddRange(noShowsBookingsOriginal);
            }
            else
            {
                NoShowsBookings.AddRange(noShowsBookingsOriginal.FindAll(b => b.Customer.Name.UnSignContains(searchText) || b.Customer.Mobile.Contains(searchText) || b.Customer.PassportId.Contains(searchText)));
            }
            tcs.SetResult(true);
        }

        private void GetEarlyBookings(object sender)
        {
            if (!ConnectivityHelper.IsNetworkAvailable())
            {
                return;
            }

            IsRefreshing3 = true;
            Task.Run(async () =>
            {
                return await checkInService.GetCheckInBookings(DateTime.Today.AddDays(1), DateTime.Today.AddDays(14));
            }).ContinueWith(task => Device.BeginInvokeOnMainThread(() =>
            {
                IsRefreshing3 = false;
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    EarlyBookings.Clear();

                    if (task.Result != null)
                    {
                        if (task.Result.Bookings?.Count > 0)
                        {
                            NoBooking3 = false;
                            earlyBookingsOriginal = task.Result.Bookings;
                            EarlyBookings.AddRange(task.Result.Bookings);
                        }
                        else
                        {
                            NoBooking3 = true;
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

        private void SearchEarlyBookings(string searchText, TaskCompletionSource<bool> tcs)
        {
            EarlyBookings.Clear();
            if (string.IsNullOrWhiteSpace(searchText))
            {
                EarlyBookings.AddRange(earlyBookingsOriginal);
            }
            else
            {
                EarlyBookings.AddRange(earlyBookingsOriginal.FindAll(b => b.Customer.Name.UnSignContains(searchText) || b.Customer.Mobile.Contains(searchText) || b.Customer.PassportId.Contains(searchText)));
            }
            tcs.SetResult(true);
        }

        private async void GuestCheckIn(ToCheckInBooking e, TaskCompletionSource<bool> tcs)
        {
            await CoreMethods.PushViewModel<GuestDetailsViewModel>(e.DeepCopy());
            tcs.SetResult(true);
        }

        private async void AddBooking(object sender, TaskCompletionSource<bool> tcs)
        {
            await CoreMethods.PushViewModel<BookingCalendarViewModel>();
            tcs.SetResult(true);
        }
    }
}