using Acr.UserDialogs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TravellerApp.Views.PopupPage
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RecommendedPlacesPopup : Rg.Plugins.Popup.Pages.PopupPage
    {
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

        public RecommendedPlacesPopup()
        {
            InitializeComponent();
            getHeightDevice();
        }

        public void getHeightDevice()
        {
            // Get Metrics
            var metrics = DeviceDisplay.MainDisplayInfo;

            // Height (in pixels)
            var height = metrics.Height;
            var width = metrics.Width;

            //viewBillPopup.WidthRequest = width;
        }

        public RecommendedPlacesPopup(CurrentlyAtResult currentlyAtResult)
        {
            InitializeComponent();
            this.currentlyAt = currentlyAtResult;
        }

        private CurrentlyAtResult currentlyAt;
        bool hasAppeared;
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (hasAppeared)
                return;
            hasAppeared = true;

            // await GetCurrently();
            await ThingsToDo(currentlyAt);
        }

        private async Task GetCurrently()
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
                        await ThingsToDo(res.result);
                    }
                    else if (!string.IsNullOrWhiteSpace(res.result.message))
                    {
                        UserDialogs.Instance.Toast(new ToastConfig(res.result.message));
                    }
                }
            });
        }

        private async void GetCurrentlyToViewBill()
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

        private async Task ThingsToDo(CurrentlyAtResult currentlyAt)
        {
            if (!currentlyAt.success)
                return;
            UserDialogs.Instance.Loading(title: "You are checked in! let's see what's up").Show();

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
            Debug.WriteLine("REQUEST-UserExistCheck: " + data);

            try
            {
                HttpResponseMessage response = await client.PostAsync(currentlyAt.URL + ApiUri.GET_THINGS_TO_DO, content);
                string responseContent = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Debug.WriteLine("RESPONSE-GetInformation: " + responseContent);

                    GetThingsToDoResponse listThingsToDoResponse = JsonConvert.DeserializeObject<GetThingsToDoResponse>(responseContent);
                    GetThingsToDoResult listResult = listThingsToDoResponse.result;

                    if (listResult.success)
                    {
                        listThings.ItemsSource = listResult.things.ToList();
                    }

                    UserDialogs.Instance.Loading().Hide();
                }
                else
                {
                    UserDialogs.Instance.Loading().Hide();
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
                Internal.ServerError();
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

        private async void GetBillInfo(CurrentlyAtResult currentlyAt)
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
                    { "visit_token", currentlyAt.visit_token }
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

        private async void ImageButton_Clicked(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            GetCurrentlyToViewBill();

            if (CurrentlyAt != null)
            {
                GetBillInfo();
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
                            new JProperty("host_token", currentlyAt.Token),
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

        private async void ThingsToDoTapped(object sender, EventArgs e)
        {
            if (((Button)sender).BindingContext is Things selectedItem)
            {
                var page = new ThingsToDoPages(selectedItem);
                await Navigation.PushAsync(page, true);
            }
        }

        private async void REVIEW_POPUP_CLICK(object sender, EventArgs e)
        {
            var page = new RecentPostsPage();
            await Navigation.PushAsync(page, true);
        }
    }
}