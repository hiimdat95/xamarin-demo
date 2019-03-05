using Android.Widget;
using KamooniHost.Droid.Effects;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ResolutionGroupName("KamooniHost")]
[assembly: ExportEffect(typeof(BorderlessEffect), "BorderlessEffect")]

namespace KamooniHost.Droid.Effects
{
    public class BorderlessEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            try
            {
                var plate = Control.FindViewById(Control.Context.Resources.GetIdentifier("android:id/search_plate", null, null));
                if (plate != null)
                {
                    plate.Background = null;
                }
                
                Control.Background = null;
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