using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace KamooniHost.Droid
{
    [Activity(Label = "Kamooni Host", Icon = "@mipmap/ic_launcher", Theme = "@style/MyTheme.Launch", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            Window.SetSoftInputMode(Android.Views.SoftInput.AdjustResize);
            AndroidBug5497WorkaroundForXamarinAndroid.assistActivity(this);

            Plugin.CurrentActivity.CrossCurrentActivity.Current.Init(this, bundle);

            Forms.SetFlags("FastRenderers_Experimental");
            Forms.Init(this, bundle);

            Calendar.Plugin.Platforms.Android.Calendar.Init();
            CarouselView.FormsPlugin.Android.CarouselViewRenderer.Init();

            // Init Dialogs
            Acr.UserDialogs.UserDialogs.Init(this);

            // Init Barcode Scanner
            ZXing.Net.Mobile.Forms.Android.Platform.Init();
            ZXing.Mobile.MobileBarcodeScanner.Initialize(Application);

            // Init Image Plugins
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(enableFastRenderer: true);
            ImageCircle.Forms.Plugin.Droid.ImageCircleRenderer.Init();
            Stormlion.ImageCropper.Droid.Platform.Init();

            // Init GoogleMaps
            Xamarin.FormsGoogleMaps.Init(this, bundle, new Xamarin.Forms.GoogleMaps.Android.PlatformConfig
            {
                BitmapDescriptorFactory = new CachingNativeBitmapDescriptorFactory()
            });

            LoadApplication(new App());

            //Xamarin.Forms.Application.Current.On<Xamarin.Forms.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            try
            {
                Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

                ZXing.Net.Mobile.Android.PermissionsHandler.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            }
            catch { }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            Stormlion.ImageCropper.Droid.Platform.OnActivityResult(requestCode, resultCode, data);
        }

        public class AndroidBug5497WorkaroundForXamarinAndroid
        {
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
                vto.GlobalLayout += (object sender, EventArgs e) => {
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
}