using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KamooniHost.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CheckInNonFullUserPage : ContentPage
    {
        public CheckInNonFullUserPage()
        {
            InitializeComponent();
        }

        protected override bool OnBackButtonPressed()
        {
            Application.Current.MainPage = new MainPage();
            return true;
        }
    }
}