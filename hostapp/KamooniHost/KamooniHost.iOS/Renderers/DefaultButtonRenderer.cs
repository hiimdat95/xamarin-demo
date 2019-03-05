using KamooniHost.iOS.Renderers;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Button), typeof(DefaultButtonRenderer))]

namespace KamooniHost.iOS.Renderers
{
    public class DefaultButtonRenderer : ButtonRenderer
    {
        public DefaultButtonRenderer()
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
                Control.ContentEdgeInsets = new UIKit.UIEdgeInsets(20, 10, 20, 10);
            }
        }
    }
}