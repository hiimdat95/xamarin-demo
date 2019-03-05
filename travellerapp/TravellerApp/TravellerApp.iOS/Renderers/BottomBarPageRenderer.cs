using System;
using BottomBar.XamarinForms;
using TravellerApp.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(BottomBarPage), typeof(BottomBarPageRenderer))]
namespace TravellerApp.iOS.Renderers
{
    public class BottomBarPageRenderer : TabbedRenderer
    {
        UIColor normal;
        UIColor active;

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);
            if (e.NewElement != null)
            {
                if (e.NewElement is BottomBarPage)
                {
                    //// Set Text Font for unselected tab states
                    var page = e.NewElement as BottomBarPage;
                    normal = page.TabNormalColor.ToUIColor();
                    active = page.TabActiveColor.ToUIColor();

                    UITextAttributes normalTextAttributes = new UITextAttributes();
                    normalTextAttributes.TextColor = normal;
                    UITabBarItem.Appearance.SetTitleTextAttributes(normalTextAttributes, UIControlState.Normal);

                    UITextAttributes activeTextAttributes = new UITextAttributes();
                    activeTextAttributes.TextColor = active;
                    UITabBarItem.Appearance.SetTitleTextAttributes(activeTextAttributes, UIControlState.Selected);

                    UITabBar.Appearance.SelectedImageTintColor = active;
                }
                try
                {
                    var tabbarController = (UITabBarController)this.ViewController;
                    if (null != tabbarController)
                    {
                        tabbarController.ViewControllerSelected += TabbarController_ViewControllerSelected; ;
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }

        }

        private void TabbarController_ViewControllerSelected(object sender, UITabBarSelectionEventArgs e)
        {
            if (this.Element != null)
                (this.Element as BottomBarPage).RaiseCurrentPageChanged();
        }
    }
}
