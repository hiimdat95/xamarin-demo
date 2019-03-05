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
    public class BookingCalendarViewModel : TinyViewModel
    {
        private readonly IBookingsService bookingsService;

        private CancellationTokenSource cts;

        private Calendar.Plugin.Shared.Calendar calendar;

        private DateTime? lastSelectedDay;
        private List<DateTime> selectedDates = new List<DateTime>();

        public DateTime MinDay { get; set; } = DateTime.Today;

        private ObservableCollection<Room> availableRooms = new ObservableCollection<Room>();
        public ObservableCollection<Room> AvailableRooms { get => availableRooms; set => SetProperty(ref availableRooms, value); }

        public ICommand DateSelectedCommand { get; private set; }
        public ICommand RoomSelectedCommand { get; private set; }

        public BookingCalendarViewModel(IBookingsService bookingsService)
        {
            this.bookingsService = bookingsService;

            DateSelectedCommand = new AwaitCommand<DateTime>(DateSelected);
            RoomSelectedCommand = new AwaitCommand<Room>(RoomSelected);
        }

        public override void OnPopped()
        {
            base.OnPopped();

            calendar?.SelectedDates.Clear();
        }

        private void GetAvailableRooms()
        {
            if (!ConnectivityHelper.IsNetworkAvailable())
            {
                return;
            }

            if (cts != null)
                cts.Cancel();

            cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;

            IsBusy = true;
            Task.Run(async () =>
            {
               
                return await bookingsService.GetAvailableRooms(selectedDates.First(), selectedDates.Last());
            }, token).ContinueWith(task => Device.BeginInvokeOnMainThread(() =>
            {
                IsBusy = false;

                if (!token.IsCancellationRequested && task.Status == TaskStatus.RanToCompletion)
                {
                    if (task.Result.Rooms != null)
                    {
                        AvailableRooms = new ObservableCollection<Room>(task.Result.Rooms);
                    }
                    else
                    {
                        CoreMethods.DisplayAlert("", TranslateExtension.GetValue("dialog_message_no_rooms_available"), TranslateExtension.GetValue("ok"));
                        CoreMethods.PopViewModel();
                    }
                }
                else if (task.IsFaulted)
                {
                    CoreMethods.DisplayAlert(TranslateExtension.GetValue("error"), task.Exception?.GetBaseException().Message, TranslateExtension.GetValue("ok"));
                }
            }));
        }

        private void DateSelected(DateTime sender, TaskCompletionSource<bool> tcs)
        {
            if (lastSelectedDay == null)
            {
                if (cts != null)
                    cts.Cancel();

                AvailableRooms.Clear();

                lastSelectedDay = sender;
                selectedDates.Clear();
                selectedDates.Add(sender);
            }
            else
            {
                selectedDates.Clear();

                if (sender >= lastSelectedDay)
                {
                    for (DateTime day = lastSelectedDay.Value; day <= sender; day = day.AddDays(1))
                    {
                        selectedDates.Add(day);
                    }
                }
                else
                {
                    for (DateTime day = sender; day <= lastSelectedDay.Value; day = day.AddDays(1))
                    {
                        selectedDates.Add(day);
                    }
                }

                GetAvailableRooms();

                lastSelectedDay = null;
            }

            calendar = calendar ?? CurrentPage.FindByName<Calendar.Plugin.Shared.Calendar>("Calendar");
            if (calendar != null)
            {
                calendar.SelectedDates.Clear();
                foreach (var day in selectedDates)
                {
                    calendar.SelectedDates.Add(day);
                }
                calendar.ForceRedraw();
            }

            tcs.SetResult(true);
        }

        private async void RoomSelected(Room sender, TaskCompletionSource<bool> tcs)
        {
            if (cts != null)
                cts.Cancel();

            var booking = new AddBooking()
            {
                CheckInDate = selectedDates.First(),
                CheckOutDate = selectedDates.Last(),
                ListRoom = new ObservableCollection<Room>() { sender }
            };
            await CoreMethods.PushViewModel<BookingRoomViewModel>(booking);
            tcs.SetResult(true);
        }
    }
}