using Android.Content;
using Android.Support.V4.Content;
using KamooniHost.Droid.Renderers;

using Xamarin.Forms;
using Xamarin.Forms.Controls;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(RoundedEntry), typeof(RoundedEntryRenderer))]

namespace KamooniHost.Droid.Renderers
{
    public class RoundedEntryRenderer : EntryRenderer
    {
        public RoundedEntryRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement == null && Element != null)
            {
                if (Control != null && Element.Keyboard == Keyboard.Numeric)
                {
                    Control.InputType = Android.Text.InputTypes.ClassNumber | Android.Text.InputTypes.NumberFlagDecimal;
                }
            }

            if (Control != null)
            {
                Control.Background = ContextCompat.GetDrawable(Android.App.Application.Context, Resource.Drawable.RoundedBackground);
                Control.SetPadding(20, 20, 20, 20);
                Control.TextAlignment = Android.Views.TextAlignment.Gravity;
            }
        }
    }
}