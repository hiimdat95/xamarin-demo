
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KamooniHost.Views {
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class HostVerificationPage : ContentPage {
		public HostVerificationPage() {
			InitializeComponent();

			MobileEntry.Completed += (s, e) => BankNumberEntry.Focus();
			BankNumberEntry.Completed += (s, e) => BankCodeEntry.Focus();
		}
	}
}