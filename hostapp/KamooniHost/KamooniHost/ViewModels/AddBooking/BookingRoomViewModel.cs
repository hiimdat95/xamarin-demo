using KamooniHost.Helpers;
using KamooniHost.IServices;
using KamooniHost.Models;
using KamooniHost.Models.Control;
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
    public class BookingRoomViewModel : TinyViewModel
    {
        private readonly IBookingsService bookingsService;

        private AddBooking booking;
        public AddBooking Booking { get => booking; set => SetProperty(ref booking, value); }

        private List<Room> listRoom;

        private ObservableCollection<Room> listRoomDisplay = new ObservableCollection<Room>();
        public ObservableCollection<Room> ListRoomDisplay { get => listRoomDisplay; set => SetProperty(ref listRoomDisplay, value); }

        private bool roomSelectVisible;
        public bool RoomSelectVisible { get => roomSelectVisible; set => SetProperty(ref roomSelectVisible, value); }

        private bool isBookingValid;
        public bool IsBookingValid { get => isBookingValid; set => SetProperty(ref isBookingValid, value); }

        public ICommand GoToHomeCommand { get; private set; }
        public ICommand GuestQuantitySelectedCommand { get; private set; }
        public ICommand AddRoomCommand { get; private set; }
        public ICommand CloseRoomSelectCommand { get; private set; }
        public ICommand RoomSelectedCommand { get; private set; }
        public ICommand DeleteRoomCommand { get; private set; }
        public ICommand ProceedCommand { get; private set; }

        public BookingRoomViewModel(IBookingsService bookingsService)
        {
            this.bookingsService = bookingsService;

            GoToHomeCommand = new AwaitCommand(GoToHome);
            GuestQuantitySelectedCommand = new AwaitCommand<GuestSelectButton>(GuestQuantitySelected);
            AddRoomCommand = new AwaitCommand(AddRoom);
            CloseRoomSelectCommand = new AwaitCommand(CloseRoomSelect);
            RoomSelectedCommand = new AwaitCommand<Room>(RoomSelected);
            DeleteRoomCommand = new AwaitCommand<Room>(DeleteRoom);
            ProceedCommand = new AwaitCommand(Proceed);
        }

        public override void Init(object data)
        {
            base.Init(data);

            Booking = ((AddBooking)data)?.DeepCopy() ?? new AddBooking();

            ValidateBooking();
        }

        public override void OnPageCreated()
        {
            base.OnPageCreated();

            GetAvailableRooms();
        }

        private void ValidateBooking()
        {
            for (int i = 0; i < Booking.ListRoom.Count; i++)
            {
                AddGuestSelectButtons(Booking.ListRoom[i]);
            }
        }

        private void AddGuestSelectButtons(Room room)
        {
            int maxGuest;
            if (room.Type == "Dormitory" || room.Type == "Camping")
            {
                maxGuest = room.Available;
            }
            else
            {
                maxGuest = room.Capacity;
            }
            for (int j = 1; j <= maxGuest; j++)
            {
                room.ListGuestSelectButton.Add(new GuestSelectButton()
                {
                    Parent = room,
                    Quantity = j
                });
            }
        }

        private void CheckBookingValid()
        {
            IsBookingValid = !Booking.ListRoom.Any(r => r.TotalGuest <= 0);
        }

        private void GetAvailableRooms()
        {
            if (IsBusy || !ConnectivityHelper.IsNetworkAvailable())
            {
                return;
            }

            IsBusy = true;
            Task.Run(async () =>
            {
                return await bookingsService.GetAvailableRooms(Booking.CheckInDate, Booking.CheckOutDate);
            }).ContinueWith(task => Device.BeginInvokeOnMainThread(() =>
            {
                IsBusy = false;
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    if (task.Result.Rooms != null)
                    {
                        listRoom = task.Result.Rooms;
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

        private void GoToHome(object sender, TaskCompletionSource<bool> tcs)
        {
            Application.Current.MainPage = new MainPage();
            tcs.SetResult(true);
        }

        private void GuestQuantitySelected(GuestSelectButton sender, TaskCompletionSource<bool> tcs)
        {
            if (!sender.IsSelected)
            {
                if (sender.Parent is Room room)
                {
                    foreach (var btn in room.ListGuestSelectButton)
                    {
                        btn.Deselect();
                    }
                    sender.Select();

                    room.TotalGuest = sender.Quantity;

                    CheckBookingValid();
                }
            }
            tcs.SetResult(true);
        }

        private void AddRoom(object sender, TaskCompletionSource<bool> tcs)
        {
            RefreshListRoomDisplay();
            RoomSelectVisible = true;
            tcs.SetResult(true);
        }

        private void RefreshListRoomDisplay()
        {
            ListRoomDisplay.Clear();

            foreach (var room in listRoom.FindAll(r => !((r.Type == "Dormitory" || r.Type == "Camping") && Booking.ListRoom.Any(er => er.Type == r.Type))))
            {
                int available = room.Available - Booking.ListRoom.ToList().FindAll(r => r.RoomType == room.RoomType)?.Count ?? 0;
                if (available > 0)
                {
                    ListRoomDisplay.Add(new Room()
                    {
                        RoomType = room.RoomType,
                        Capacity = room.Capacity,
                        Price = room.Price,
                        Available = available,
                        TotalGuest = room.TotalGuest
                    });
                }
            }
        }

        private void CloseRoomSelect(object sender, TaskCompletionSource<bool> tcs)
        {
            RoomSelectVisible = false;
            tcs.SetResult(true);
        }

        private void RoomSelected(Room sender, TaskCompletionSource<bool> tcs)
        {
            sender.CanRemove = true;

            // Add Guest Select Button
            AddGuestSelectButtons(sender);

            Booking.ListRoom.Add(sender);
            RoomSelectVisible = false;
            CheckBookingValid();
            tcs.SetResult(true);
        }

        private void DeleteRoom(Room sender, TaskCompletionSource<bool> tcs)
        {
            Booking.ListRoom.Remove(sender);
            CheckBookingValid();
            tcs.SetResult(true);
        }

        private async void Proceed(object sender, TaskCompletionSource<bool> tcs)
        {
            await CoreMethods.PushViewModel<BookingCustomerViewModel>(Booking);
            tcs.SetResult(true);
        }
    }
}