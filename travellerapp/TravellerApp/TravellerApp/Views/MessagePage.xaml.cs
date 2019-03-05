using Acr.UserDialogs;
using Xamarin.Forms;

namespace TravellerApp.Views
{
    public partial class MessagePage : ContentPage
    {
        public MessagePage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            UserDialogs.Instance.Loading(title: "Open chat ...").Show();
        }
    }
}
