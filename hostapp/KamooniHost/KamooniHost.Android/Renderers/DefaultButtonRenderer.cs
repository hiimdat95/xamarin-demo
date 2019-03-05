using Android.Content;
using KamooniHost.Droid.Renderers;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Button), typeof(DefaultButtonRenderer))]

namespace KamooniHost.Droid.Renderers
{
    public class DefaultButtonRenderer : ButtonRenderer
    {
        public DefaultButtonRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement == null && Element != null)
            {
            }

            if (Control != null)
            {
                Control.SetPadding(20, 10, 20, 10);
            }
        }
    }
}