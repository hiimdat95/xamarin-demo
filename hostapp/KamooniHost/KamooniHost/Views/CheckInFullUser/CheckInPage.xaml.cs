using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KamooniHost.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CheckInPage : TabbedPage
    {
        public CheckInPage()
        {
            InitializeComponent();

            CurrentPage = Children[1];
        }

        protected override bool OnBackButtonPressed()
        {
            Application.Current.MainPage = new MainPage();
            return true;
        }
    }
}