using Foundation;
using UIKit;
using System;
using Xamarin.Forms.GoogleMaps.iOS;
using PremindSDK;
using CarouselView.FormsPlugin.iOS;
using Plugin.FirebasePushNotification;

namespace TravellerApp.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
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
            //popup
            Rg.Plugins.Popup.Popup.Init();

            Xamarin.Forms.Forms.Init();

            FirebasePushNotificationManager.Initialize(options, true);

            UINavigationBar.Appearance.ShadowImage = new UIImage();

            // Init image cropper
            Stormlion.ImageCropper.iOS.Platform.Init();

            //// Init image circle cropper
            ImageCircle.Forms.Plugin.iOS.ImageCircleRenderer.Init();

            FFImageLoading.Forms.Platform.CachedImageRenderer.Init();

            CarouselViewRenderer.Init();

            // Init QR code scanner
            ZXing.Net.Mobile.Forms.iOS.Platform.Init();

            Xamarin.FormsGoogleMaps.Init("AIzaSyC-wryBpQqc8ELEpPBOltvsr6J-voZxcfo", new PlatformConfig
            {
                ImageFactory = new CachingImageFactory()
            });

            LoadApplication(new App());

            //start premind sdk 
            Premind.Shared.FinishedLaunching(app, options);

            return base.FinishedLaunching(app, options);
        }
       
        [Export("application:performFetchWithCompletionHandler:")]
        public override void PerformFetch(UIApplication application, Action<UIBackgroundFetchResult> completionHandler)
        {
            Premind.Shared.PerformFetch(application, completionHandler);
        }
        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            FirebasePushNotificationManager.DidRegisterRemoteNotifications(deviceToken);

        }
        public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
        {
            FirebasePushNotificationManager.RemoteNotificationRegistrationFailed(error);

        }
        public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        {
            // If you are receiving a notification message while your app is in the background,
            // this callback will not be fired 'till the user taps on the notification launching the application.

            // If you disable method swizzling, you'll need to call this method. 
            // This lets FCM track message delivery and analytics, which is performed
            // automatically with method swizzling enabled.
            FirebasePushNotificationManager.DidReceiveMessage(userInfo);
            // Do your magic to handle the notification data
            Console.WriteLine("DidReceiveRemoteNotification : " + userInfo);
        }

    }
}