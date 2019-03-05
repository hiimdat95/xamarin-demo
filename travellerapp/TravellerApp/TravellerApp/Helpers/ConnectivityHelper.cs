using Plugin.Connectivity;
using Xamarin.Forms;

namespace TravellerApp.Helpers
{
    public class ConnectivityHelper
    {
        public static bool IsNetworkAvailable()
        {
            if (!CrossConnectivity.IsSupported)
                return true;

            if (!CrossConnectivity.Current.IsConnected)
            {
                //You are offline, notify the user
                Application.Current.MainPage.DisplayAlert("Network", "You are not connected to the internet", "OK");

                return false;
            }

            return true;
        }
    }
}