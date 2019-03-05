using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.Runtime;
using Android.Views;
using TravellerApp.Droid.Services;
using TravellerApp.Interfaces;
using Plugin.CurrentActivity;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: Dependency(typeof(PopupService))]

namespace TravellerApp.Droid.Services
{
    public class PopupService : IPopupService
    {
        private Android.Views.View contentNativeView;

        private Dialog contentDialog;

        public void ShowContent(Xamarin.Forms.View content)
        {
            // build the loading page with native base
            content.Parent = Xamarin.Forms.Application.Current.MainPage;

            content.Layout(new Rectangle(0, 0,
                Xamarin.Forms.Application.Current.MainPage.Width,
                Xamarin.Forms.Application.Current.MainPage.Height));

            var renderer = content.GetOrCreateRenderer();

            contentNativeView = renderer.View;

            contentDialog = new Dialog(CrossCurrentActivity.Current.Activity);
            contentDialog.RequestWindowFeature((int)WindowFeatures.NoTitle);
            contentDialog.SetCanceledOnTouchOutside(true);
            contentDialog.SetOnKeyListener(new DialogInterfaceOnKeyListener());
            contentDialog.SetContentView(contentNativeView);
            Window window = contentDialog.Window;
            window.SetLayout(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            window.ClearFlags(WindowManagerFlags.DimBehind);
            window.SetBackgroundDrawable(new ColorDrawable(Android.Graphics.Color.Transparent));

            // Show Popup
            contentDialog.Show();
        }

        private class DialogInterfaceOnKeyListener : Java.Lang.Object, IDialogInterfaceOnKeyListener
        {
            public bool OnKey(IDialogInterface dialog, [GeneratedEnum] Keycode keyCode, KeyEvent e)
            {
                return keyCode == Keycode.Back;
            }
        }

        public void HideContent()
        {
            // Hide Popup
            contentDialog?.Hide();
        }
    }

    internal static class PlatformExtension
    {
        public static IVisualElementRenderer GetOrCreateRenderer(this VisualElement bindable)
        {
            var renderer = Platform.GetRenderer(bindable);
            if (renderer == null)
            {
                renderer = Platform.CreateRendererWithContext(bindable, CrossCurrentActivity.Current.Activity);
                Platform.SetRenderer(bindable, renderer);
            }
            return renderer;
        }
    }
}