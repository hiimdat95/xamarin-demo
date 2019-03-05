using KamooniHost.Helpers;
using KamooniHost.Models;
using KamooniHost.ViewModels;
using KamooniHost.ViewModels.Settings;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Extensions;
using Xamarin.Forms.TinyMVVM;
using Xamarin.Forms.Xaml;

namespace KamooniHost
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : MasterDetailPage
    {
        public Host Host => Settings.CurrentHost;

        public List<Post> Post => Settings.CurrentPost;

        public MainPage()
        {
            InitializeComponent();

            Detail = new NavigationContainer(ViewModelResolver.ResolveViewModel<PostsAndReviewsViewModel>());


        }
        

        public async Task NavigateFromMenu(HomeMenuItemType type)
        {
            Debug.WriteLine("ITEM SELECTED");

            Page selectedPage = null;

            switch (type)
            {
                case HomeMenuItemType.Home:
                    selectedPage = ViewModelResolver.ResolveViewModel<HomeViewModel>();
                    break;

                case HomeMenuItemType.CheckIn:
                    selectedPage = string.IsNullOrWhiteSpace(Host.Url) ? selectedPage = ViewModelResolver.ResolveViewModel<CheckInNonFullUserViewModel>() : selectedPage = ViewModelResolver.ResolveViewModel<CheckInViewModel>();
                    break;

                case HomeMenuItemType.Bookings:
                    selectedPage = ViewModelResolver.ResolveViewModel<BookingsViewModel>();
                    break;

                case HomeMenuItemType.CheckOut:
                    selectedPage = ViewModelResolver.ResolveViewModel<CheckOutViewModel>();
                    break;

                case HomeMenuItemType.AddBooking:
                    selectedPage = ViewModelResolver.ResolveViewModel<BookingCalendarViewModel>();
                    break;

                case HomeMenuItemType.GuestBook:
                    selectedPage = ViewModelResolver.ResolveViewModel<GuestBookViewModel>();
                    break;

                case HomeMenuItemType.Stats:
                    selectedPage = ViewModelResolver.ResolveViewModel<StatsViewModel>();
                    break;

                case HomeMenuItemType.Settings:
                    selectedPage = ViewModelResolver.ResolveViewModel<SettingsViewModel>();
                    break;

                case HomeMenuItemType.PostsAndReviews:
                    selectedPage = ViewModelResolver.ResolveViewModel<PostsAndReviewsViewModel>();
                    break;

                case HomeMenuItemType.LogOut:
                    await LogOut();
                    break;

                default:
                    selectedPage = ViewModelResolver.ResolveViewModel<PostsAndReviewsViewModel>();
                    break;
            }

            if (selectedPage == null || type == HomeMenuItemType.LogOut)
                return;
            
            if (Detail is NavigationPage detailPage)
            {
                detailPage.NotifyAllChildrenPopped();
            }

            // Default [Works for iOS]
            Detail = new NavigationContainer(selectedPage)
            {
                BarBackgroundColor = (Color)Application.Current.Resources["colorPrimary"],
                BarTextColor = Color.White
            };

            if (Device.RuntimePlatform == Device.Android)
                await Task.Delay(100);

            // Work around for nav drawer hide lag [Android]
            //var root = Detail.Navigation.NavigationStack[0];
            //Detail.Navigation.InsertPageBefore(page, root);
            //await Detail.Navigation.PopToRootAsync(false);

            IsPresented = false;
        }

        private async Task LogOut()
        {
            if (await DisplayAlert(TranslateExtension.GetValue("alert_title_log_out"), TranslateExtension.GetValue("alert_message_log_out_confirm"), TranslateExtension.GetValue("alert_message_yes"), TranslateExtension.GetValue("alert_message_no")))
            {
                Settings.LoggedIn = false;

                Application.Current.MainPage = new NavigationContainer(ViewModelResolver.ResolveViewModel<LoginViewModel>())
                {
                    BarBackgroundColor = Color.DodgerBlue,
                    BarTextColor = Color.White
                };
            }
        }
    }
}