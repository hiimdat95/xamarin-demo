using Foundation;
using UIKit;
using UserNotifications;

namespace KamooniHost.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            Xamarin.Forms.Forms.Init();

            Calendar.Plugin.Platforms.iOS.Calendar.Init();
            CarouselView.FormsPlugin.iOS.CarouselViewRenderer.Init();

            // Init Barcode Scanner
            ZXing.Net.Mobile.Forms.iOS.Platform.Init();

            // Init Image Plugins
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init();
            ImageCircle.Forms.Plugin.iOS.ImageCircleRenderer.Init();
            Stormlion.ImageCropper.iOS.Platform.Init();

           // Acr.UserDialogs.UserDialogs.Init();
            // Init GoogleMaps
            Xamarin.FormsGoogleMaps.Init("AIzaSyC-wryBpQqc8ELEpPBOltvsr6J-voZxcfo", new Xamarin.Forms.GoogleMaps.iOS.PlatformConfig
            {
                ImageFactory = new CachingImageFactory()
            });

            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                // Ask the user for permission to get notifications on iOS 10.0+
                UNUserNotificationCenter.Current.RequestAuthorization(
                        UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound,
                        (approved, error) => { });

                // Watch for notifications while app is active
                UNUserNotificationCenter.Current.Delegate = new UserNotificationCenterDelegate();
            }
            else if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                // Ask the user for permission to get notifications on iOS 8.0+
                var settings = UIUserNotificationSettings.GetSettingsForTypes(
                        UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound,
                        new NSSet());

                UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
            }

            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }
    }
}