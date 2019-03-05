using Android.App;
using Android.Graphics.Drawables;
using Android.Views;
using ImageCircle.Forms.Plugin.Abstractions;
using KamooniHost.Droid.Services;
using KamooniHost.IServices;
using KamooniHost.Views.Popup;
using Plugin.CurrentActivity;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: Dependency(typeof(PremindService))]

namespace KamooniHost.Droid.Services
{
    public class PremindService : IPremindService
    {
        private Android.Views.View _contentNativeView, _iconNativeView;

        private Dialog _contentDialog, _iconDialog;

        private bool _isContentInitialized, _isIconInitialized;

        public void InitContent(Xamarin.Forms.View content)
        {
            // build the loading page with native base
            content.Parent = Xamarin.Forms.Application.Current.MainPage;

            content.Layout(new Rectangle(0, 0,
                Xamarin.Forms.Application.Current.MainPage.Width,
                Xamarin.Forms.Application.Current.MainPage.Height));

            var renderer = content.GetOrCreateRenderer();

            _contentNativeView = renderer.View;

            _contentDialog = new Dialog(CrossCurrentActivity.Current.Activity);
            _contentDialog.RequestWindowFeature((int)WindowFeatures.NoTitle);
            _contentDialog.SetCancelable(true);
            _contentDialog.SetCanceledOnTouchOutside(true);
            _contentDialog.SetContentView(_contentNativeView);
            Window window = _contentDialog.Window;
            window.SetLayout(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            window.ClearFlags(WindowManagerFlags.DimBehind);
            window.SetBackgroundDrawable(new ColorDrawable(Android.Graphics.Color.Transparent));

            _isContentInitialized = true;
        }

        public void ShowContent()
        {
            // check if the user has set the page or not
            if (!_isContentInitialized)
                InitContent(new PremindNotification()); // set the default page

            // showing the native loading page
            _contentDialog.Show();
        }

        public void HideContent()
        {
            // Hide the page
            _contentDialog.Hide();
        }

        public void InitIcon(CircleImage icon)
        {
            // build the loading page with native base
            icon.Parent = Xamarin.Forms.Application.Current.MainPage;

            icon.Layout(new Rectangle(Xamarin.Forms.Application.Current.MainPage.Width - icon.WidthRequest, Xamarin.Forms.Application.Current.MainPage.Height - icon.HeightRequest,
                icon.WidthRequest,
                icon.HeightRequest));

            var renderer = icon.GetOrCreateRenderer();

            _iconNativeView = renderer.View;

            _iconDialog = new Dialog(CrossCurrentActivity.Current.Activity);
            _iconDialog.RequestWindowFeature((int)WindowFeatures.NoTitle);
            _iconDialog.SetCancelable(true);
            _iconDialog.SetCanceledOnTouchOutside(true);
            _iconDialog.SetContentView(_iconNativeView);
            Window window = _iconDialog.Window;
            window.SetLayout(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
            window.SetGravity(GravityFlags.Bottom);
            window.ClearFlags(WindowManagerFlags.DimBehind);
            window.SetBackgroundDrawable(new ColorDrawable(Android.Graphics.Color.Transparent));

            _isIconInitialized = true;
        }

        public void ShowIcon()
        {
            if (!_isIconInitialized)
                return;

            _iconDialog.Show();
        }

        public void HideIcon()
        {
            _iconDialog.Hide();
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