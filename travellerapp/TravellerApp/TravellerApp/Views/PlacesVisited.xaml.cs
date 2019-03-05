using Acr.UserDialogs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plugin.Geolocator.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TravellerApp.Constants;
using TravellerApp.Helpers;
using TravellerApp.Interfaces;
using TravellerApp.Models;
using TravellerApp.Models.Response;
using TravellerApp.Models.Result;
using TravellerApp.Notifications;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using Xamarin.Forms.Xaml;

namespace TravellerApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PlacesVisitedPage : TabbedPage, IUpdate
    {
        private List<Visits> listVisitLocal;
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

        public PlacesVisitedPage()
        {
            InitializeComponent();

            GetListPlacesVisited();

            sbrVisitMap.TextChanged += SbrHostMap_TextChanged;
            listVisitMap.Refreshing += ListHostMap_Refreshing;
            listVisitMap.ItemTapped += ListHostMap_ItemTapped;

            CheckLocation();
        }

        async void OnAddPlaceTapped(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new AddPlacePage(this), true);
        }

        public void OnUpdate(bool update)
        {
            GetListPlacesVisited();
            CheckLocation();
        }


        private async void GetListPlacesVisited()
        {
            UserDialogs.Instance.Loading(title: "Places Visited...").Show();
            var client = new HttpClient()
            {
                Timeout = TimeSpan.FromSeconds(50)
            };

            var AuthService = DependencyService.Get<IAuthService>();

            JObject @params = new JObject()
            {
                new JProperty("params", new JObject()
                {
                    new JProperty("auth", new JObject()
                    {
                        new JProperty("login", AuthService.UserName),
                        new JProperty("password", AuthService.Password),
                        new JProperty("db", ServerAuth.DB)
                    })
                })
            };

            var data = @params.ToString();
            var content = new StringContent(data, Encoding.UTF8, "application/json");
            Debug.WriteLine("REQUEST-GetListPlacesVisited: " + data);

            try
            {
                HttpResponseMessage response = await client.PostAsync(ApiUri.BASE_URL + ApiUri.GET_VISITS, content);
                string responseContent = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Debug.WriteLine("RESPONSE-GetListPlacesVisited: " + responseContent);

                    VisitResponse visitResponse = JsonConvert.DeserializeObject<VisitResponse>(responseContent, App.DefaultSerializerSettings);
                    VisitResult visitResult = visitResponse.result;

                    if (visitResult.success)
                    {
                        //var a = "GRGW7SL4TTCKGCKKVJ9-GEL27B8M7UW907LDBT0";
                        //var listVisit = visitResult.visits.Where(x => x.booking_token == a).ToList();
                        var listVisit = visitResult.visits.OrderByDescending(x => x.date).ToList();

                        listPlacesVisited.ItemsSource = listVisit;

                        await InitContentAsync();

                        if (listHostOriginal != null && listVisit != null)
                        {
                            foreach (var visit in listVisit)
                            {
                                foreach (var host in listHostOriginal)
                                {
                                    if (visit.Host!=null && visit.Host.Equals(host.name))
                                    {
                                        visit.distance = host.distance;
                                        visit.distance_display = host.distance_display;
                                        visit.partner_longitude = host.partner_longitude;
                                        visit.partner_latitude = host.partner_latitude;
                                        visit.province = host.province;
                                        visit.id = host.id;
                                        visit.city = host.city;
                                        visit.state_id = host.state_id;
                                    }
                                }
                            }

                            listVisitLocal = listVisit.ToList();
                            GetListHostMap();
                            await GetNearestHosts();
                        }

                        UserDialogs.Instance.Loading().Hide();
                        //listVisitMap.ItemsSource = listHostOriginal.OrderBy(h => h.distance).ToList();
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

        private void GetListHostMap()
        {
            foreach (var host in listHostOriginal)
            {
                maps.Pins.Add(new Pin()
                {
                    Label = host.name,
                    Address = host.province,
                    Position = new Xamarin.Forms.GoogleMaps.Position(host.partner_latitude, host.partner_longitude),
                    Type = PinType.Place,
                    BindingContext = host,
                    ZIndex = host.id,
                    IsDraggable = false
                });
            }
        }

        private void ListHostMap_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (!(e.Item is Visits visit))
                return;

            maps.MoveToRegion(MapSpan.FromCenterAndRadius(new Xamarin.Forms.GoogleMaps.Position(visit.partner_latitude, visit.partner_longitude), Distance.FromMeters(1000)));
            maps.SelectedPin = maps.Pins.FirstOrDefault(h => h.ZIndex == visit.id);
        }

        private async void ListHostMap_Refreshing(object sender, EventArgs e)
        {
            listVisitMap.IsRefreshing = true;

            if (!await LocationHelper.IsGeolocationAvailable())
            {
                listVisitMap.IsRefreshing = false;
                return;
            }

            var location = await LocationHelper.GetCurrentPosition(50);

            if (location == null)
            {
                listVisitMap.IsRefreshing = false;
                return;
            }

            //listHost2 = listHostOriginal.ToList();

            foreach (var host in listVisitLocal)
            {
                host.distance = location.CalculateDistance(new Plugin.Geolocator.Abstractions.Position(host.partner_latitude, host.partner_longitude), GeolocatorUtils.DistanceUnits.Kilometers);
                host.distance_display = host.distance > 1 ? host.distance.ToString("N0") + " km" : (host.distance / 1000).ToString("N0") + " m";
            }

            listVisitMap.ItemsSource = null;
            listVisitMap.ItemsSource = listVisitLocal.OrderBy(h => h.distance).ToList();

            listVisitMap.IsRefreshing = false;
        }

        private async Task GetNearestHosts()
        {
            if (!await LocationHelper.IsGeolocationAvailable())
            {
                return;
            }

            var location = await LocationHelper.GetCurrentPosition(50);

            if (location == null)
            {
                listVisitMap.IsRefreshing = false;
                return;
            }

            maps.MoveToRegion(MapSpan.FromCenterAndRadius(new Xamarin.Forms.GoogleMaps.Position(location.Latitude, location.Longitude), Distance.FromMeters(1000)));

            //listVisit = listVisit.ToList();

            foreach (var host in listVisitLocal)
            {
                host.distance = location.CalculateDistance(new Plugin.Geolocator.Abstractions.Position(host.partner_latitude, host.partner_longitude), GeolocatorUtils.DistanceUnits.Kilometers);
                host.distance_display = host.distance > 1 ? host.distance.ToString("N0") + " km" : (host.distance / 1000).ToString("N0") + " m";
            }

            listVisitMap.ItemsSource = null;
            listVisitMap.ItemsSource = listVisitLocal.OrderBy(h => h.distance).ToList();
        }

        private async void OpenBookingClicked(object sender, EventArgs e)
        {
            try
            {
                if (((Button)sender).BindingContext is Visits selectedItem)
                {
                    CurrentlyAtResult currentlyAtResult = new CurrentlyAtResult
                    {
                        URL = selectedItem.URL,
                        Host = selectedItem.Host,
                        image = selectedItem.image,
                        summary = selectedItem.summary,
                        Token = selectedItem.Token,
                        visit_token = selectedItem.visit_token,
                        date = selectedItem.date,
                        date_out = selectedItem.date_out,
                        agreed = selectedItem.agreed,
                        state = selectedItem.state
                    };

                    var page = new CurrentlyAtPage(currentlyAtResult);
                    await Navigation.PushAsync(page, true);
                }
                else
                {
                    await DisplayAlert("Warning", "This Place don't have currently at.", "OK");
                }
            }
            catch (Exception exx)
            {
                Debug.WriteLine(exx.Message);
            }
        }

        private void CheckLocation()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                if (await LocationHelper.IsGeolocationAvailable())
                {
                    maps.MyLocationEnabled = true;
                    maps.UiSettings.MyLocationButtonEnabled = true;

                    await InitContentAsync();
                }
                else
                {
                    await DisplayAlert("Location", "To use this feature, the location permission is required. Please go into Settings and turn on location permission.", "OK");
                    Application.Current.MainPage = new MainNavigation();
                }
            });
        }

        private void SbrHostMap_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.NewTextValue))
            {
                listVisitMap.ItemsSource = null;
                listVisitMap.ItemsSource = listVisitLocal.OrderByDescending(h => h.distance).ToList();
            }
            else
            {
                listVisitMap.ItemsSource = null;
                listVisitMap.ItemsSource = listVisitLocal.FindAll(h => Xamarin.Forms.Extensions.StringExtensions.Contains(h.Host, e.NewTextValue) || Xamarin.Forms.Extensions.StringExtensions.Contains(h.province, e.NewTextValue)).OrderByDescending(h => h.distance).ToList();
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

        private async void WRITE_NEW_POST_CLICK(object sender, EventArgs e)
        {
            var page = new WriteReviewPage();
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
    }
}