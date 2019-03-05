using System;
using System.Linq;
using CometChatBinding;
using CometChatUI;
using Firebase.Auth;
using Firebase.Database;
using Foundation;
using Plugin.FirebasePushNotification;
using TravellerApp.Interfaces;
using TravellerApp.Interfaces.ComectChatCallback;
using TravellerApp.iOS.DependencyService;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(CometChatService))]
namespace TravellerApp.iOS.DependencyService
{
    public class CometChatService : ICometChatService
    {
        CometChat cometchat = null;
        readyUIFIle readyUI = null;
        private bool isInitializeCometChat = false;
        private bool isLogin = false;

        public CometChatService()
        {
        }


        public void initializeCometChat(string SiteUrl, string LicenseKey, string ApiKey, bool isCometOnDemand, CometChatFormCallback callback)
        {
            cometchat = new CometChat();
            readyUI = new readyUIFIle();
            User user = Auth.DefaultInstance.CurrentUser;
            Database database = Database.DefaultInstance;
            CrossFirebasePushNotification.Current.OnNotificationReceived += (s, p) =>
            {
            };
            cometchat.InitializeCometChat(SiteUrl, LicenseKey, ApiKey, isCometOnDemand,
                   (dict) => { if (dict != null && callback != null) { callback.SuccessCallback(dict.ToString()); } },
                   (err) => { if (err != null && callback != null) { callback.SuccessCallback(err.ToString()); } });

        }

        public void logout(CometChatFormCallback callback)
        {
            cometchat.Logout(
                 (dict) => { if (dict != null && callback != null) { callback.SuccessCallback(dict.ToString()); this.isLogin = false; } },
                 (err) => { if (err != null && callback != null) { callback.SuccessCallback(err.ToString()); } });
        }

        public bool isCometChatLogin()
        {
            return this.isLogin;
        }

        public bool isCometChaInitialize()
        {
            return this.isInitializeCometChat;
        }

        public void loginWithUID(string UID, CometChatFormCallback callback)
        {
            cometchat.LoginWithUID("OIKUBJ2CH5180ISTO6Q",
                   (dict) => { if (dict != null && callback != null) { callback.SuccessCallback(dict.ToString()); this.isLogin = true; } },
                   (err) => { if (err != null && callback != null) { callback.SuccessCallback(err.ToString()); } });
        }

        public void launchCometChatWindow(bool isFullScreen, CometChatLaunchCallbacks launchCallbacks)
        {
            //get current UIViewController
            var window = UIApplication.SharedApplication.KeyWindow;
            var currentView = window.RootViewController;
            while (currentView.PresentedViewController != null)
                currentView = currentView.PresentedViewController;

            var navController = currentView as UINavigationController;
            if (navController != null)
                currentView = navController.ViewControllers.Last();

            readyUI.LaunchCometChat(true, currentView,
                                    (dict) => {
                                        String push_channel = (dict["push_channel"] as NSString);
                                        CrossFirebasePushNotification.Current.Subscribe(push_channel);
                                        launchCallbacks.UserInfoCallback(dict.ToString());
                                    },
                                    (dict1) => { launchCallbacks.ChatroomInfoCallback(dict1.ToString()); },
                                    (dict2) => { launchCallbacks.MessageReceiveCallback(dict2.ToString()); },
                                    (dict3) => { launchCallbacks.SuccessCallback(dict3.ToString()); },
                                    (err) => { launchCallbacks.FailCallback(err.ToString()); },
                                    (dict4) => { launchCallbacks.LogoutCallback(dict4.ToString()); }
                             );
        }
        public void launchCometChatWithId(bool isFullScreen, string userOrGroupId, bool isGroup, bool setBackButton, CometChatLaunchCallbacks launchCallbacks)
        {
            var window = UIApplication.SharedApplication.KeyWindow;
            var currentView = window.RootViewController;
            while (currentView.PresentedViewController != null)
                currentView = currentView.PresentedViewController;

            var navController = currentView as UINavigationController;
            if (navController != null)
                currentView = navController.ViewControllers.Last();

            readyUI.LaunchCometChat(userOrGroupId, isGroup, isFullScreen, currentView, setBackButton,
                                    (dict) => {
                                        Console.WriteLine("OnUserInfo CometChatImplementation" + dict.ToString());
                                        String push_channel = (dict["push_channel"] as NSString);
                                        Console.WriteLine("push_channel " + push_channel);
                                        CrossFirebasePushNotification.Current.Subscribe(push_channel);
                                        launchCallbacks.UserInfoCallback(dict.ToString());
                                    },
                                    (dict1) => { launchCallbacks.ChatroomInfoCallback(dict1.ToString()); },
                                    (dict2) => { launchCallbacks.MessageReceiveCallback(dict2.ToString()); },
                                    (dict3) => { launchCallbacks.SuccessCallback(dict3.ToString()); },
                                    (err) => { launchCallbacks.FailCallback(err.ToString()); },
                                    (dict4) => { launchCallbacks.LogoutCallback(dict4.ToString()); }
                             );
        }

       
    }
}
