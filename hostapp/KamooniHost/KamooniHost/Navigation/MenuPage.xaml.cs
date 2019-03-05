using KamooniHost.Constants;
using KamooniHost.Helpers;
using KamooniHost.Models;
using KamooniHost.Models.Local;
using KamooniHost.ViewModels.Settings;
using SQLite;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.TinyMVVM;
using Xamarin.Forms.Xaml;

namespace KamooniHost
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuPage : ContentPage
    {
        public MenuPage()
        {
            InitializeComponent();

            BindingContext = new MenuPageViewModel();
        }

        public class MenuPageViewModel : TinyViewModel
        {
            private MainPage RootPage => Application.Current.MainPage as MainPage;

            private readonly SQLiteConnection LocalDb;

            public Host Host => Settings.CurrentHost;

            public List<Post> Post => Settings.CurrentPost;

            private HomeMenuItem selectedItem;
            public HomeMenuItem SelectedItem { get => selectedItem; set => SetProperty(ref selectedItem, value); }

            public ObservableCollection<HomeMenuItem> MenuItems { get; set; } = new ObservableCollection<HomeMenuItem>();

            public ICommand MenuItemSelectedCommand { get; set; }

            public MenuPageViewModel()
            {
                LocalDb = TinyIOC.Container.Resolve<SQLiteConnection>();

                InitLocalMasterMenus();
                InitMasterMenus();

                MenuItemSelectedCommand = new Command<HomeMenuItem>(MenuItemSelected);

                MessagingCenter.Subscribe<ModifyMasterMenuViewModel>(this, MessageKey.HOME_MENUS_CHANGED, OnHomeMenusChanged);
            }

            private void InitLocalMasterMenus()
            {
                LocalDb.DeleteAll<HomeMenuItem>();

                if (string.IsNullOrWhiteSpace(Settings.CurrentHost.Url))
                {
                    LocalDb.InsertOrReplace(new HomeMenuItem { Id = HomeMenuItemType.PostsAndReviews, Title = "Posts / Reviews", Icon = "ic_post_review" });
                    //LocalDb.InsertOrReplace(new HomeMenuItem { Id = HomeMenuItemType.Home, Title = "Home", Icon = "ic_menu_home_white" });
                    LocalDb.InsertOrReplace(new HomeMenuItem { Id = HomeMenuItemType.CheckIn, Title = "Check In", Icon = "ic_menu_check_in_white" });
                    LocalDb.InsertOrReplace(new HomeMenuItem { Id = HomeMenuItemType.GuestBook, Title = "Guest Book", Icon = "ic_menu_bookings_white" });
                    LocalDb.InsertOrReplace(new HomeMenuItem { Id = HomeMenuItemType.Settings, Title = "Settings", Icon = "ic_menu_settings_white" });
                }
                else
                {
                    LocalDb.InsertOrReplace(new HomeMenuItem { Id = HomeMenuItemType.PostsAndReviews, Title = "Posts / Reviews", Icon = "ic_post_review" });
                    //LocalDb.InsertOrReplace(new HomeMenuItem { Id = HomeMenuItemType.Home, Title = "Home", Icon = "ic_menu_home_white" });
                    LocalDb.InsertOrReplace(new HomeMenuItem { Id = HomeMenuItemType.CheckIn, Title = "Check In", Icon = "ic_menu_check_in_white" });
                    LocalDb.InsertOrReplace(new HomeMenuItem { Id = HomeMenuItemType.Bookings, Title = "Bookings", Icon = "ic_menu_bookings_white" });
                    LocalDb.InsertOrReplace(new HomeMenuItem { Id = HomeMenuItemType.CheckOut, Title = "Check Out", Icon = "ic_menu_check_out_white" });
                    LocalDb.InsertOrReplace(new HomeMenuItem { Id = HomeMenuItemType.AddBooking, Title = "Add Booking", Icon = "ic_menu_add_booking_white" });
                    LocalDb.InsertOrReplace(new HomeMenuItem { Id = HomeMenuItemType.Stats, Title = "Statistics", Icon = "ic_menu_stats_white", Hide = true });
                    LocalDb.InsertOrReplace(new HomeMenuItem { Id = HomeMenuItemType.Settings, Title = "Settings", Icon = "ic_menu_settings_white" });
                }
            }

            private async void MenuItemSelected(HomeMenuItem selected)
            {
                selected.TextColor = (Color)Application.Current.Resources["colorAccent"];

                foreach (var item in MenuItems.Where(m => m.Id != selected.Id))
                {
                    item.TextColor = Color.White;
                }

                await RootPage.NavigateFromMenu(selected.Id);
            }

            private void OnHomeMenusChanged(ModifyMasterMenuViewModel sender)
            {
                InitMasterMenus();
            }

            private void InitMasterMenus()
            {
                MenuItems.Clear();

                foreach (var menu in LocalDb.Table<HomeMenuItem>().ToList())
                {
                    if (!menu.Hide)
                    {
                        MenuItems.Add(new HomeMenuItem()
                        {
                            Id = menu.Id,
                            Title = menu.Title,
                            Icon = menu.Icon,
                            Hide = menu.Hide
                        });
                    }
                }

                if (SelectedItem == null)
                {
                    SelectedItem = MenuItems.FirstOrDefault();
                    SelectedItem.TextColor = (Color)Application.Current.Resources["colorAccent"];
                }
                else
                {
                    SelectedItem = MenuItems.FirstOrDefault(s => s.Id == SelectedItem.Id);
                    SelectedItem.TextColor = (Color)Application.Current.Resources["colorAccent"];
                }
            }
        }
    }
}