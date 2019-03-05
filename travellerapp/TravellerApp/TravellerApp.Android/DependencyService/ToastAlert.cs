using Android.App;
using Android.Widget;

using TravellerApp.Droid;
using TravellerApp.Interfaces;

[assembly: Xamarin.Forms.Dependency(typeof(ToastAlert))]

namespace TravellerApp.Droid
{
    internal class ToastAlert : IToast
    {
        public void LongAlert(string message)
        {
            Toast.MakeText(
                Application.Context,
                message,
                ToastLength.Long
                ).Show();
        }

        public void ShortAlert(string message)
        {
            Toast.MakeText(
                Application.Context,
                message,
                ToastLength.Short
                ).Show();
        }
    }
}