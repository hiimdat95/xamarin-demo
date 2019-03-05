using SQLite;
using System;
using Xamarin.Forms;

namespace KamooniHost.Models
{
    public enum HomeMenuItemType
    {
        PostsAndReviews,
        Home,
        CheckIn,
        Bookings,
        CheckOut,
        AddBooking,
        GuestBook,
        Stats,
        Settings,
        LogOut
    }

    [Table("home_menu")]
    public class HomeMenuItem : BaseModel
    {
        public HomeMenuItem()
        {
            TextColor = Color.White;
            BackgroundColor = (Color)Application.Current.Resources["colorPrimary"];
        }

        private HomeMenuItemType id;
        [Column("id")]
        public HomeMenuItemType Id { get => id; set => SetProperty(ref id, value); }

        private string title;
        [Column("title")]
        public string Title { get => title; set => SetProperty(ref title, value); }

        private string icon;
        [Column("icon")]
        public string Icon { get => icon; set => SetProperty(ref icon, value); }

        private Color textColor;
        [Ignore]
        public Color TextColor { get => textColor; set => SetProperty(ref textColor, value); }

        private Color backgroundColor;
        [Ignore]
        public Color BackgroundColor { get => backgroundColor; set => SetProperty(ref backgroundColor, value); }

        private Type targetType;
        [Ignore]
        public Type TargetType { get => targetType; set => SetProperty(ref targetType, value); }

        private bool hide;
        [Column("hide")]
        public bool Hide { get => hide; set => SetProperty(ref hide, value); }
    }
}