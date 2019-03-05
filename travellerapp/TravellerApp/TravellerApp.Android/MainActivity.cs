using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using CarouselView.FormsPlugin.Android;
using PremindSDK.Android;
using PremindSDK.Core;
using System;
using TravellerApp.Droid.DependencyService;
using TravellerApp.Droid.PeachPayment.Activity;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps.Android;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

namespace TravellerApp.Droid
{
    [Activity(Label = "Kamooni Traveller", Icon = "@mipmap/ic_launcher", Theme = "@style/MyTheme.Launch", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    [IntentFilter(new[] { Intent.ActionView }, Categories = new[] { Intent.ActionDefault, Intent.CategoryBrowsable }, DataScheme = "kamooni")]
    public class MainActivity : BasePaymentActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            CometChatService.context = this;

            base.OnCreate(bundle);

            //Popup
            Rg.Plugins.Popup.Popup.Init(this, bundle);

            Window.SetSoftInputMode(SoftInput.AdjustResize);
            AndroidBug5497WorkaroundForXamarinAndroid.assistActivity(this);

            Plugin.CurrentActivity.CrossCurrentActivity.Current.Init(this, bundle);

            //Forms.SetFlags("FastRenderers_Experimental");
            Forms.Init(this, bundle);

            //Register Premind SDK
            Xamarin.Forms.DependencyService.Register<IPremind, Premind>();
            Premind.Init(this);

            // Init GoogleMaps
            Xamarin.FormsGoogleMaps.Init(this, bundle, new PlatformConfig
            {
                BitmapDescriptorFactory = new CachingNativeBitmapDescriptorFactory()
            });

            // Init dialogs
            Acr.UserDialogs.UserDialogs.Init(this);

            // Init image cropper
            Stormlion.ImageCropper.Droid.Platform.Init();

            // Init image circle cropper
            ImageCircle.Forms.Plugin.Droid.ImageCircleRenderer.Init();

            CarouselViewRenderer.Init();

            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(enableFastRenderer: true);

            // Init QR code scanner
            ZXing.Net.Mobile.Forms.Android.Platform.Init();
            ZXing.Mobile.MobileBarcodeScanner.Initialize(Application);

            LoadApplication(new App());

            //Xamarin.Forms.Application.Current.On<Xamarin.Forms.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            ZXing.Net.Mobile.Android.PermissionsHandler.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            Stormlion.ImageCropper.Droid.Platform.OnActivityResult(requestCode, resultCode, data);
        }
    }

    public class AndroidBug5497WorkaroundForXamarinAndroid
    {
        // For more information, see https://code.google.com/p/android/issues/detail?id=5497
        // To use this class, simply invoke assistActivity() on an Activity that already has its content view set.

        // CREDIT TO Joseph Johnson (http://stackoverflow.com/users/341631/joseph-johnson) for publishing the original Android solution on stackoverflow.com

        public static void assistActivity(Activity activity)
        {
            new AndroidBug5497WorkaroundForXamarinAndroid(activity);
        }

        private Android.Views.View mChildOfContent;
        private int usableHeightPrevious;
        private FrameLayout.LayoutParams frameLayoutParams;

        private AndroidBug5497WorkaroundForXamarinAndroid(Activity activity)
        {
            FrameLayout content = (FrameLayout)activity.FindViewById(Android.Resource.Id.Content);
            mChildOfContent = content.GetChildAt(0);
            ViewTreeObserver vto = mChildOfContent.ViewTreeObserver;
            vto.GlobalLayout += (object sender, EventArgs e) =>
            {
                possiblyResizeChildOfContent();
            };
            frameLayoutParams = (FrameLayout.LayoutParams)mChildOfContent.LayoutParameters;
        }

        private void possiblyResizeChildOfContent()
        {
            int usableHeightNow = computeUsableHeight();
            if (usableHeightNow != usableHeightPrevious)
            {
                int usableHeightSansKeyboard = mChildOfContent.RootView.Height;
                int heightDifference = usableHeightSansKeyboard - usableHeightNow;

                frameLayoutParams.Height = usableHeightSansKeyboard - heightDifference;

                mChildOfContent.RequestLayout();
                usableHeightPrevious = usableHeightNow;
            }
        }

        private int computeUsableHeight()
        {
            Rect r = new Rect();
            mChildOfContent.GetWindowVisibleDisplayFrame(r);
            return (r.Bottom - r.Top);
        }
    }
}