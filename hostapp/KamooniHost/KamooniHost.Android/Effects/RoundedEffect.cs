using Android.Support.V4.Content;
using KamooniHost.Droid.Effects;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportEffect(typeof(RoundedEffect), "RoundedEffect")]

namespace KamooniHost.Droid.Effects
{
    public class RoundedEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            try
            {
                Control.Background = ContextCompat.GetDrawable(Android.App.Application.Context, Resource.Drawable.RoundedBackground);
                Control.SetPadding(20, 10, 20, 10);
                Control.TextAlignment = Android.Views.TextAlignment.Gravity;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Cannot set property on attached control. Error: ", ex.Message);
            }
        }

        protected override void OnDetached()
        {
        }
    }
}