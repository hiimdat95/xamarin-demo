using KamooniHost.iOS.Renderers;
using System.ComponentModel;

using Xamarin.Forms;
using Xamarin.Forms.Controls;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(RoundedSearchBar), typeof(RoundedSearchBarRenderer))]

namespace KamooniHost.iOS.Renderers
{
    public class RoundedSearchBarRenderer : SearchBarRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<SearchBar> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement == null && Element != null)
            {
            }

            if (Control != null)
            {
                Control.ShowsCancelButton = false;
                Control.ShowsSearchResultsButton = false;
                Control.EnablesReturnKeyAutomatically = false;

                Control.SearchButtonClicked += (sender, arg) =>
                {
                    Element?.SearchCommand?.Execute(Element?.Text);
                };
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == "Text")
            {
                Control.ShowsCancelButton = false;
            }
            else if (e.PropertyName == RoundedSearchBar.BarTintColorProperty.PropertyName)
            {
                if (sender is RoundedSearchBar searchBar)
                {
                    Control.BarTintColor = searchBar.BarTintColor.ToUIColor();
                }
            }
        }
    }
}