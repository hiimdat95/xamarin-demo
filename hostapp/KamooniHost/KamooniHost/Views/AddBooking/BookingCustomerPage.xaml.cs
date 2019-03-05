using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KamooniHost.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BookingCustomerPage : ContentPage
    {
        public BookingCustomerPage()
        {
            InitializeComponent();

			NameEntry.Completed += (s, e) => EmailEntry.Focus();
			EmailEntry.Completed += (s, e) => MobileEntry.Focus();
		}
    }
}