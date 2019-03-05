using Android.Content;
using KamooniHost.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Controls;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(NoKeyboardEntry), typeof(NoKeyboardEntryRenderer))]

namespace KamooniHost.Droid.Renderers
{
    public class NoKeyboardEntryRenderer : EntryRenderer
    {
        public NoKeyboardEntryRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                // Disable the Keyboard on Focus
                Control.ShowSoftInputOnFocus = false;
            }
        }
    }
}