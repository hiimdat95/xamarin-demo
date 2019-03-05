using Android.Content;
using TravellerApp.Droid.Renderers;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Entry), typeof(DefaultEntryRenderer))]

namespace TravellerApp.Droid.Renderers
{
    public class DefaultEntryRenderer : EntryRenderer
    {
        public DefaultEntryRenderer(Context context) : base(context)
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
        }
    }
}