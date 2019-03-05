using Acr.UserDialogs;
using KamooniHost.Helpers;
using KamooniHost.IServices;
using KamooniHost.Models;
using KamooniHost.Models.Local;
using KamooniHost.RestClient;
using KamooniHost.Services;
using KamooniHost.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Plugin.Connectivity;
using Plugin.Connectivity.Abstractions;
using Premind;
using SQLite;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Threading;
using Xamarin.Forms;
using Xamarin.Forms.Converters;
using Xamarin.Forms.Extensions;
using Xamarin.Forms.TinyMVVM;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace KamooniHost
{
    public partial class App : Application
    {
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

        public App()
        {
            InitializeComponent();
          
           

            // Init Extensions
            ImageResourceExtension.InitImageResourceExtension("AppResources.Assets", typeof(App).GetTypeInfo().Assembly);
            TranslateExtension.InitTranslateExtension("AppResources.Localization.Resources", CultureInfo.CurrentCulture, typeof(App).GetTypeInfo().Assembly);

            RegisterDependency();

            Init();

            if (Settings.LoggedIn)
            {
                Current.MainPage = new MainPage();
            }
            else
            {
                Current.MainPage = new NavigationContainer(ViewModelResolver.ResolveViewModel<LoginViewModel>())
                {
                    BarBackgroundColor = Color.FromHex("#835e7e"),
                    BarTextColor = Color.White
                };
            }
        }

        private void RegisterDependency()
        {
            TinyIOC.Container.Register(DependencyService.Get<ILocalDatabaseService>().GetDatabaseConnection());
            TinyIOC.Container.Register<IRestClient, RestClient.RestClient>();
            TinyIOC.Container.Register<IHostService, HostService>();
            TinyIOC.Container.Register<ICheckInService, CheckInService>();
            TinyIOC.Container.Register<IBookingsService, BookingsService>();
            TinyIOC.Container.Register<IGuestBookService, GuestBookService>();
            TinyIOC.Container.Register<IStatsService, StatsService>();
            TinyIOC.Container.Register<ISignUpService, SignUpService>();
            TinyIOC.Container.Register<ILoginService, LoginService>();
            TinyIOC.Container.Register<IPostsAndReviewsService, PostsAndReviewsService>();
            TinyIOC.Container.Register<ICountryService, CountryService>();
            TinyIOC.Container.Register<ITermsConditionsService, TermsConditionsService>();
            TinyIOC.Container.Register<IAddPlaceService, AddPlaceService>();
        }

        private  void  Init()
        {
            TinyIOC.Container.Resolve<SQLiteConnection>()?.CreateTable<UserData>();
            TinyIOC.Container.Resolve<SQLiteConnection>()?.CreateTable<HomeMenuItem>();

            CultureInfo ci = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
          
        }

        private async void Current_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            if (e.IsConnected)
            {
                await PremindClient.StartClient();
            }
        }

        private async void Current_ConnectivityTypeChanged(object sender, ConnectivityTypeChangedEventArgs e)
        {
            if (e.IsConnected)
            {
                await PremindClient.StartClient();
            }
        }

        protected override void OnStart()
        {
            // Handle when your app starts

            //await PremindClient.StartClient();

            CrossConnectivity.Current.ConnectivityChanged += Current_ConnectivityChanged;
            CrossConnectivity.Current.ConnectivityTypeChanged += Current_ConnectivityTypeChanged;
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps

            CrossConnectivity.Current.ConnectivityChanged -= Current_ConnectivityChanged;
            CrossConnectivity.Current.ConnectivityTypeChanged -= Current_ConnectivityTypeChanged;
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}