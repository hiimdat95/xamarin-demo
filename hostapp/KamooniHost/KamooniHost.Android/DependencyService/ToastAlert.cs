using System;
using Android.App;
using Android.Widget;
using KamooniHost.Droid.DependencyService;
using KamooniHost.IDependencyServices;

[assembly: Xamarin.Forms.Dependency(typeof(ToastAlert))]
namespace KamooniHost.Droid.DependencyService
{
    public class ToastAlert : IToast
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
