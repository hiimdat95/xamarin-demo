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
using TravellerApp.Interfaces.ComectChatCallback;
using TravellerApp.Models.DTO;
using TravellerApp.Models.Response;
using TravellerApp.Models.Result;
using TravellerApp.Notifications;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TravellerApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Home : ContentPage
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

        public Home()
        {
            //LiveReload.Init();
            InitializeComponent();
            getHeightDevice();
        }

        public void getHeightDevice()
        {
            // Get Metrics
            var metrics = DeviceDisplay.MainDisplayInfo;

            // Height (in pixels)
            var height = metrics.Height;

            HeightOfDevice.HeightRequest = height;

            image_FIEND_EXPERIENCES_CLICK.HeightRequest = 0.2 * height;
            image_RECENT_POSTS_CLICK.HeightRequest = 0.4 * height;
            image_MESSAGES_CLICK.HeightRequest = 0.2 * height;
            image_VISITS_CLICK.HeightRequest = 0.2 * height;
            image_VIEW_BILL_CLICK.HeightRequest = 0.2 * height;
            image_CHECK_IN_CLICK.HeightRequest = 0.2 * height;
            image_WRITE_NEW_POST_CLICK.HeightRequest = 0.2 * height;
        }

        private async void FIEND_EXPERIENCES_CLICK(object sender, EventArgs e)
        {
            var page = new ListInfoPage();
            await Navigation.PushAsync(page, true);
        }

        private async void RECENT_POSTS_CLICK(object sender, EventArgs e)
        {
            var page = new RecentPostsPage();
            await Navigation.PushAsync(page, true);
        }

        private void MESSAGES_CLICK(object sender, EventArgs e)
        {
            UserDialogs.Instance.Loading(title: "Open Messager ...").Show();
            if (DependencyService.Get<ICometChatService>().isCometChaInitialize())
            {
                if (DependencyService.Get<ICometChatService>().isCometChatLogin())
                {
                    DependencyService.Get<ICometChatService>().launchCometChatWindow(true, new LaunchCallbackImplementation(successObj => OnSuccessCall(successObj), fail => OnFailCall(fail), onChatroomInfo => OnChatroomInfo(onChatroomInfo), onError => OnError(onError), onLogout => OnLogout(onLogout), onMessageReceive => OnMessageReceive(onMessageReceive), onUserInfo => OnUserInfo(onUserInfo), onWindowClose => OnWindowClose(onWindowClose)));
                }
                else
                {
                    TravellerApp.Models.User User = Realm.GetInstance().Find<TravellerApp.Models.User>(DBLocalID.USER);
                    // Login comet chat.
                    DependencyService.Get<ICometChatService>().loginWithUID(User.traveller_token, new Callbacks(success => loginCometChatSuccess(success), fail => loginCometChatFail(fail)));
                }
            }
            else
            {
                // Init comet chat.
                DependencyService.Get<ICometChatService>().initializeCometChat(CometChatConstants.siteurl, CometChatConstants.licenseKey, CometChatConstants.apiKey, CometChatConstants.isCometOnDemand, new Callbacks(success => initCometChatSuccess(success), fail => initCometChatFail(fail)));
            }
        }

        private async void VISITS_CLICK(object sender, EventArgs e)
        {
            var page = new PlacesVisitedPage();
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

                JObject jsonAuth = new JObject
                {
                    { "login",  AuthService.UserName},
                    { "password",  AuthService.Password},
                    { "db",  ServerAuth.DB}
                };

                JObject jsonDataObject = new JObject
                {
                    { "auth",  jsonAuth }
                };

                JObject jsonData = new JObject
                {
                    { "params", jsonDataObject }
                };

                await WebService.Instance.PostAsync<CurrentlyAtResponse>(ApiUri.BASE_URL + ApiUri.CURRENTLY_AT, content: jsonData, onSuccess: (res) =>
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

        private async void CHECK_IN_CLICK(object sender, EventArgs e)
        {
            var page = new HomePage();
            await Navigation.PushAsync(page, true);
        }

        private async void WRITE_NEW_POST_CLICK(object sender, EventArgs e)
        {
            var page = new WriteReviewPage(new Review());
            await Navigation.PushAsync(page, true);
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

        private void initCometChatSuccess(string success)
        {
            if (success != null)
            {
                System.Console.WriteLine("initCometChatSuccess" + success.ToString());
                TravellerApp.Models.User User = Realm.GetInstance().Find<TravellerApp.Models.User>(DBLocalID.USER);
                // Login comet chat.
                DependencyService.Get<ICometChatService>().loginWithUID(User.traveller_token, new Callbacks(successlogin => loginCometChatSuccess(successlogin), fail => loginCometChatFail(fail)));
            }
        }

        private void initCometChatFail(string fail)
        {
            UserDialogs.Instance.Loading().Hide();
            if (fail != null)
            {
                System.Console.WriteLine("initCometChatFail" + fail.ToString());
            }
        }

        private void loginCometChatSuccess(string success)
        {
            if (success != null)
            {
                System.Console.WriteLine("loginCometChatSuccess" + success.ToString());
                DependencyService.Get<ICometChatService>().launchCometChatWindow(true, new LaunchCallbackImplementation(successObj => OnSuccessCall(successObj), fail => OnFailCall(fail), onChatroomInfo => OnChatroomInfo(onChatroomInfo), onError => OnError(onError), onLogout => OnLogout(onLogout), onMessageReceive => OnMessageReceive(onMessageReceive), onUserInfo => OnUserInfo(onUserInfo), onWindowClose => OnWindowClose(onWindowClose)));
            }
        }

        private void loginCometChatFail(string fail)
        {
            UserDialogs.Instance.Loading().Hide();
            if (fail != null)
            {
                System.Console.WriteLine("loginCometChatSuccess" + fail.ToString());
            }
        }

        private void OnLaunchCometChat(object sender, EventArgs args)
        {
            var cometchat = DependencyService.Get<ICometChatService>();
            cometchat.launchCometChatWindow(true, new LaunchCallbackImplementation(successObj => OnSuccessCall(successObj), fail => OnFailCall(fail), onChatroomInfo => OnChatroomInfo(onChatroomInfo), onError => OnError(onError), onLogout => OnLogout(onLogout), onMessageReceive => OnMessageReceive(onMessageReceive), onUserInfo => OnUserInfo(onUserInfo), onWindowClose => OnWindowClose(onWindowClose)));
        }

        private void OnSuccessCall(String successObj)
        {
            UserDialogs.Instance.Loading().Hide();
            if (successObj != null)
            {
                System.Console.WriteLine("loginSuccess " + successObj.ToString());
            }
        }

        private void OnFailCall(String fail)
        {
            UserDialogs.Instance.Loading().Hide();
            if (fail != null)
            {
                System.Console.WriteLine("OnFailCall " + fail.ToString());
            }
        }

        private void OnChatroomInfo(String onChatroomInfo)
        {
            if (onChatroomInfo != null)
            {
                System.Console.WriteLine("OnChatroomInfo " + onChatroomInfo.ToString());
            }
        }

        private void OnError(String onError)
        {
            if (onError != null)
            {
                System.Console.WriteLine("OnError " + onError.ToString());
            }
        }

        private void OnLogout(String onError)
        {
            if (onError != null)
            {
                System.Console.WriteLine("OnLogout " + onError.ToString());
            }
        }

        private void OnMessageReceive(String onMessageReceive)
        {
            if (onMessageReceive != null)
            {
                System.Console.WriteLine("OnMessageReceive " + onMessageReceive.ToString());
            }
        }

        private void OnUserInfo(String onUserInfo)
        {
            if (onUserInfo != null)
            {
                System.Console.WriteLine("OnUserInfo " + onUserInfo.ToString());
            }
        }

        private void OnWindowClose(String onWindowClose)
        {
            if (onWindowClose != null)
            {
                System.Console.WriteLine("OnWindowClose " + onWindowClose.ToString());
            }
        }
    }
}