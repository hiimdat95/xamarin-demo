using Acr.UserDialogs;
//using Android.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plugin.Geolocator.Abstractions;
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
using TravellerApp.Helpers;
using TravellerApp.Models;
using TravellerApp.Models.Response;
using TravellerApp.Models.Result;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using Xamarin.Forms.Xaml;

namespace TravellerApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListInfoPage : TabbedPage, IUpdate
    {
        private User user;

        private List<ListInfo> listHostOriginal;
        private List<ListInfo> listHost1;
        private List<ListInfo> listHost2;
        private List<Places> listPlaces;
        private List<Types> listTypes;

        private Frame lastFilter;

        public ListInfoPage()
        {
            //LiveReload.Init();
            InitializeComponent();

            var realm = Realm.GetInstance();
            user = realm.Find<User>(DBLocalID.USER);

            srbSearch.TextChanged += SrbSearch_TextChanged;

            sbrHostMap.TextChanged += SbrHostMap_TextChanged;

            listHostMap.Refreshing += ListHostMap_Refreshing;
            listHostMap.ItemTapped += ListHostMap_ItemTapped;

            maps.InfoWindowClicked += Maps_InfoWindowClicked;

            //maps.PinClicked += Maps_PinClicked;

            CheckLocation();
        }

        //public HostListing GetListInfo(string token)
        //{                                
        //    foreach (var host in listHostOriginal)
        //    {
        //        var listInfo = listHostOriginal.Where(x => x.usr_token.Equals(token)).FirstOrDefault();
        //        return listInfo;
        //    }

        //    return new HostListing();
        //}

        private void CheckLocation()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                if (await LocationHelper.IsGeolocationAvailable())
                {
                    maps.MyLocationEnabled = true;
                    maps.UiSettings.MyLocationButtonEnabled = true;

                    InitContentAsync();
                }
                else
                {
                    await DisplayAlert("Location", "To use this feature, the location permission is required. Please go into Settings and turn on location permission.", "OK");
                    Application.Current.MainPage = new MainNavigation();
                }
            });
        }

        async void OnAddPlaceTapped(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new AddPlacePage(this), true);
        }

        public void OnUpdate(bool update)
        {
            CheckLocation();
        }

        private async void InitContentAsync()
        {
            UserDialogs.Instance.Loading(title: "Finding places that will change you!").Show();

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
                HttpResponseMessage response = await client.PostAsync(ApiUri.BASE_URL + ApiUri.FETCH_ALL_HOSTS, content);
                string responseContent = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    UserDialogs.Instance.Loading().Hide();
                    Debug.WriteLine("RESPONSE-GetInformation: " + responseContent);

                    ListInfoResponse listInfoResponse = JsonConvert.DeserializeObject<ListInfoResponse>(responseContent);
                    ListInfoResult listResult = listInfoResponse.result;

                    if (listResult.success)
                    {
                        listHostOriginal = listResult.hosts;
                        listHost1 = listResult.hosts;
                        listHost2 = listResult.hosts;

                        GetListHostMap();
                        FetchPlaces();
                        await GetNearestHosts();
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

        private async void FetchPlaces()
        {
            UserDialogs.Instance.Loading(title: "Processing Get Information...").Show();

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
            Debug.WriteLine("REQUEST-FetchPlaces: " + data);

            try
            {
                HttpResponseMessage response = await client.PostAsync(ApiUri.BASE_URL + ApiUri.FETCH_PLACES, content);
                string responseContent = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    UserDialogs.Instance.Loading().Hide();
                    Debug.WriteLine("RESPONSE-FetchPlaces: " + responseContent);

                    ListPlacesResponse listPlacesResponse = JsonConvert.DeserializeObject<ListPlacesResponse>(responseContent, App.DefaultSerializerSettings);
                    ListPlacesResult listPlacesResult = listPlacesResponse.Result;

                    listPlaces = listPlacesResult.places.ToList();

                    var location = await LocationHelper.GetCurrentPosition(50);
                    maps.MoveToRegion(MapSpan.FromCenterAndRadius(new Xamarin.Forms.GoogleMaps.Position(location.Latitude, location.Longitude), Distance.FromMeters(1000)));

                    foreach (var place in listPlaces)
                    {
                        place.distance = location.CalculateDistance(new Plugin.Geolocator.Abstractions.Position(place.latitude, place.longitude), GeolocatorUtils.DistanceUnits.Kilometers);
                        place.distance_display = place.distance > 1 ? place.distance.ToString("N0") + " km" : (place.distance / 1000).ToString("N0") + " m";
                    }

                    listPlaceOfInterest.ItemsSource = listPlaces
                        .Where(x => x.longitude != 0 && x.latitude != 0)
                        .OrderByDescending(x => x.id)
                        .ToList();

                    await FetchPlaceTypes();
                    await GetNearestPlaces();
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

        private async Task FetchPlaceTypes()
        {
            UserDialogs.Instance.Loading(title: "Processing Get Information...").Show();

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
            Debug.WriteLine("REQUEST-FetchPlaces: " + data);

            try
            {
                HttpResponseMessage response = await client.PostAsync(ApiUri.BASE_URL + ApiUri.FETCH_PLACE_TYPES, content);
                string responseContent = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    UserDialogs.Instance.Loading().Hide();
                    Debug.WriteLine("RESPONSE-FetchPlaces: " + responseContent);

                    ListPlaceTypesResponse listPlaceTypesResponse = JsonConvert.DeserializeObject<ListPlaceTypesResponse>(responseContent);
                    ListPlaceTypesResult listPlaceTypesResult = listPlaceTypesResponse.Result;

                    listTypes = listPlaceTypesResult.Types.ToList();

                    if (listPlaceTypesResult.Success)
                    {
                        foreach (var place in listPlaces)
                        {
                            foreach (var type in listTypes)
                            {
                                if (!string.IsNullOrWhiteSpace(place.place_id[0]) && !"false".Equals(place.place_id[0]))
                                {
                                    if (Convert.ToInt32(place.place_id[0]) == type.id)
                                    {
                                        place.image = type.image;
                                    }
                                }
                                
                            }
                        }
                        
                        foreach (var place in listPlaces)
                        {
                            ImageSource image = !(place.image is string base64) ? null : ImageSource.FromStream(() 
                                                                                    => new MemoryStream(Convert.FromBase64String(base64)));
                            LayoutOptions Center = default;
                            //var img = Convert.FromBase64String(place.image);
                            //var icons = Android.Graphics.BitmapFactory.DecodeByteArray(img, 0, img.Length);
                            //var bitmap = Android.Graphics.Bitmap.CreateBitmap(icons);
                            //var contents = new StreamContent(new MemoryStream(img));
                            //var abc = Base64.EncodeToString(img, Base64Flags.Default);
                            maps.Pins.Add(new Pin()
                            {

                                Label = place.name,
                                //Address = host.province,
                                Icon = BitmapDescriptorFactory.FromView(
                                        new ContentView
                                        {
                                            HeightRequest = 40,
                                            WidthRequest = 25,
                                            Content = new Image
                                            {
                                                Source = image,
                                                Aspect = Aspect.AspectFit,
                                                HeightRequest = 40,
                                                WidthRequest = 25,
                                                VerticalOptions = Center,
                                                HorizontalOptions = Center
                                            }
                                        }),
                                Position = new Xamarin.Forms.GoogleMaps.Position(place.latitude, place.longitude),
                                Type = PinType.Place,
                                BindingContext = place,
                                ZIndex = place.id,
                                IsDraggable = false
                            });
                        }
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
        /*
        private void pinMapsPlaces()
        {
            
            foreach (var place in listPlaces)
            {
                ImageSource image = !(place.image is string base64) ? null : ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(base64)));
                var img = Convert.FromBase64String(place.image);
                var icons = Android.Graphics.BitmapFactory.DecodeByteArray(img, 0, img.Length);
                var bitmap = Android.Graphics.Bitmap.CreateBitmap(icons);
                
                var contents = new StreamContent(new MemoryStream(img));


                var abc = Base64.EncodeToString(img, Base64Flags.Default);
                maps.Pins.Add(new Pin()
                {
                    Label = place.name,
                    //Address = host.province,
                    Icon = BitmapDescriptorFactory.FromView(
                            new ContentView
                            {
                                WidthRequest = 10,
                                HeightRequest = 10,
                                Content = new Image
                                {
                                    Source = image
                                }

                             }),
                    Position = new Xamarin.Forms.GoogleMaps.Position(place.latitude, place.longitude),
                    Type = PinType.Place,
                    BindingContext = place,
                    ZIndex = place.id,
                    IsDraggable = false
                });
            }
        }
        */
        private async Task GetNearestPlaces()
        {
            if (!await LocationHelper.IsGeolocationAvailable())
            {
                return;
            }

            var location = await LocationHelper.GetCurrentPosition(50);
            maps.MoveToRegion(MapSpan.FromCenterAndRadius(new Xamarin.Forms.GoogleMaps.Position(location.Latitude, location.Longitude), Distance.FromMeters(1000)));

            //listVisit = listVisit.ToList();

            foreach (var place in listPlaces)
            {
                place.distance = location.CalculateDistance(new Plugin.Geolocator.Abstractions.Position(place.latitude, place.longitude), GeolocatorUtils.DistanceUnits.Kilometers);
                place.distance_display = place.distance > 1 ? place.distance.ToString("N0") + " km" : (place.distance / 1000).ToString("N0") + " m";
            }
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
                listHostMap.IsRefreshing = false;
                return;
            }

            maps.MoveToRegion(MapSpan.FromCenterAndRadius(new Xamarin.Forms.GoogleMaps.Position(location.Latitude, location.Longitude), Distance.FromMeters(1000)));

            listHost1 = listHostOriginal.ToList();

            foreach (var host in listHost1)
            {
                host.distance = location.CalculateDistance(new Plugin.Geolocator.Abstractions.Position(host.partner_latitude, host.partner_longitude), GeolocatorUtils.DistanceUnits.Kilometers);
                host.distance_display = host.distance > 1 ? host.distance.ToString("N0") + " km" : (host.distance / 1000).ToString("N0") + " m";
            }

            listHostInfo.ItemsSource = null;
            listHostInfo.ItemsSource = listHost1.OrderBy(h => h.distance).ToList();

            listHost2 = listHostOriginal.ToList();

            foreach (var host in listHost2)
            {
                host.distance = location.CalculateDistance(new Plugin.Geolocator.Abstractions.Position(host.partner_latitude, host.partner_longitude), GeolocatorUtils.DistanceUnits.Kilometers);
                host.distance_display = host.distance > 1 ? host.distance.ToString("N0") + " km" : (host.distance / 1000).ToString("N0") + " m";
            }

            listHostMap.ItemsSource = null;
            listHostMap.ItemsSource = listHost2.OrderBy(h => h.distance).ToList();
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

        private void SrbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void OnAccomodationClick(object sender, EventArgs e)
        {
            listHostInfo.IsVisible = true;
            placeOfInterest_group.IsVisible = false;

            if (lastFilter == filterAccomodation)
            {
                lastFilter = null;

                SeparatorAcc.BackgroundColor = (Color)Application.Current.Resources["Color_Background"];
                //filterAccomodation.BorderColor = (Color)Application.Current.Resources["Color_Background"];
                //filterTitleAccomodation.BackgroundColor = (Color)Application.Current.Resources["Color_Background"];

                listHostInfo.ItemsSource = null;
                listHostInfo.ItemsSource = listHostOriginal?.OrderBy(h => h.name).ToList();
            }
            else
            {
                lastFilter = filterAccomodation;

                SeparatorAcc.BackgroundColor = (Color)Application.Current.Resources["Color_SeparatorButton"];
                SeparatorAdv.BackgroundColor = (Color)Application.Current.Resources["Color_Background"];
                //SeparatorTr.BackgroundColor = (Color)Application.Current.Resources["Color_Background"];
                //filterAccomodation.BorderColor = (Color)Application.Current.Resources["Color_Background"];
                //filterAdventures.BorderColor = (Color)Application.Current.Resources["Color_Background"];
                //filterTravel.BorderColor = (Color)Application.Current.Resources["Color_Background"];

                //filterTitleAccomodation.BackgroundColor = (Color)Application.Current.Resources["Color_Background"];
                //filterTitleAdventures.BackgroundColor = (Color)Application.Current.Resources["Color_Background"];
                //filterTitleTravel.BackgroundColor = (Color)Application.Current.Resources["Color_Background"];

                listHostInfo.ItemsSource = null;
                listHostInfo.ItemsSource = listHost1?.FindAll(h => h.is_accommodation).OrderBy(h => h.name).ToList();
            }
        }

        private void OnPlaceOfInterest(object sender, EventArgs e)
        {
            listHostInfo.IsVisible = false;
            placeOfInterest_group.IsVisible = true;

            if (lastFilter == placeOfInterest)
            {
                lastFilter = null;
                SeparatorAdv.BackgroundColor = (Color)Application.Current.Resources["Color_Background"];

                listPlaceOfInterest.ItemsSource = listPlaces
                        .Where(x => x.longitude != 0 && x.latitude != 0)
                        .OrderByDescending(x => x.id)
                        .ToList();
            }
            else
            {
                lastFilter = placeOfInterest;

                SeparatorAcc.BackgroundColor = (Color)Application.Current.Resources["Color_Background"];
                SeparatorAdv.BackgroundColor = (Color)Application.Current.Resources["Color_SeparatorButton"];
                if (listPlaces == null)
                    return;
                listPlaceOfInterest.ItemsSource = listPlaces
                        .Where(x => x.longitude != 0 && x.latitude != 0)
                        .OrderByDescending(x => x.id)
                        .ToList();
            }
        }

        async void addNewPlaceOfInterestOnclick(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddPlacePage(this), true);
        }

        private void OnAdventuresClick(object sender, EventArgs e)
        {
            //if (lastFilter == filterAdventures)
            //{
            //    lastFilter = null;

            //    SeparatorAdv.BackgroundColor = (Color)Application.Current.Resources["Color_Background"];
            //    //filterAdventures.BorderColor = (Color)Application.Current.Resources["Color_Background"];
            //    //filterTitleAdventures.BackgroundColor = (Color)Application.Current.Resources["Color_Background"];

            //    listHostInfo.ItemsSource = null;
            //    listHostInfo.ItemsSource = listHostOriginal?.OrderBy(h => h.name).ToList();
            //}
            //else
            //{
            //    lastFilter = filterAdventures;

            //    SeparatorAcc.BackgroundColor = (Color)Application.Current.Resources["Color_Background"];
            //    SeparatorAdv.BackgroundColor = (Color)Application.Current.Resources["Color_SeparatorButton"];
            //    SeparatorTr.BackgroundColor = (Color)Application.Current.Resources["Color_Background"];

            //    //filterAccomodation.BorderColor = (Color)Application.Current.Resources["Color_Background"];
            //    //filterAdventures.BorderColor = (Color)Application.Current.Resources["Color_Background"];
            //    //filterTravel.BorderColor = (Color)Application.Current.Resources["Color_Background"];

            //    //filterTitleAccomodation.BackgroundColor = (Color)Application.Current.Resources["Color_Background"];
            //    //filterTitleAdventures.BackgroundColor = (Color)Application.Current.Resources["Color_Background"];
            //    //filterTitleTravel.BackgroundColor = (Color)Application.Current.Resources["Color_Background"];

            //    listHostInfo.ItemsSource = null;
            //    listHostInfo.ItemsSource = listHost1?.FindAll(h => h.is_activity_company).OrderBy(h => h.name).ToList();
            //}
        }

        private void OnTravelClick(object sender, EventArgs e)
        {
            //if (lastFilter == filterTravel)
            //{
            //    lastFilter = null;

            //    SeparatorTr.BackgroundColor = (Color)Application.Current.Resources["Color_Background"]; ;
            //    //filterTravel.BorderColor = (Color)Application.Current.Resources["Color_Background"];
            //    //filterTitleTravel.BackgroundColor = (Color)Application.Current.Resources["Color_Background"];

            //    listHostInfo.ItemsSource = null;
            //    listHostInfo.ItemsSource = listHostOriginal?.OrderBy(h => h.name).ToList();
            //}
            //else
            //{
            //    lastFilter = filterTravel;

            //    SeparatorAcc.BackgroundColor = (Color)Application.Current.Resources["Color_Background"];
            //    SeparatorAdv.BackgroundColor = (Color)Application.Current.Resources["Color_Background"];
            //    SeparatorTr.BackgroundColor = (Color)Application.Current.Resources["Color_SeparatorButton"];

            //    //filterAccomodation.BorderColor = (Color)Application.Current.Resources["Color_Background"];
            //    //filterAdventures.BorderColor = (Color)Application.Current.Resources["Color_Background"];
            //    //filterTravel.BorderColor = (Color)Application.Current.Resources["Color_Background"];

            //    //filterTitleAccomodation.BackgroundColor = (Color)Application.Current.Resources["Color_Background"];
            //    //filterTitleAdventures.BackgroundColor = (Color)Application.Current.Resources["Color_Background"];
            //    //filterTitleTravel.BackgroundColor = (Color)Application.Current.Resources["Color_Background"];

            //    listHostInfo.ItemsSource = null;
            //    listHostInfo.ItemsSource = listHost1?.FindAll(h => h.is_transport).OrderBy(h => h.name).ToList();
            //}
        }

        private async void ListHostMap_Refreshing(object sender, EventArgs e)
        {
            listHostMap.IsRefreshing = true;

            if (!await LocationHelper.IsGeolocationAvailable())
            {
                listHostMap.IsRefreshing = false;
                return;
            }

            var location = await LocationHelper.GetCurrentPosition(50);

            if (location == null)
            {
                listHostMap.IsRefreshing = false;
                return;
            }

            listHost2 = listHostOriginal.ToList();

            foreach (var host in listHost2)
            {
                host.distance = location.CalculateDistance(new Plugin.Geolocator.Abstractions.Position(host.partner_latitude, host.partner_longitude), GeolocatorUtils.DistanceUnits.Kilometers);
                host.distance_display = host.distance > 1 ? host.distance.ToString("N0") + " km" : (host.distance / 1000).ToString("N0") + " m";
            }

            listHostMap.ItemsSource = null;
            listHostMap.ItemsSource = listHost2.OrderBy(h => h.distance).ToList();

            listHostMap.IsRefreshing = false;
        }

        private void ListHostMap_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (!(e.Item is ListInfo host))
                return;

            maps.MoveToRegion(MapSpan.FromCenterAndRadius(new Xamarin.Forms.GoogleMaps.Position(host.partner_latitude, host.partner_longitude), Distance.FromMeters(1000)));
            maps.SelectedPin = maps.Pins.FirstOrDefault(h => h.ZIndex == host.id);
        }

        public async void Listing_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (!(e.Item is ListInfo host))
                return;

            if (string.IsNullOrWhiteSpace(host.host_url))
                return;
            if (host.is_activity_company)
            {
                var page = new NewBookingPage(host);
                await Navigation.PushAsync(page, true);
            }
            else
            {
                var page = new BookingPage(host);
                await Navigation.PushAsync(page, true);
            }
        }

        private void SbrHostMap_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.NewTextValue))
            {
                listHostMap.ItemsSource = null;
                listHostMap.ItemsSource = listHost2.OrderByDescending(h => h.distance).ToList();
            }
            else
            {
                listHostMap.ItemsSource = null;
                listHostMap.ItemsSource = listHost2.FindAll(h => Xamarin.Forms.Extensions.StringExtensions.Contains(h.name, e.NewTextValue) || Xamarin.Forms.Extensions.StringExtensions.Contains(h.province, e.NewTextValue)).OrderByDescending(h => h.distance).ToList();
            }
        }

        //private void Maps_PinClicked(object sender, EventArgs e)
        //{
        //    //var thePin = sender as PinModel;

        //    //OpenPlace(thePin.Pin.ZIndex);
        //}

        public void ListPlaceOfInterest_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (!(e.Item is Places place))
                return;

            if (string.IsNullOrWhiteSpace(place.id.ToString()))
                return;

            OpenPlace(place.id);
        }

        private async void OpenPlace(int id)
        {
            UserDialogs.Instance.Loading(title: "Processing Get Information...").Show();

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
                        new JProperty("token", "H6V$36A*!?L^G2NXX7U%=GY@"),
                        new JProperty("id", id)
                    })
                })
            };

            var data = @params.ToString();
            var content = new StringContent(data, Encoding.UTF8, "application/json");
            Debug.WriteLine("REQUEST-FetchPlaces: " + data);

            try
            {
                HttpResponseMessage response = await client.PostAsync(ApiUri.BASE_URL + ApiUri.OPEN_PLACE, content);
                string responseContent = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    UserDialogs.Instance.Loading().Hide();
                    Debug.WriteLine("RESPONSE-FetchPlaces: " + responseContent);

                    OpenPlaceResponse openPlaceResponse = JsonConvert.DeserializeObject<OpenPlaceResponse>(responseContent, App.DefaultSerializerSettings);
                    OpenPlaceResult openPlaceResult = openPlaceResponse.Result;

                    if (openPlaceResult.Success)
                    {
                        var page = new OpenPlacesPage(openPlaceResult.Founder, id);
                        await Navigation.PushAsync(page, true);
                        
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


        private async void Maps_InfoWindowClicked(object sender, InfoWindowClickedEventArgs e)
        {
            if (listHost2.Find(h => e.Pin.ZIndex == h.id) is ListInfo host)
            {
                if (string.IsNullOrWhiteSpace(host.host_url))
                    return;

                var page = new BookingPage(host);
                await Navigation.PushAsync(page, true);
            }

            if (listPlaces.Find(p => e.Pin.ZIndex == p.id) is Places place)
            {
                OpenPlace(place.id);
            }
        }

        private void SearchContent_TextChanged(object sender, TextChangedEventArgs e)
        {
            var keyword = srbSearch.Text;
            if (keyword.Length >= 1)
            {
                var listHostInfoSearch = listHostOriginal?.Where(h => h.name.ToLower().Contains(keyword.ToLower())).OrderBy(h => h.name).ToList();
                listHostInfo.ItemsSource = listHostInfoSearch;
            }
            else
            {
                listHostInfo.ItemsSource = listHostOriginal?.OrderBy(h => h.name).ToList();
            }
        }
    }
}