using TravellerApp.Interfaces;
using TravellerApp.iOS.Services;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: Dependency(typeof(PopupService))]

namespace TravellerApp.iOS.Services
{
    public class PopupService : IPopupService
    {
        private UIView contentNativeView;

        public void ShowContent(View content)
        {
            // build the loading page with native base
            content.Parent = Xamarin.Forms.Application.Current.MainPage;

            content.Layout(new Rectangle(0, 0,
                Xamarin.Forms.Application.Current.MainPage.Width,
                Xamarin.Forms.Application.Current.MainPage.Height));
            
            // get native view
            contentNativeView = content.GetOrCreateRenderer().NativeView;

            // showing the native loading page
            UIApplication.SharedApplication.KeyWindow.AddSubview(contentNativeView);
        }

        public void HideContent()
        {
            // Hide the page
            contentNativeView?.RemoveFromSuperview();
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