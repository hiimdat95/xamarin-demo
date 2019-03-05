using ImageCircle.Forms.Plugin.Abstractions;
using KamooniHost.iOS.Services;
using KamooniHost.IServices;
using KamooniHost.Views.Popup;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: Dependency(typeof(PremindService))]

namespace KamooniHost.iOS.Services
{
    public class PremindService : IPremindService
    {
        private UIView _contentNativeView, _iconNativeView;

        private bool _isContentInitialized, _isIconInitialized;

        public void InitContent(View content)
        {
            // build the loading page with native base
            content.Parent = Xamarin.Forms.Application.Current.MainPage;

            content.Layout(new Rectangle(0, 0,
                Xamarin.Forms.Application.Current.MainPage.Width,
                Xamarin.Forms.Application.Current.MainPage.Height));

            var renderer = content.GetOrCreateRenderer();

            _contentNativeView = renderer.NativeView;

            _isContentInitialized = true;
        }

        public void ShowContent()
        {
            // check if the user has set the page or not
            if (!_isContentInitialized)
                InitContent(new PremindNotification()); // set the default page

            // showing the native loading page
            UIApplication.SharedApplication.KeyWindow.AddSubview(_contentNativeView);
        }

        public void HideContent()
        {
            // Hide the page
            _contentNativeView.RemoveFromSuperview();
        }

        public void InitIcon(CircleImage icon)
        {
            // build the loading page with native base
            icon.Parent = Xamarin.Forms.Application.Current.MainPage;

            icon.Layout(new Rectangle(Xamarin.Forms.Application.Current.MainPage.Width - icon.WidthRequest, Xamarin.Forms.Application.Current.MainPage.Height - icon.HeightRequest,
                icon.WidthRequest,
                icon.HeightRequest));

            var renderer = icon.GetOrCreateRenderer();

            _iconNativeView = renderer.NativeView;

            _isIconInitialized = true;
        }

        public void ShowIcon()
        {
            if (!_isIconInitialized)
                return;

            UIApplication.SharedApplication.KeyWindow.AddSubview(_iconNativeView);
        }

        public void HideIcon()
        {
            _iconNativeView.RemoveFromSuperview();
        }
    }

    internal static class PlatformExtension
    {
        public static IVisualElementRenderer GetOrCreateRenderer(this VisualElement bindable)
        {
            var renderer = Platform.GetRenderer(bindable);
            if (renderer == null)
            {
                renderer = Platform.CreateRenderer(bindable);
                Platform.SetRenderer(bindable, renderer);
            }
            return renderer;
        }
    }
}