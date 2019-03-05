using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Realms;
using System.Collections.Generic;
using TravellerApp.Constants;
using TravellerApp.Interfaces;
using TravellerApp.Models;
using TravellerApp.Utils;
using TravellerApp.Views;
using Xamarin.Forms;
using Xamarin.Forms.Converters;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace TravellerApp
{
    public partial class App : Application
    {
        public static MainTabbedPage MainTabbedPage { get; set; }

        public static JsonSerializerSettings DefaultSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            Converters = new List<JsonConverter>()
            {
                new IgnoreDataTypeConverter(),
                new IgnoreFalseStringConverter()
            }
        };

        private const string appName = "Kamooni Traveller";

        public static string AppName => appName;

        public App()
        {
            InitializeComponent();

            Init();

            if (AppSettings.LoggedIn)
            {
                var user = Realm.GetInstance().Find<User>(DBLocalID.USER);
                DependencyService.Get<IAuthService>().onLoginSuccess(user.traveller_token);

                NavigationPage.SetHasNavigationBar(this, false);

                MainPage = new MainNavigation();
            }
            else
            {
                MainPage = new NavigationPage(new LoginPage())
                {
                    BarTextColor = Color.White
                };
            }
        }

        private void Init()
        {
            DependencyService.Register<IRestClient, RestClient>();

            // Init comet chat.
            DependencyService.Get<ICometChatService>()?.initializeCometChat(CometChatConstants.siteurl, CometChatConstants.licenseKey, CometChatConstants.apiKey, CometChatConstants.isCometOnDemand, null);
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}