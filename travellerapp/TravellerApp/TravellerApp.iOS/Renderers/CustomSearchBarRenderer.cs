using System;
using System.ComponentModel;
using TravellerApp.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Controls;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomSearchBar), typeof(CustomSearchBarRenderer))]
namespace TravellerApp.iOS.Renderers
{
    public class CustomSearchBarRenderer : SearchBarRenderer
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
                Control.SearchBarStyle = UISearchBarStyle.Minimal;
                Control.BarTintColor = UIColor.Clear;
                //set empty image
                Control.SetBackgroundImage(new UIImage(), UIBarPosition.TopAttached, UIBarMetrics.Default);

                Control.SearchButtonClicked += (sender, arg) =>
                {
                    Element?.SearchCommand?.Execute(Element?.Text);
                };

                UITextField txSearchField = (UITextField)Control.ValueForKey(new Foundation.NSString("searchField"));
                txSearchField.BackgroundColor = UIColor.White;
                txSearchField.BorderStyle = UITextBorderStyle.None;
                txSearchField.Layer.BorderWidth =  0.5f;
                txSearchField.Layer.CornerRadius = 10.0f;

                //txSearchField.Layer.ShadowOpacity = 50;
                txSearchField.Layer.ShadowColor = UIColor.DarkGray.CGColor;
                txSearchField.Layer.BorderColor = UIColor.LightGray.CGColor;
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == "Text")
            {
                Control.ShowsCancelButton = false;
            }
            else if (e.PropertyName == CustomSearchBar.BarTintColorProperty.PropertyName)
            {
                if (sender is CustomSearchBar searchBar)
                {
                    Control.BarTintColor = searchBar.BarTintColor.ToUIColor();
                }
            }
        }

    }
}
