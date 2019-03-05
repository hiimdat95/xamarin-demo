using KamooniHost.IDependencyServices;
using Xamarin.Forms;

namespace KamooniHost.Notifications
{
    internal class Internal
    {
        public static void ServerError()
        {
            DependencyService.Get<IToast>().LongAlert("Server Error. Please try again in 1 minute.");
        }

        public static void BadConnectionError()
        {
            DependencyService.Get<IToast>().LongAlert("Bad Connection Error. Try Again.");
        }
    }
}