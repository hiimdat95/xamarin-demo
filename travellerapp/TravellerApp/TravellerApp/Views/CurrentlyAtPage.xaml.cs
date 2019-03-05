using Acr.UserDialogs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Realms;
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
using TravellerApp.Models.DTO;
using TravellerApp.Models.Response;
using TravellerApp.Models.Result;
using TravellerApp.Notifications;
using Xamarin.Forms;

namespace TravellerApp.Views
{
    public partial class CurrentlyAtPage : ContentPage
    {
        private User User => Realm.GetInstance().Find<User>(DBLocalID.USER);
        private bool HasAppeared;
        CurrentlyAtResult currentlyAt;
        public CurrentlyAtResult CurrentlyAt
        {
            get { return currentlyAt; }
            set
            {
                currentlyAt = value;
                if (currentlyAt != null)
                {
                    Title = currentlyAt.Host;
                    stateNameDisplay.Text = currentlyAt.stateDisplay;
                }
            }
        }

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

        public CurrentlyAtPage()
        {
            InitializeComponent();

            NavigationPage.SetBackButtonTitle(this, "");
        }

        public CurrentlyAtPage(CurrentlyAtResult currentlyAt)
        {
            InitializeComponent();

            NavigationPage.SetBackButtonTitle(this, "");

            this.CurrentlyAt = currentlyAt;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            if (HasAppeared)
                return;
            HasAppeared = true;
            if (CurrentlyAt == null)
            {
                await GetCurrently();
              //  await ThingsToDo();
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            HasAppeared = false;
        }

        private async Task ThingsToDo()
        {
            if (CurrentlyAt == null) return;
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
                HttpResponseMessage response = await client.PostAsync(CurrentlyAt.URL + ApiUri.GET_THINGS_TO_DO, content);
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

        private async void Read_More_Clicked(object sender, EventArgs e)
        {
            if (((Button)sender).BindingContext is Things selectedItem)
            {
                var page = new ThingsToDoPages(selectedItem);
                await Navigation.PushAsync(page, true);
            }
            //await Navigation.PushModalAsync(
            //    new ThingsToDoPages(new Things
            //    {
            //        name = selectedItem.name,
            //        image = selectedItem.image,
            //        id = selectedItem.id,
            //        list_price = selectedItem.list_price,
            //        sales_count = selectedItem.sales_count,
            //        summary = selectedItem.summary
            //    }));
        }

        private void CheckOut_Clicked(object sender, EventArgs e)
        {
            checkOutPopup.IsVisible = true;
        }

        private void CheckOut_Close_Tapped(object sender, EventArgs e)
        {
            checkOutPopup.IsVisible = false;
        }

        private void CheckOut_CheckOut_Clicked(object sender, EventArgs e)
        {
            CheckOut(1);
        }

        private void CheckOut_CheckOutAndReview_Clicked(object sender, EventArgs e)
        {
            CheckOut(2);
        }

        private async void CheckOut(int type = 1)
        {
            if (CurrentlyAt == null) return;
            try
            {
                UserDialogs.Instance.Loading(title: "Checking Out...").Show();

                var client = new HttpClient()
                {
                    Timeout = TimeSpan.FromSeconds(20)
                };

                var AuthService = DependencyService.Get<IAuthService>();

                JObject jsonAuth = new JObject
                {
                    { "token", CurrentlyAt.Token }
                };

                JArray jsonTokens = new JArray
                {
                    new JObject
                    {
                        { "token", User.traveller_token },
                        { "visit_token", CurrentlyAt.visit_token }
                    }
                };

                JObject jsonDataObject = new JObject
                {
                    { "auth", jsonAuth },
                    { "tokens", jsonTokens }
                };

                JObject jsonData = new JObject
                {
                    { "params", jsonDataObject }
                };

                var data = jsonData.ToString();
                var content = new StringContent(data, Encoding.UTF8, "application/json");

                Debug.WriteLine("REQUEST-CheckOutTraveller: " + data);

                HttpResponseMessage response = await client.PostAsync(ApiUri.BASE_URL + ApiUri.CHECK_OUT_TRAVELLER, content);

                string responseContent = await response.Content.ReadAsStringAsync();

                Debug.WriteLine("RESPONSE-CheckOutTraveller: " + responseContent);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    ViewBillResponse viewBillResponse = JsonConvert.DeserializeObject<ViewBillResponse>(responseContent, App.DefaultSerializerSettings);

                    if (viewBillResponse != null && viewBillResponse.Result != null)
                    {
                        if (viewBillResponse.Result.Success)
                        {
                            if (type.Equals(1))
                            {
                                //await Navigation.PopAsync();
                                Application.Current.MainPage = new MainNavigation();
                            }
                            else
                            {
                                var mainPage = new MainNavigation();
                                Application.Current.MainPage = mainPage;

                                await mainPage.Detail.Navigation.PushAsync(new WriteReviewPage(new Review { Host = CurrentlyAt.Host, Token = CurrentlyAt.Token, VisitToken = CurrentlyAt.visit_token }));
                            }
                        }
                        else if (!string.IsNullOrWhiteSpace(viewBillResponse.Result.Message))
                        {
                            UserDialogs.Instance.Toast(new ToastConfig(viewBillResponse.Result.Message));
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

        private async void WriteReview_Clicked(object sender, EventArgs e)
        {
            if (CurrentlyAt == null) return;
            await Navigation.PushAsync(new WriteReviewPage(new Review { Host = CurrentlyAt.Host, Token = CurrentlyAt.Token, VisitToken = CurrentlyAt.visit_token }), true);
            //await Navigation.PushModalAsync(new WriteReviewPage(new Review { Host = CurrentlyAt.Host, Token = CurrentlyAt.Token, VisitToken = CurrentlyAt.visit_token }));
        }

        private void ViewBill_Clicked(object sender, EventArgs e)
        {
            GetBillInfo();
        }

        private void ViewBillClose_Tapped(object sender, EventArgs e)
        {
            viewBillPopup.IsVisible = false;
        }

        private async void GetBillInfo()
        {
            if (CurrentlyAt == null) return;
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
            if (CurrentlyAt == null) return;
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
                        CurrentlyAt = res.result;
                        if (CurrentlyAt != null)
                        {

                            //If not checked in have popup message "You aren't currently checked in anywhere"
                            if (!string.IsNullOrEmpty(CurrentlyAt.state) && !States.Sale.Equals(CurrentlyAt.state.ToLower()))
                            {
                                UserDialogs.Instance.Alert(res.result.message, null, "Ok");
                            }
                            else
                            {
                                GetBillInfo();
                            }
                        }
                        else
                        {
                            UserDialogs.Instance.Alert(res.result.message, null, "Ok");
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(res.result.message))
                    {
                        UserDialogs.Instance.Alert(res.result.message, null, "Ok");
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
}