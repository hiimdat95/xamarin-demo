using Android.Graphics.Drawables;
using System;
using TravellerApp.Droid.Effects;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ResolutionGroupName("TravellerApp")]
[assembly: ExportEffect(typeof(BorderlessEffect), "BorderlessEffect")]

namespace TravellerApp.Droid.Effects
{
    public class BorderlessEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            try
            {
                if (Element is SearchBar searchBar)
                {
                    var plate = Control.FindViewById(Control.Resources.GetIdentifier("android:id/search_plate", null, null));
                    if (plate != null)
                        plate.Background = new ColorDrawable(Android.Graphics.Color.Transparent);

                    Control.Background = new ColorDrawable(Android.Graphics.Color.Transparent);

                    return;
                }
                Control.Background = new ColorDrawable(Android.Graphics.Color.Transparent);
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