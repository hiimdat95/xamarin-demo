using Android.Content;
using Android.Support.V4.Content;
using KamooniHost.Droid.Renderers;

using Xamarin.Forms;
using Xamarin.Forms.Controls;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(RoundedNoKeyboardEntry), typeof(RoundedNoKeyboardEntryRenderer))]

namespace KamooniHost.Droid.Renderers
{
    public class RoundedNoKeyboardEntryRenderer : EntryRenderer
    {
        public RoundedNoKeyboardEntryRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.Background = ContextCompat.GetDrawable(Android.App.Application.Context, Resource.Drawable.RoundedBackground);
                Control.SetPadding(20, 20, 20, 20);
                Control.TextAlignment = Android.Views.TextAlignment.Gravity;

                // Disable the Keyboard on Focus
                Control.ShowSoftInputOnFocus = false;
            }
        }
    }
}