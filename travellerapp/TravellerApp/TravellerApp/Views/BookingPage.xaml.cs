using Acr.UserDialogs;
using FFImageLoading.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plugin.Messaging;
using Realms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TravellerApp.Constants;
using TravellerApp.Models;
using TravellerApp.Models.Response;
using TravellerApp.Models.Result;
using Xamarin.Forms;
using Xamarin.Forms.Extensions;
using Xamarin.Forms.Xaml;

namespace TravellerApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BookingPage : ContentPage
    {
        private ListInfo currentHost;
        private User user;
        private List<RoomList> listCamping;
        private List<RoomList> listPrivateRoom;
        private List<RoomList> listSharedRoom;
        private List<RoomList> listSelectedRoom = new List<RoomList>();
        private List<Reviews> listReviewLayer = new List<Reviews>();
        private decimal totalPay = 0;
        DateTime newDateStart = DateTime.Today;
        DateTime newDateEnd = DateTime.Today.AddDays(1);

        public BookingPage()
        {
            //Use for debug real time
            //LiveReload.Init();

            InitializeComponent();
        }

        public BookingPage(ListInfo host)
        {
            //Use for debug real time
            //LiveReload.Init();

            InitializeComponent();

            //Get User
            var realm = Realm.GetInstance();
            user = realm.Find<User>(DBLocalID.USER);

            // Object is reference, use DeepCopy to make sure that object in previous page won't change
            currentHost = host.DeepCopy();

            GetHostDetail();
            GetRooms();

            startDatePicker.MinimumDate = DateTime.Today;
            endDatePicker.MinimumDate = DateTime.Today.AddDays(1);
        }
        
        private void MainDatePicker_DateEndSelected(object sender, DateChangedEventArgs e)
        {
            newDateEnd = e.NewDate;
        }

        private void MainDatePicker_DateStartSelected(object sender, DateChangedEventArgs e)
        {
            newDateStart = e.NewDate;
        }

        private async void ShowAvailableRooms(object sender, EventArgs e)
        {
            try
            {
                var startDate = newDateStart.ToString("yyyy-MM-dd");
                var endDate = newDateEnd.ToString("yyyy-MM-dd");

                if (!string.IsNullOrWhiteSpace(startDate) && !string.IsNullOrWhiteSpace(endDate))
                {
                    if (newDateStart >= newDateEnd)
                    {
                        var msg = "Start Date must be bigger End Date. Please try again !";
                        await DisplayAlert("Warning", msg, "OK");
                    }
                    else
                    {
                        GetRooms(startDate, endDate);
                    }
                }
                else
                {
                    UserDialogs.Instance.Toast(new ToastConfig("Bad Connection Error. Try Again"));
                }
            }
            catch (Exception exx)
            {
                Debug.WriteLine(exx.Message);
            }
        }

        private async void GetHostDetail()
        {
            UserDialogs.Instance.Loading(title: "Getting Rooms...").Show();
            var client = new HttpClient()
            {
                Timeout = TimeSpan.FromSeconds(20)
            };

            JObject @params = new JObject()
            {
                new JProperty("params", new JObject()
                {
                    new JProperty("auth", new JObject()
                    {
                        new JProperty("token", currentHost.usr_token)
                    })
                })
            };

            var data = @params.ToString();
            var content = new StringContent(data, Encoding.UTF8, "application/json");
            Debug.WriteLine("REQUEST-GetHostDetails: " + data);

            try
            {
                HttpResponseMessage response = await client.PostAsync(ApiUri.BASE_URL + ApiUri.GET_HOST_DETAILS, content);
                string responseContent = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    UserDialogs.Instance.Loading().Hide();
                    Debug.WriteLine("RESPONSE-GetHostDetails: " + responseContent);

                    HostDetailsResponse hostDetailsResponse = JsonConvert.DeserializeObject<HostDetailsResponse>(responseContent, App.DefaultSerializerSettings);
                    HostDetailsResult hostDetailsResult = hostDetailsResponse.result;

                    //Image header
                    ImageSource image = !(hostDetailsResult.host.image is string base64) ? null : ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(base64)));
                    imageHeader.Source = image;

                    //Name header
                    nameHeader.Text = currentHost.name;
                    nameHeaderLast.Text = "Posts about " + currentHost.name;

                    //provinceHeader
                    provinceHeader.Text = hostDetailsResult.host.summary;

                    //Rating
                    var rating = hostDetailsResult.host.rating;
                    host_rating.Text = Math.Round(rating, 1).ToString();

                    var listReview = hostDetailsResult.reviews.ToList();  
                    var listReviewActive = new List<Reviews>();

                    foreach (var item in listReview)
                    {
                        if (!string.IsNullOrWhiteSpace(item.dateDisplay) && !string.IsNullOrWhiteSpace(item.image_medium)
                            && !string.IsNullOrWhiteSpace(item.text) && !string.IsNullOrWhiteSpace(item.traveller_partner_id)
                            && (!"0".Equals(item.rating) || !string.IsNullOrWhiteSpace(item.rating)))
                        {
                            listReviewActive.Add(item);
                        }
                    }

                    if (listReviewActive != null)
                    {
                        //foreach (var item in listReviewActive.ToList())
                        //{
                        //    if (item.text.Length > 10)
                        //    {
                        //        ReviewLayer.IsVisible = true;
                        //        ReviewLayer_full.IsVisible = false;
                        //    }
                        //    else
                        //    {
                        //        ReviewLayer.IsVisible = false;
                        //        ReviewLayer_full.IsVisible = true;
                        //    }
                        //}

                        ReviewLayer.ItemsSource = listReviewActive.Where(r => !string.IsNullOrWhiteSpace(r.text) && r.text.Length >= 180).OrderByDescending(r => r.date).ToList();
                        ReviewLayer_full.ItemsSource = listReviewActive.Where(r => string.IsNullOrWhiteSpace(r.text) || r.text.Length < 180).OrderByDescending(r => r.date).ToList();

                        listReviewLayer = listReviewActive.OrderByDescending(r => r.date).ToList();
                        //ReviewLayer.ItemsSource = listReviewActive.OrderByDescending(r => r.date).ToList();
                        //ReviewLayer_full.ItemsSource = listReviewActive.OrderByDescending(r => r.date).ToList();
                    }                                           
                    else
                    {
                        ReviewLayer.ItemsSource = null;
                        ReviewLayer_full.ItemsSource = null;
                    }
                }
            }
            catch (TaskCanceledException ex)
            {
                UserDialogs.Instance.Loading().Hide();
                UserDialogs.Instance.Toast(new ToastConfig("Bad Connection Error. Try Again"));
                Debug.WriteLine(ex.Message);
            }
            catch (Exception exx)
            {
                Debug.WriteLine(exx.Message);
                UserDialogs.Instance.Loading().Hide();
                Notifications.Internal.ServerError();
            }
        }

        private async void GetRooms(string startDate = null, string endDate = null)
        {
            UserDialogs.Instance.Loading(title: "Getting Rooms...").Show();
            var client = new HttpClient()
            {
                Timeout = TimeSpan.FromSeconds(20)
            };

            JObject @params = new JObject()
            {
                new JProperty("params", new JObject()
                {
                    new JProperty("auth", new JObject()
                    {
                        new JProperty("token", currentHost.usr_token)
                    }),
                    new JProperty("dates", new JObject()
                    {
                        new JProperty("start_date", !string.IsNullOrWhiteSpace(startDate) ? startDate : DateTime.Today.ToString("yyyy-MM-dd")),
                        new JProperty("end_date", !string.IsNullOrWhiteSpace(endDate) ? endDate : DateTime.Today.AddDays(1).ToString("yyyy-MM-dd"))
                    })
                })
            };

            var data = @params.ToString();
            var content = new StringContent(data, Encoding.UTF8, "application/json");
            Debug.WriteLine("REQUEST-AvailableRooms: " + data);

            try
            {
                HttpResponseMessage response = await client.PostAsync(currentHost.host_url + ApiUri.AVAILABLE_ROOMS, content);
                string responseContent = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    UserDialogs.Instance.Loading().Hide();
                    Debug.WriteLine("RESPONSE-AvailableRooms: " + responseContent);

                    ListRoomResponse listRoomResponse = JsonConvert.DeserializeObject<ListRoomResponse>(responseContent, App.DefaultSerializerSettings);

                    listCamping = listRoomResponse.result.Where(s => s.camping.Equals(true)).OrderBy(s => s.room_type).ToList();
                    listPrivateRoom = listRoomResponse.result.Where(s => s.Private.Equals(true) && !s.camping.Equals(true)).OrderBy(s => s.room_type).ToList();
                    listSharedRoom = listRoomResponse.result.Where(s => s.Private.Equals(false) && !s.camping.Equals(true)).OrderBy(s => s.room_type).ToList();

                    if (listCamping.Count != 0 || listPrivateRoom.Count != 0 || listSharedRoom.Count != 0)
                    {
                        //private rooms
                        if (listPrivateRoom.Count != 0)
                        {
                            privateRoomsLayout.IsVisible = true;
                            listPrivateRooms.ItemsSource = listPrivateRoom;
                        }
                        else
                        {
                            privateRoomsLayout.IsVisible = false;
                            listSharedRooms.Padding = 0;
                        }

                        //shared rooms
                        if (listSharedRoom.Count != 0)
                        {
                            sharedRoomsLayout.IsVisible = true;
                            listSharedRooms.ItemsSource = listSharedRoom;
                        }
                        else
                        {
                            sharedRoomsLayout.IsVisible = false;
                            listCampingRooms.Padding = 0;
                        }

                        //camping
                        if (listCamping.Count != 0)
                        {
                            campingRoomsLayout.IsVisible = true;
                            listCampingRooms.ItemsSource = listCamping;
                        }
                        else
                        {
                            campingRoomsLayout.IsVisible = false;
                        }

                        notAvailableRoom.IsVisible = false;
                    }
                    else
                    {
                        notAvailableRoom.IsVisible = true;
                        privateRoomsLayout.IsVisible = false;
                        sharedRoomsLayout.IsVisible = false;
                        campingRoomsLayout.IsVisible = false;
                    }
                }
            }
            catch (TaskCanceledException ex)
            {
                UserDialogs.Instance.Loading().Hide();
                UserDialogs.Instance.Toast(new ToastConfig("Bad Connection Error. Try Again"));
                Debug.WriteLine(ex.Message);
            }
            catch (Exception exx)
            {
                Debug.WriteLine(exx.Message);
                UserDialogs.Instance.Loading().Hide();
                Notifications.Internal.ServerError();
            }
        }

        private void OnCallPhoneClick(object sender, EventArgs e)
        {
            var PhoneCallTask = CrossMessaging.Current.PhoneDialer;
            if (PhoneCallTask.CanMakePhoneCall)
                PhoneCallTask.MakePhoneCall(currentHost.mobile);
        }

        private void OnPinMapClick(object sender, EventArgs e)
        {
            //var name = currentHost.name.Replace("&", "and");
            var name = Uri.EscapeUriString(currentHost.name);
            var loc = string.Format("{0},{1}", currentHost.partner_latitude, currentHost.partner_longitude);
            var addr = Uri.EscapeUriString(currentHost.name);
            if (Device.RuntimePlatform == Device.iOS)
            {
                var request = string.Format("http://maps.apple.com/maps?q={0}&sll={1}", name.Replace(' ', '+'), loc);
                //var request = string.Format("http://maps.apple.com/?daddr=" + currentHost.partner_latitude + "," + currentHost.partner_longitude + "");
                Device.OpenUri(new Uri(request));
            }

            if (Device.RuntimePlatform == Device.Android)
            {
                var request = string.Format("geo:0,0?q={0}({1})", string.IsNullOrWhiteSpace(addr) ? loc : addr, name);
                //var request = string.Format("http://maps.google.com/?daddr=" + currentHost.partner_latitude + "," + currentHost.partner_longitude + "");
                Device.OpenUri(new Uri(request));
            }
        }

        private async void createBookingRooms(object sender, EventArgs e)
        {
            string msgCreatBooking;
            UserDialogs.Instance.Loading(title: "Creating Booking...").Show();
            var client = new HttpClient()
            {
                Timeout = TimeSpan.FromSeconds(20)
            };

            JArray roomData = new JArray();
            foreach (var item in listSelectedRoom)
            {
                roomData.Add(new JObject()
                {
                   new JProperty("room_type", item.room_type),
                   new JProperty("start_date", newDateStart.ToString("yyyy-MM-dd")),
                   new JProperty("end_date", newDateEnd.ToString("yyyy-MM-dd")),
                   new JProperty("id", item.id),
                   new JProperty("guest_total", item.selected_room)
                });
            }

            JObject @params = new JObject()
            {
                new JProperty("params", new JObject()
                {
                    new JProperty("auth", new JObject()
                    {
                        new JProperty("token",  currentHost.usr_token),
                        new JProperty("url",  currentHost.host_url)
                    }),
                    new JProperty("source", new JObject()
                    {
                        new JProperty("source", "Kamooni Traveller App"),
                        new JProperty("channel", "Kamooni Traveller App")
                    }),
                    new JProperty("customer", new JObject()
                    {
                        new JProperty("name", user.name),
                        new JProperty("email", user.email),
                        new JProperty("mobile", "false".Equals(user.mobile) ? "" : user.mobile),
                        new JProperty("token", user.traveller_token)
                    }),
                    new JProperty("roomdata", roomData)
                })
            };

            var data = @params.ToString();
            var content = new StringContent(data, Encoding.UTF8, "application/json");
            Debug.WriteLine("REQUEST-CreateBooking: " + data);

            try
            {
                HttpResponseMessage response = await client.PostAsync(ApiUri.BASE_URL + ApiUri.CREATE_BOOKING, content);
                string responseContent = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    UserDialogs.Instance.Loading().Hide();
                    Debug.WriteLine("RESPONSE-AvailableRooms: " + responseContent);

                    CreateBookingResponse createBookingResponse = JsonConvert.DeserializeObject<CreateBookingResponse>(responseContent, App.DefaultSerializerSettings);
                    CreateBookingResult createBookingResult = createBookingResponse.result;
                    if (createBookingResult.success)
                    {
                        msgCreatBooking = "Your reservation is almost confirmed! Please click pay to confirm your booking. A payment link is also email to you.";
                        await DisplayAlert("Booking Created Successfully", msgCreatBooking, "Pay Now");

                        //Init new data
                        //frCheckOut.IsVisible = false;
                        //InitializeComponent();
                        //GetHostDetail();
                        //GetRooms();    

                        //Open PayNow
                        Device.OpenUri(new Uri(createBookingResult.peach_payment_link_url));

                        //backtohome
                        var page = new HomePage();
                        await Navigation.PushAsync(page, true);
                    }
                    else
                    {
                        msgCreatBooking = createBookingResult.message;
                        await DisplayAlert("Booking Created Unsuccessfully", msgCreatBooking, "OK");
                    }
                }
            }
            catch (TaskCanceledException ex)
            {
                UserDialogs.Instance.Loading().Hide();
                UserDialogs.Instance.Toast(new ToastConfig("Bad Connection Error. Try Again"));
                Debug.WriteLine(ex.Message);
            }
            catch (Exception exx)
            {
                Debug.WriteLine(exx.Message);
                UserDialogs.Instance.Loading().Hide();
                Notifications.Internal.ServerError();
            }
        }

        private void Choose_Room(object sender, EventArgs e)
        {
            listBookedRooms.ItemsSource = new List<RoomList>();
            listSelectedRoom.Clear();
            if (listPrivateRooms.ItemsSource is List<RoomList> listRoom1)
            {
                foreach (var room in listRoom1)
                {
                    if (room.selected_room > 0)
                    {
                        listSelectedRoom.Add(room);
                    }
                }
            }
            if (listSharedRooms.ItemsSource is List<RoomList> listRoom2)
            {
                foreach (var room in listRoom2)
                {
                    if (room.selected_room > 0)
                    {
                        listSelectedRoom.Add(room);
                    }
                }
            }

            if (listCampingRooms.ItemsSource is List<RoomList> listRoom3)
            {
                foreach (var room in listRoom3)
                {
                    if (room.selected_room > 0)
                    {
                        listSelectedRoom.Add(room);
                    }
                }
            }

            //calculator
            foreach (var item in listSelectedRoom)
            {
                item.totalPrice = item.selected_room * decimal.Parse(item.price, CultureInfo.InvariantCulture);
                totalPay += item.selected_room * decimal.Parse(item.price, CultureInfo.InvariantCulture);
            }

            listBookedRooms.ItemsSource = listSelectedRoom;
            total_pay.Text = totalPay.ToString();

            frCheckOut.IsVisible = (!string.IsNullOrEmpty(total_pay.Text) && !"0".Equals(total_pay.Text)) ? true : false;
        }

        private void OnCancelItemClick(object sender, EventArgs e)
        {
            var selectedItem = ((CachedImage)sender).BindingContext as RoomList;
            if (listBookedRooms.ItemsSource is List<RoomList> listRoom)
            {
                if (listRoom.Count > 0)
                {
                    foreach (var room in listRoom.ToList())
                    {
                        if (selectedItem.room_type.Equals(room.room_type))
                        {
                            if (listPrivateRooms.ItemsSource is List<RoomList> listRoomPrivate)
                            {
                                if (listRoomPrivate.Count > 0)
                                {
                                    foreach (var rooms in listRoomPrivate.ToList())
                                    {
                                        if (rooms.room_type.Equals(selectedItem.room_type))
                                        {
                                            rooms.selected_room = 0;
                                            break;
                                        }
                                    }

                                    listPrivateRooms.ItemsSource = null;
                                    listPrivateRooms.ItemsSource = listRoomPrivate;
                                }            
                            }
                            if (listSharedRooms.ItemsSource is List<RoomList> listRoom2)
                            {
                                foreach (var rooms in listRoom2.ToList())
                                {
                                    if (rooms.room_type.Equals(selectedItem.room_type))
                                    {
                                        rooms.selected_room = 0;
                                        break;
                                    }
                                }

                                listSharedRooms.ItemsSource = null;
                                listSharedRooms.ItemsSource = listRoom2;
                            }
                            if (listCampingRooms.ItemsSource is List<RoomList> listRoom3)
                            {
                                foreach (var rooms in listRoom3.ToList())
                                {
                                    if (rooms.room_type.Equals(selectedItem.room_type))
                                    {
                                        rooms.selected_room = 0;
                                        break;
                                    }
                                }

                                listCampingRooms.ItemsSource = null;
                                listCampingRooms.ItemsSource = listRoom3;
                            }
                        }    
                    }

                    listSelectedRoom = listSelectedRoom.Where(x => !x.room_type.Equals(selectedItem.room_type)).ToList();
                }

                //calculator again
                totalPay = 0;
                foreach (var item in listSelectedRoom)
                {
                    item.totalPrice = item.selected_room * decimal.Parse(item.price, CultureInfo.InvariantCulture);
                    totalPay += item.selected_room * decimal.Parse(item.price, CultureInfo.InvariantCulture);
                }

                listBookedRooms.ItemsSource = listSelectedRoom;
                total_pay.Text = totalPay.ToString();

                frCheckOut.IsVisible = (!string.IsNullOrEmpty(total_pay.Text) && !"0".Equals(total_pay.Text)) ? true : false;
            }
        }

        private void OnRatingChanged(object sender, float e)
        {
            //customRatingBar.Rating = e;
        }

        private void ReadMoreClick(object sender, EventArgs e)
        {
            ReviewLayer.IsVisible = false;
            ReviewLayer_full.IsVisible = true;
            ReviewLayer.ItemsSource = listReviewLayer;
            ReviewLayer_full.ItemsSource = listReviewLayer;
        }
    }
}