using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

namespace TravellerApp.iOS
{
    public class Application
    {
        // This is the main entry point of the application.
        static void Main(string[] args)
        {
            UIColor colorAccent = UIColor.FromRGB(115, 195, 058);
            UIColor colorBackground = UIColor.FromRGB(252, 200, 076);

            UINavigationBar.Appearance.TintColor = UIColor.White;
            UINavigationBar.Appearance.BarTintColor = colorBackground;
            UINavigationBar.Appearance.TitleTextAttributes = new UIStringAttributes
            {
                ForegroundColor = UIColor.White
            };

            UINavigationBar.Appearance.ShadowImage = new UIImage();

            UIButton.Appearance.SetTitleColor(colorAccent, UIControlState.Normal);
            //UIButton.Appearance.BackgroundColor = colorAccent;

            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
            UIApplication.Main(args, null, "AppDelegate");
        }
    }
}
