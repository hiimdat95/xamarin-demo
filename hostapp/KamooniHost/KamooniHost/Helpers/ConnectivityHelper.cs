using Plugin.Connectivity;
using Xamarin.Forms;
using Xamarin.Forms.Extensions;

namespace KamooniHost.Helpers
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
                Application.Current.MainPage.DisplayAlert("Network", "You are not connected to the internet", TranslateExtension.GetValue("ok"));

                return false;
            }

            return true;
        }
    }
}