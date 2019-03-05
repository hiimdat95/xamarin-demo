using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreGraphics;
using Foundation;
using KamooniHost.IDependencyServices;
using KamooniHost.iOS.DependencyService;
using ObjCRuntime;
using TravellerApp.iOS;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(ShareOnInstagramService))]
namespace KamooniHost.iOS.DependencyService
{
    class ShareOnInstagramService : IShareOnInstagramAPI
    {
        TaskCompletionSource<bool> ShareInsTaskCompletion;
        UIDocumentInteractionController documentController;

        public Task<bool> OpenShareOnInstagram(string path, string content)
        {
            ShareInsTaskCompletion = new TaskCompletionSource<bool>();
            // Copy content to clip broad.
            CopyToClipbroad(content);
            // Open share.
            CreateShareInstagram(path);
            return ShareInsTaskCompletion.Task;

        }

        private void CopyToClipbroad(string content)
        {
            var pasteBoard = UIPasteboard.General;
            pasteBoard.String = content;
        }

        private void CreateShareInstagram(string mediaPath)
        {
            NSUrl imageURL = new NSUrl(mediaPath, false);
            NSUrl instagramURL = NSUrl.FromString(@"instagram://app");
            //check for App is install or not
            if (UIApplication.SharedApplication.CanOpenUrl(instagramURL))
            {
                documentController = UIDocumentInteractionController.FromUrl(imageURL);
                documentController.Uti = "com.instagram.exclusivegram";
                UIView presentingView = GetVisibleViewController().View;
                documentController.PresentOpenInMenu(new CGRect(x: 1, y: 1, width: 1, height: 1), presentingView, true);
                ShareInsTaskCompletion.SetResult(true);
            }
            else
            {
                bool isSimulator = Runtime.Arch == Arch.SIMULATOR;
                NSUrl itunesLink;
                if (isSimulator)
                {
                    itunesLink = new NSUrl("https://itunes.apple.com/us/app/instagram/id389801252?mt=8");
                }
                else
                {
                    itunesLink = new NSUrl("itms://itunes.apple.com/us/app/instagram/id389801252?mt=8");
                }
                UIApplication.SharedApplication.OpenUrl(itunesLink, new NSDictionary() { }, null);
            }
        }

        private UIViewController GetVisibleViewController(UIViewController controller = null)
        {
            controller = controller ?? UIApplication.SharedApplication.KeyWindow.RootViewController;

            if (controller.PresentedViewController == null)
                return controller;

            if (controller.PresentedViewController is UINavigationController)
            {
                return ((UINavigationController)controller.PresentedViewController).VisibleViewController;
            }

            if (controller.PresentedViewController is UITabBarController)
            {
                return ((UITabBarController)controller.PresentedViewController).SelectedViewController;
            }

            return GetVisibleViewController(controller.PresentedViewController);
        }

    }
}