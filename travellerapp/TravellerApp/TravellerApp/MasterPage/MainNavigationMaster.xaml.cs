using Acr.UserDialogs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Realms;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TravellerApp.Constants;
using TravellerApp.Interfaces;
using TravellerApp.Models;
using TravellerApp.Models.Response;
using TravellerApp.Notifications;
using TravellerApp.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TravellerApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainNavigationMaster : ContentPage
    {
        public ListView ListView;

        private User user;

        public MainNavigationMaster()
        {
            InitializeComponent();

            BindingContext = new MainNavigationMasterViewModel();

            ListView = MenuItemsListView;

            var realm = Realm.GetInstance();
            user = realm.Find<User>(DBLocalID.USER);

            GetCheckStatus();
            InitContent();
        }

        private async void Help_Tapped(object sender, EventArgs args)
        {
            await Navigation.PushModalAsync(new ExplainTheStats(), true);
        }

        private async void GetCheckStatus()
        {
            try
            {
                var client = new HttpClient()
                {
                    Timeout = TimeSpan.FromSeconds(30)
                };

                var AuthService = DependencyService.Get<IAuthService>();

                JObject jsonAuth = new JObject
                {
                    { "login",  AuthService.UserName},
                    { "password",  AuthService.Password},
                    { "db",  ServerAuth.DB}
                };

                JObject jsonDataObject = new JObject
                {
                    { "auth",  jsonAuth}
                };

                JObject jsonData = new JObject
                {
                    { "params", jsonDataObject }
                };

                var data = jsonData.ToString();
                var content = new StringContent(data, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(ApiUri.BASE_URL + ApiUri.CHECK_STATUS, content);
                string responseContent = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    CheckStatusResponse successResponse = JsonConvert.DeserializeObject<CheckStatusResponse>(responseContent, App.DefaultSerializerSettings);

                    if (successResponse != null && successResponse.Result != null)
                    {
                        if (successResponse.Result.Success)
                        {
                            lbTotalVisits.Text = successResponse.Result.Status.TotalVisits;
                            lbTotalPost.Text = successResponse.Result.Status.TotalRatings;
                            //btnTotalPoint.Text = successResponse.Result.Status.TotalPoints;
                            lbTotalGiveAMinute.Text = successResponse.Result.Status.TotalGiveAMinute;
                            lbTotalKamoonity.Text = successResponse.Result.Status.TotalKamoonity;
                            lbName.Text = user.name;
                        }
                        else if (!string.IsNullOrWhiteSpace(successResponse.Result.Message))
                        {
                            UserDialogs.Instance.Toast(new ToastConfig(successResponse.Result.Message));
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
            catch (TaskCanceledException)
            {
                UserDialogs.Instance.Toast(new ToastConfig("Bad Connection Error. Try Again"));
            }
            catch (Exception)
            {
                Internal.ServerError();
            }
            finally
            {
                UserDialogs.Instance.Loading().Hide();
            }
        }

        private void InitContent()
        {
            //name.Text = user.name;
            //email.Text = user.email;

            Country country = Country.GetCountryByISO(user.country);
            //lagImage.Source = country.FlagPath;

            byte[] imageBytes = Convert.FromBase64String(user.profile_pic);
            profilePicture.Source = ImageSource.FromStream(() => new MemoryStream(imageBytes));
        }

        public class MainNavigationMasterViewModel : BaseModel
        {
            public ObservableCollection<MainNavigationMenuItem> MenuItems { get; set; }

            private MainNavigationMenuItem selectedItem;
            public MainNavigationMenuItem SelectedItem { get => selectedItem; set => SetProperty(ref selectedItem, value, onChanged: OnSelectedItemChanged); }

            public ICommand NavigateToProfileCommand { get; private set; }

            public MainNavigationMasterViewModel()
            {
                NavigateToProfileCommand = new Command(NavigateToProfile);

                MenuItems = new ObservableCollection<MainNavigationMenuItem>()
                {
                    //new MainNavigationMenuItem { Id = 0, Title = "Home", TargetType = typeof(Home), IconPath = "home1" },
                    new MainNavigationMenuItem { Id = 0, Title = "Recent Posts", TargetType = typeof(RecentPostsPage), IconPath = "home1" },
                    //new MainNavigationMenuItem { Id = 1, Title = "Home", TargetType = typeof(Home), IconPath = "home1" },
                    new MainNavigationMenuItem { Id = 1, Title = "Billing", TargetType = typeof(BillingPage), IconPath = "wallet" },
                    new MainNavigationMenuItem { Id = 2, Title = "Explore", TargetType = typeof(OurStoryPage), IconPath = "explore" },
                    new MainNavigationMenuItem { Id = 3, Title = "Messages", TargetType = typeof(MessagePage), IconPath = "folder" },
                    new MainNavigationMenuItem { Id = 4, Title = "My Bookings", TargetType = typeof(MyBookingsPage), IconPath = "profile" },
                    new MainNavigationMenuItem { Id = 5, Title = "My Profile", TargetType = typeof(MyProfilePage), IconPath = "feed" },
                    //new MainNavigationMenuItem { Id = 7, Title = "Message", TargetType = null, IconPath = "MESSAGES" },

                    //new MainNavigationMenuItem { Id = 1, Title = "Recent Posts", TargetType = typeof(RecentPostsPage), IconPath = "review" },
                    //new MainNavigationMenuItem { Id = 0, Title = "Check In", TargetType = typeof(HomePage), IconPath = "home" },
                    //new MainNavigationMenuItem { Id = 2, Title = "Find Experiences", TargetType = typeof(ListInfoPage), IconPath = "find_experience" },
                    //new MainNavigationMenuItem { Id = 3, Title = "My Bookings", TargetType = typeof(MyBookingsPage), IconPath = "my_booking" },
                    //new MainNavigationMenuItem { Id = 4, Title = "Places Visited", TargetType = typeof(PlacesVisitedPage), IconPath = "place_visit" },
                    //new MainNavigationMenuItem { Id = 5, Title = "Our Story", TargetType = typeof(OurStoryPage), IconPath = "story" },
                    ////new MainNavigationMenuItem { Id = 6, Title = "Peach Payment", TargetType = typeof(TestPage), IconPath = "home" },
                    //new MainNavigationMenuItem { Id = 7, Title = "Message", TargetType = null, IconPath = "home" }
                };
                //GetCurrently();
            }

            private void OnSelectedItemChanged()
            {
                if (SelectedItem != null)
                {
                    foreach (var item in MenuItems)
                    {
                        if (!item.Equals(SelectedItem))
                        {
                            item.TextColor = Color.White;
                        }
                    }
                }
            }

            private async void GetCurrently()
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
                    { "auth",  jsonAuth}
                };

                JObject jsonData = new JObject
                {
                    { "params", jsonDataObject }
                };

                await WebService.Instance.PostAsync<CurrentlyAtResponse>(ApiUri.BASE_URL + ApiUri.CURRENTLY_AT, content: jsonData, onSuccess: (res) =>
                {
                    if (res.result != null && res.result.success)
                    {
                        MenuItems[0].TargetType = typeof(CurrentlyAtPage);
                        MenuItems[0].Params = new object[] { res.result };
                    }
                });
            }

            private void NavigateToProfile(object sender)
            {
                App.MainTabbedPage.NavigateToPage(typeof(MyProfilePage));
                /*if (Application.Current.MainPage is MasterDetailPage masterDetailPage)
                {
                    masterDetailPage.IsPresented = false;
                    await masterDetailPage.Detail.Navigation.PushAsync(new MyProfilePage());
                }*/
            }
        }
    }
}