using Acr.UserDialogs;
using Newtonsoft.Json.Linq;
using Realms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using TravellerApp.Constants;
using TravellerApp.Helpers;
using TravellerApp.Interfaces;
using TravellerApp.Models;
using TravellerApp.Models.Response;
using TravellerApp.Models.Result;
using TravellerApp.Notifications;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TravellerApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage
    {
        private User User => Realm.GetInstance().Find<User>(DBLocalID.USER);

        private CurrentlyAtResult currentlyAt = default;

        private string bookingToken;
        private List<AddToBookingGuest> listGuest;

        private int selectedRoom;

        public HomePage()
        {
            InitializeComponent();

            InitContent();

            listPlatforms.Refreshing += (s, e) => GetCurrently();

            listPlatforms.BeginRefresh();

            MessagingCenter.Unsubscribe<CurrentlyAtPage>(this, "CheckedOut");
            MessagingCenter.Subscribe<CurrentlyAtPage>(this, "CheckedOut", (s) =>
            {
                GetCurrently();
            });
        }

        private void InitContent()
        {
            name.Text = User.name;
            passportId.Text = User.passport_id;
            email.Text = User.email;
            mobile.Text = User.mobile;

            Country country = Country.GetCountryByISO(User.country);
            flagImage.Source = country?.FlagPath;

            byte[] imageBytes = Convert.FromBase64String(User.profile_pic);
            profilePicture.Source = ImageSource.FromStream(() => new MemoryStream(imageBytes));

            qrContainer.Source = ImageSource.FromStream(() =>
            {
                return DependencyService.Get<IBarcodeService>()?.GenerateQR(User.traveller_token);
            });
        }

        private async void GetCurrently()
        {
            var AuthService = DependencyService.Get<IAuthService>();

            JObject jsonAuth = new JObject {
                { "login",  AuthService.UserName},
                { "password",  AuthService.Password},
                { "db",  ServerAuth.DB}
            };

            JObject jsonDataObject = new JObject {
                { "auth",  jsonAuth}
            };

            JObject jsonData = new JObject {
                { "params", jsonDataObject }
            };

            await WebService.Instance.PostAsync<CurrentlyAtResponse>(ApiUri.BASE_URL + ApiUri.CURRENTLY_AT, content: jsonData, onSuccess: async (res) =>
            {
                if (res.result != null)
                {
                    if (res.result.success)
                    {
                        currentlyAt = res.result;

                        currentlyStatus.Text = currentlyAt.Host;

                        var page = new CurrentlyAtPage(currentlyAt);
                        await Navigation.PushAsync(page, true);
                    }
                    else if (!string.IsNullOrWhiteSpace(res.result.message))
                    {
                        UserDialogs.Instance.Toast(new ToastConfig(res.result.message));
                        currentlyAt = default;
                    }
                    else
                    {
                        currentlyAt = default;
                    }
                }
                else
                {
                    currentlyAt = default;
                }
            });

            listPlatforms.EndRefresh();
        }

        private async void ScanQR_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (RoomSelector.IsVisible)
                    return;

                bookingToken = "";
                bookingToken = await BarcodeHelper.Instance.ScanQRCode();
                //bookingToken = "CDK1ECF2KN0BTH2W70L-1JVIPX43YGPXKCWU9BI";
                if (!string.IsNullOrWhiteSpace(bookingToken))
                {
                    AddToBookingRequest();
                }
            }
            catch (Exception exx)
            {
                Debug.WriteLine(exx.Message);
            }
        }

        private async void AddToBookingRequest()
        {
            var AuthService = DependencyService.Get<IAuthService>();

            JObject jsonAuth = new JObject
            {
                { "db", ServerAuth.DB },
                { "login", AuthService.UserName },
                { "password", AuthService.Password }
                //{ "login", "baoptlse@gmail.com" },
                //{ "password", "MTIz" }
            };

            JObject jsonDataObject = new JObject
            {
                { "auth", jsonAuth },
                { "booking_token", bookingToken }
            };

            JObject jsonData = new JObject
            {
                { "params", jsonDataObject }
            };

            await WebService.Instance.PostAsync<AddToBookingResponse>(ApiUri.BASE_URL + ApiUri.ADD_TO_BOOKING_REQUEST, "Getting Booking Info...", jsonData, (res) =>
            {
                if (res.Result != null)
                {
                    if (res.Result.Success)
                    {
                        if (res.Result.ManualAdd)
                        {
                            ListRoom.ItemsSource = null;
                            ListRoom.ItemsSource = listGuest = res.Result.Guests;

                            RoomSelector.IsVisible = true;
                        }
                        else
                        {
                            UserDialogs.Instance.Toast(new ToastConfig(res.Result.Message));
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(res.Result.Message))
                    {
                        UserDialogs.Instance.Toast(new ToastConfig(res.Result.Message));
                    }
                    else
                    {
                        Internal.ServerError();
                    }
                }
                else
                {
                    Internal.ServerError();
                }
            });
        }

        private void ListRoom_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var item = (AddToBookingGuest)e.Item;

            if (!string.IsNullOrWhiteSpace(item.Email))
                return;

            selectedRoom = item.Id;

            foreach (var guest in listGuest)
            {
                if (string.IsNullOrWhiteSpace(guest.Email))
                {
                    guest.Customer = "";
                }
            }

            listGuest.Find(g => g.Id == selectedRoom).Customer = User.name;

            ListRoom.ItemsSource = null;
            ListRoom.ItemsSource = listGuest;
        }

        private async void SubmitRoom_Clicked(object sender, EventArgs e)
        {
            var AuthService = DependencyService.Get<IAuthService>();

            JObject jsonAuth = new JObject
            {
                { "db", ServerAuth.DB },
                { "login", AuthService.UserName },
                { "password", AuthService.Password }
                //{ "login", "baoptlse@gmail.com" },
                //{ "password", "MTIz" }
            };

            JObject jsonDataObject = new JObject
            {
                { "auth", jsonAuth },
                { "booking_token", bookingToken },
                { "room_line_id", selectedRoom }
            };

            JObject jsonData = new JObject
            {
                { "params", jsonDataObject }
            };

            await WebService.Instance.PostAsync<SuccessResponse>(ApiUri.BASE_URL + ApiUri.ADD_TO_BOOKING_MANUALLY, "Submit Booking...", jsonData, (res) =>
            {
                if (res.Result != null)
                {
                    if (res.Result.Success)
                    {
                        selectedRoom = 0;
                        RoomSelector.IsVisible = false;
                        UserDialogs.Instance.Toast(new ToastConfig(res.Result.Message));
                    }
                    else if (!string.IsNullOrWhiteSpace(res.Result.Message))
                    {
                        UserDialogs.Instance.Toast(new ToastConfig(res.Result.Message));
                    }
                    else
                    {
                        Internal.ServerError();
                    }
                }
                else
                {
                    Internal.ServerError();
                }
            });
        }

        private void CancelSelectRoom_Clicked(object sender, EventArgs e)
        {
            selectedRoom = 0;
            RoomSelector.IsVisible = false;
        }

        private async void Currently_Tapped(object sender, EventArgs e)
        {
            if (currentlyAt == null)
                return;

            var page = new CurrentlyAtPage(currentlyAt);
            await Navigation.PushAsync(page, true);
        }
    }
}