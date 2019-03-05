using KamooniHost.iOS.Renderers;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(SearchBar), typeof(DefaultSearchBarRenderer))]

namespace KamooniHost.iOS.Renderers
{
    public class DefaultSearchBarRenderer : SearchBarRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<SearchBar> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement == null && Element != null)
            {
            }

            if (Control != null)
            {
                Control.SearchButtonClicked += (sender, arg) =>
                {
                    Element?.SearchCommand?.Execute(Element?.Text);
                };
            }
        }
    }
}