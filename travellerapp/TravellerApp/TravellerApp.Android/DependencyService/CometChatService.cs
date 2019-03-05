using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Content;
using Cometchat.Inscripts.Com.Cometchatcore.Coresdk;
using CometChatUIBinding.Additions;
using Org.Json;
using TravellerApp.Droid.DependencyService;
using TravellerApp.Interfaces;
using TravellerApp.Interfaces.ComectChatCallback;
using Xamarin.Forms;

[assembly: Dependency(typeof(CometChatService))]

namespace TravellerApp.Droid.DependencyService
{
    public class CometChatService : ICometChatService
    {
        private CometChat cometchat = null;
        public static Context context;
        private bool isInitializeCometChat = false;
        private bool isLogin = false;

        public CometChatService()
        {
        }

        public void initializeCometChat(string SiteUrl, string LicenseKey, string ApiKey, bool isCometOnDemand, CometChatFormCallback callback)
        {
            if (context != null)
            {
                cometchat = CometChat.GetInstance(context);
                cometchat.InitializeCometChat(SiteUrl, LicenseKey, ApiKey, isCometOnDemand, new CometChatCallback((JSONObject obj) =>
                {
                    System.Console.Write("InitializeCometChat success");
                    this.isInitializeCometChat = true;
                    if (callback != null) callback.SuccessCallback((string)obj);
                }, (JSONObject obj) =>
                {
                    System.Console.Write("InitializeCometChat fail");
                    if (callback != null) callback.FailCallback((string)obj);
                }));
            }
        }

        public bool isCometChatLogin()
        {
            return this.isLogin;
        }

        public bool isCometChaInitialize()
        {
            return this.isInitializeCometChat;
        }

        public void logout(CometChatFormCallback callback)
        {
            cometchat.Logout(new CometChatCallback((JSONObject obj) =>
            {
                System.Console.Write("Logout success");
                this.isLogin = false;
                if (callback != null) callback.SuccessCallback((string)obj);
            }, (JSONObject obj) =>
            {
                System.Console.Write("Logout fail");
                if (callback != null) callback.FailCallback((string)obj);
            }));
        }

        public void loginWithUID(string UID, CometChatFormCallback callback)
        {
            cometchat.LoginWithUID(context, UID,
            new CometChatCallback((JSONObject obj) =>
            {
                System.Console.Write("LoginWithUID success");
                this.isLogin = true;
                if (callback != null) callback.SuccessCallback((string)obj);
            }, (JSONObject obj) =>
            {
                System.Console.Write("LoginWithUID fail");
                if (callback != null) callback.FailCallback((string)obj);
            }));
        }

        public void launchCometChatWindow(bool isFullScreen, CometChatLaunchCallbacks launchCallbacks)
        {
            if (context != null)
            {
                cometchat.LaunchCometChat((Android.App.Activity)context, isFullScreen, new LaunchCallbacks(successObj => launchCallbacks.SuccessCallback((string)successObj), fail => launchCallbacks.FailCallback((string)fail), onChatroomInfo => launchCallbacks.ChatroomInfoCallback((string)onChatroomInfo), onError => launchCallbacks.ErrorCallback((string)onError), onLogout => launchCallbacks.LogoutCallback((string)onLogout), onMessageReceive => launchCallbacks.MessageReceiveCallback((string)onMessageReceive), onUserInfo => launchCallbacks.UserInfoCallback((string)onUserInfo), onWindowClose => launchCallbacks.WindowCloseCallback((string)onWindowClose)));
            }
        }
    }

}

