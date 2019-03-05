using Acr.UserDialogs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Realms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TravellerApp.Constants;
using TravellerApp.Interfaces;
using TravellerApp.Models;
using TravellerApp.Models.Response;
using TravellerApp.Models.Result;
using TravellerApp.Notifications;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing.Net.Mobile.Forms;

namespace TravellerApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyBookingsPage : TabbedPage
    {
        private User User => Realm.GetInstance().Find<User>(DBLocalID.USER);
        private List<Bookings> listbook;
        private List<ListInfo> listHostOriginal;
        public CurrentlyAtResult CurrentlyAt = default;

        private ViewBillResult billResult;

        public ViewBillResult BillResult
        {
            get => billResult;
            set
            {
                billResult = value;
                OnPropertyChanged();
            }
        }

        public MyBookingsPage()
        {
            InitializeComponent();

            lbName.Text = User.name;
            byte[] imageBytes = Convert.FromBase64String(User.profile_pic);
            profilePicture.Source = ImageSource.FromStream(() => new MemoryStream(imageBytes));

            GetMyBookings();
        }

        private async void GetMyBookings()
        {
            UserDialogs.Instance.Loading(title: "Fetching all your bookings").Show();
            var client = new HttpClient()
            {
                Timeout = TimeSpan.FromSeconds(20)
            };

            var AuthService = DependencyService.Get<IAuthService>();

            JObject @params = new JObject()
            {
                new JProperty("params", new JObject()
                {
                    new JProperty("auth", new JObject()
                    {
                        //new JProperty("login", "herman.wolkie@gmail.com"),
                        //new JProperty("password", "MTIz"),
                        new JProperty("login", AuthService.UserName),
                        new JProperty("password", AuthService.Password),
                        new JProperty("db", ServerAuth.DB)
                    })
                })
            };

            var data = @params.ToString();
            var content = new StringContent(data, Encoding.UTF8, "application/json");
            Debug.WriteLine("REQUEST-GetMyBookings: " + data);

            try
            {
                HttpResponseMessage response = await client.PostAsync(ApiUri.BASE_URL + ApiUri.GET_BOOKINGS, content);
                string responseContent = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    UserDialogs.Instance.Loading().Hide();
                    Debug.WriteLine("RESPONSE-GetMyBookings: " + responseContent);

                    MyBookingsResponse bookingsResponse = JsonConvert.DeserializeObject<MyBookingsResponse>(responseContent, App.DefaultSerializerSettings);
                    ListBookingResult bookingResult = bookingsResponse.result;
                    if (bookingResult.success)
                    {
                        listbook = bookingResult.bookings.ToList();
                        var listBookingReal = bookingResult.bookings.Where(b => !string.IsNullOrWhiteSpace(b.depart_date) && !"false".Equals(b.depart_date.Trim()));

                        var listPast = listBookingReal.Where(b => !string.IsNullOrWhiteSpace(b.arrive_dates.ToString()) && !"false".Equals(b.depart_date.ToString().Trim()) && b.arrive_dates < DateTime.Today).ToList();
                        var listCurrent = listBookingReal.Where(b => States.Sale.Equals(b.state.ToLower().Trim())).ToList();
                        var listUpcoming = listBookingReal.Where(b => !string.IsNullOrWhiteSpace(b.arrive_dates.ToString()) && !"false".Equals(b.depart_date.ToString().Trim()) && b.arrive_dates >= DateTime.Today).ToList();

                        listPastBookings.ItemsSource = listPast;
                        listCurrentBookings.ItemsSource = listCurrent;
                        listUpcomingBookings.ItemsSource = listUpcoming;

                        //listUpcomingBookingsExpire.ItemsSource = listUpcoming;
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

        private async Task InitContentAsync()
        {
            var client = new HttpClient()
            {
                Timeout = TimeSpan.FromSeconds(20)
            };

            var auth = "H6V$36A*!?L^G2NXX7U%=GY@";
            JObject jsonDataObject = new JObject {
                { "auth",  auth}
            };

            JObject jsonData = new JObject {
                { "params", jsonDataObject }
            };

            var data = jsonData.ToString();
            var content = new StringContent(data, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await client.PostAsync(ApiUri.BASE_URL + ApiUri.FETCH_ALL_HOSTS, content);
                string responseContent = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    ListInfoResponse listInfoResponse = JsonConvert.DeserializeObject<ListInfoResponse>(responseContent, App.DefaultSerializerSettings);
                    ListInfoResult listResult = listInfoResponse.result;

                    if (listResult.success)
                    {
                        listHostOriginal = listResult.hosts;
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

        private async void BookAgain(object sender, EventArgs e)
        {
            try
            {
                await InitContentAsync();
                var selectedItem = ((Button)sender).BindingContext as Bookings;
                if (listHostOriginal != null)
                {
                    var host = listHostOriginal.Where(x => x.usr_token.Equals(selectedItem.host_token)).FirstOrDefault();
                    if (host != null)
                    {
                        var page = new BookingPage(host);
                        await Navigation.PushAsync(page, true);
                    }
                }
            }
            catch (Exception exx)
            {
                Debug.WriteLine(exx.Message);
            }
        }

        private async void buttonConfirm_UpComing_Clicked(object sender, EventArgs e)
        {
            try
            {
                string msg;
                Button btn = (Button)sender;
                var buttonName = btn.Text;
                var selectedItem = ((Button)sender).BindingContext as Bookings;

                if (!string.IsNullOrWhiteSpace(selectedItem.peach_payment_link_url)
                        && !"false".Equals(selectedItem.peach_payment_link_url))
                {
                    Device.OpenUri(new Uri(selectedItem.peach_payment_link_url));
                }
                else
                {
                    msg = "Payment Link Not Available";
                    await DisplayAlert("Confirm", msg, "OK");
                }

                //if (States.Confirm_Booking.Equals(buttonName.Trim()))
                //{
                //    if (!string.IsNullOrWhiteSpace(selectedItem.peach_payment_link_url)
                //        && !"false".Equals(selectedItem.peach_payment_link_url))
                //    {
                //        Device.OpenUri(new Uri(selectedItem.peach_payment_link_url));
                //    }
                //    else
                //    {
                //        msg = "Payment Link Not Available";
                //        await DisplayAlert("Confirm", msg, "OK");
                //    }
                //}
                //else if (States.Manage_Booking.Equals(buttonName.Trim()))
                //{
                //    msg = "Coming Soon!";
                //    await DisplayAlert("Manage booking", msg, "OK");
                //}
            }
            catch (Exception exx)
            {
                Debug.WriteLine(exx.Message);
            }
        }

        void OnDeleteImageTapped(object sender, EventArgs args)
        {
            button_img_popup.IsVisible = false;
            profile_img_popup.IsVisible = false;
            OpenCurrentAt.IsVisible = false;
            delete_button_popup.IsVisible = false;
        }

        private async void Open_Booking_Clicked(object sender, EventArgs e)
        {
            try
            {
                button_img_popup.IsVisible = true;
                profile_img_popup.IsVisible = true;
                OpenCurrentAt.IsVisible = true;
                delete_button_popup.IsVisible = true;

                if (((Button)sender).BindingContext is Bookings selectedItem)
                {
                    //byte[] imageBytes = Convert.FromBase64String(selectedItem.image_url);
                    image_url_popup.Source = selectedItem.image_url;
                }
            }
            catch (Exception exx)
            {
                Debug.WriteLine(exx.Message);
            }
        }

        private async void buttonConfirm_Current_Clicked(object sender, EventArgs e)
        {
            try
            {
                Button btn = (Button)sender;
                var buttonName = btn.Text;
                if (States.Open_Booking.Equals(buttonName.Trim()))
                {
                    var page = new HomePage();
                    await Navigation.PushAsync(page, true);
                }
            }
            catch (Exception exx)
            {
                Debug.WriteLine(exx.Message);
            }
        }

        private async void WriteReview_Click(object sender, EventArgs e)
        {
            try
            {
                //open review page in #32
                var msg = "Coming Soon!";
                await DisplayAlert("Write a review", msg, "OK");
            }
            catch (Exception exx)
            {
                Debug.WriteLine(exx.Message);
            }
        }

        private void OnQrCode_Tapped(object sender, EventArgs e)
        {
            var view = sender as ZXingBarcodeImageView;
            try
            {
                var popup = new AbsoluteLayout();
                popup.GestureRecognizers.Add(new TapGestureRecognizer()
                {
                    Command = new Command((s) => DependencyService.Get<IPopupService>()?.HideContent())
                });
                popup.Children.Add(new BoxView()
                {
                    BackgroundColor = Color.Black,
                    Opacity = .8
                }, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);
                popup.Children.Add(new Image()
                {
                    Source = ImageSource.FromStream(() => DependencyService.Get<IBarcodeService>()?.GenerateQR((view.BindingContext as Bookings)?.booking_token))
                }, new Rectangle(.5, .5, 250, 250), AbsoluteLayoutFlags.PositionProportional);

                DependencyService.Get<IPopupService>()?.ShowContent(popup);
            }
            catch { }
        }

        private async void REVIEW_CLICK(object sender, EventArgs e)
        {
            var page = new RecentPostsPage();
            await Navigation.PushAsync(page, true);
        }

        private async void REVIEW_POPUP_CLICK(object sender, EventArgs e)
        {
            var page = new RecentPostsPage();
            await Navigation.PushAsync(page, true);
        }

        private void VIEW_BILL_CLICK(object sender, EventArgs e)
        {
            GetCurrently();

            if (CurrentlyAt != null)
            {
                GetBillInfo();
            }
        }

        private void VIEW_BILL_POPUP_CLICK(object sender, EventArgs e)
        {
            GetCurrently();

            if (CurrentlyAt != null)
            {
                GetBillInfo();
            }
        }

        private async void GetBillInfo()
        {
            try
            {
                UserDialogs.Instance.Loading(title: "Getting Bill Info...").Show();

                var client = new HttpClient()
                {
                    Timeout = TimeSpan.FromSeconds(20)
                };

                var AuthService = DependencyService.Get<IAuthService>();

                JObject jsonAuth = new JObject
                {
                    { "db", ServerAuth.DB },
                    { "login", AuthService.UserName },
                    { "password", AuthService.Password },
                    { "visit_token", CurrentlyAt.visit_token }
                };

                JObject jsonDataObject = new JObject
                {
                    { "auth", jsonAuth }
                };

                JObject jsonData = new JObject
                {
                    { "params", jsonDataObject }
                };

                var data = jsonData.ToString();
                var content = new StringContent(data, Encoding.UTF8, "application/json");

                Debug.WriteLine("REQUEST-ViewBill: " + data);

                HttpResponseMessage response = await client.PostAsync(ApiUri.BASE_URL + ApiUri.VIEW_BILL, content);

                string responseContent = await response.Content.ReadAsStringAsync();

                Debug.WriteLine("RESPONSE-ViewBill: " + responseContent);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    ViewBillResponse viewBillResponse = JsonConvert.DeserializeObject<ViewBillResponse>(responseContent, App.DefaultSerializerSettings);

                    if (viewBillResponse != null && viewBillResponse.Result != null)
                    {
                        if (viewBillResponse.Result.Success)
                        {
                            viewBillResponse.Result.GroupedBillLines = new List<GroupedBillLine>();
                            var billLines = viewBillResponse.Result.BillLines.GroupBy(b => b.CategId).OrderBy(b => b.Key);
                            foreach (var item in billLines.ToList())
                            {
                                viewBillResponse.Result.GroupedBillLines.Add(new GroupedBillLine
                                {
                                    CategId = item.Key,
                                    BillLines = item.ToList(),
                                    Total = item.Sum(b => b.Total)
                                });
                            }

                            BillResult = viewBillResponse.Result;

                            GroupedBillLinesView.ItemsSource = viewBillResponse.Result.GroupedBillLines.ToList();
                            BillResultTotal.Text = "R" + BillResult.Total.ToString("N2");
                            BillResultPaid.Text = "R" + BillResult.Paid.ToString("N2");
                            BillResultBalance.Text = "R" + BillResult.Balance.ToString("N2");

                            YourBillResultTotal.Text = "R" + BillResult.Total.ToString("N2");
                            YourBillResultPaid.Text = "R" + BillResult.Paid.ToString("N2");
                            YourBillResultBalance.Text = "R" + BillResult.Balance.ToString("N2");

                            viewBillPopup.IsVisible = true;
                        }
                        else if (!string.IsNullOrWhiteSpace(viewBillResponse.Result.Message))
                        {
                            await DisplayAlert("", viewBillResponse.Result.Message, "Ok");
                        }
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
            }
            catch (TaskCanceledException e)
            {
                UserDialogs.Instance.Toast(new ToastConfig("Bad Connection Error. Try Again"));
                Debug.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                Internal.ServerError();
                Debug.WriteLine(e.Message);
            }
            finally
            {
                UserDialogs.Instance.Loading().Hide();
            }
        }

        private async void GetCurrently()
        {
            using (UserDialogs.Instance.Loading(title: "Getting Currently..."))
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
                            CurrentlyAt = res.result;
                        }
                        else if (!string.IsNullOrWhiteSpace(res.result.message))
                        {
                            UserDialogs.Instance.Toast(new ToastConfig(res.result.message));
                            CurrentlyAt = default;
                        }
                        else
                        {
                            CurrentlyAt = default;
                        }
                    }
                    else
                    {
                        CurrentlyAt = default;
                    }
                });
            }
        }

        private void ViewBillClose_Tapped(object sender, EventArgs e)
        {
            viewBillPopup.IsVisible = false;
        }

        private void ViewBillPayNow_Clicked(object sender, EventArgs e)
        {
            yourBillPopup.IsVisible = true;
        }

        private void YourBillClose_Tapped(object sender, EventArgs e)
        {
            yourBillPopup.IsVisible = false;
        }

        private void YourBillPay_Clicked(object sender, EventArgs e)
        {
            SendPaymentLink();
        }

        private async void SendPaymentLink()
        {
            try
            {
                UserDialogs.Instance.Loading(title: "Send Payment Link...").Show();

                var client = new HttpClient()
                {
                    Timeout = TimeSpan.FromSeconds(20)
                };

                JObject @params = new JObject
                {
                    new JProperty("params", new JObject
                    {
                        new JProperty("auth", new JObject
                        {
                            new JProperty("host_token", CurrentlyAt.Token),
                            new JProperty("booking_id", billResult.BookingId)
                        }),
                        new JProperty("payment", new JObject
                        {
                            new JProperty("amount", billResult.Balance),
                            new JProperty("guest_id", billResult.GuestId)
                        })
                    })
                };

                var data = @params.ToString();
                var content = new StringContent(data, Encoding.UTF8, "application/json");

                Debug.WriteLine("REQUEST-SendPaymentLink-URI: " + billResult.HostUrl + ApiUri.SEND_PAYMENT_LINK);
                Debug.WriteLine("REQUEST-SendPaymentLink-Data: " + data);

                HttpResponseMessage response = await client.PostAsync(billResult.HostUrl + ApiUri.SEND_PAYMENT_LINK, content);

                string responseContent = await response.Content.ReadAsStringAsync();

                Debug.WriteLine("RESPONSE-SendPaymentLink: " + responseContent);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    PaymentResponse paymentResponse = JsonConvert.DeserializeObject<PaymentResponse>(responseContent, App.DefaultSerializerSettings);

                    if (paymentResponse != null && paymentResponse.Result != null)
                    {
                        if (paymentResponse.Result.Success)
                        {
                            //Open Url for payment
                            Device.OpenUri(new Uri(paymentResponse.Result.PaymentLink));

                            yourBillPopup.IsVisible = false;
                            viewBillPopup.IsVisible = false;
                        }
                        else if (!string.IsNullOrWhiteSpace(paymentResponse.Result.Message))
                        {
                            UserDialogs.Instance.Toast(new ToastConfig(paymentResponse.Result.Message));
                        }
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
            }
            catch (TaskCanceledException ex)
            {
                Internal.ServerError();
                Debug.WriteLine(ex.Message);
            }
            catch (Exception exx)
            {
                Internal.ServerError();
                Debug.WriteLine(exx.Message);
            }
            finally
            {
                UserDialogs.Instance.Loading().Hide();
            }
        }
    }
}