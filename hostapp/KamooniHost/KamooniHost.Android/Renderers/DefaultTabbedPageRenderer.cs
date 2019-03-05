using Android.Content;
using KamooniHost.Droid.Renderers;
using System.ComponentModel;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android.AppCompat;

[assembly: ExportRenderer(typeof(TabbedPage), typeof(DefaultTabbedPageRenderer))]

namespace KamooniHost.Droid.Renderers
{
    public class DefaultTabbedPageRenderer : TabbedPageRenderer
    {
        public DefaultTabbedPageRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == VisualElement.HeightProperty.PropertyName)
            {
                UpdateHeight();
            }
        }

        private double _previousHeight;

        private void UpdateHeight()
        {
            // Handle size changes because of the soft keyboard (there's probably a more elegant solution to this)

            // This is only necessary if:
            // - we're navigating back from a page where the soft keyboard was open when the user hit the Navigation Bar 'back' button
            // - the Application's content height has changed because WindowSoftInputModeAdjust was set to Resize
            // - the height has increased (in other words, the last layout was with the keyboard open, and now it's closed)
            var newHeight = Element.Height;

            if (_previousHeight > 0 && newHeight > _previousHeight)
            {
                var nav = Element.Navigation;

                // This update check will fire for all the pages on the stack, but we only need to request a layout for the top one
                if (nav?.NavigationStack != null && nav.NavigationStack.Count > 0 && Element == nav.NavigationStack[nav.NavigationStack.Count - 1])
                {
                    // The Forms layout stuff is already correct, we just need to force Android to catch up
                    RequestLayout();
                }
            }

            // Cache the height for next time
            _previousHeight = newHeight;
        }
    }
}