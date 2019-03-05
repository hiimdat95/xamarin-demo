using System;
using System.Collections.Generic;
using System.Text;
using TravellerApp.Interfaces.ComectChatCallback;

namespace TravellerApp.Interfaces
{
    public interface ICometChatService
    {
        void initializeCometChat(string SiteUrl, string LicenseKey, string ApiKey, bool isCometOnDemand, CometChatFormCallback callback);
        void logout(CometChatFormCallback callback);
        bool isCometChatLogin();
        bool isCometChaInitialize();
        void loginWithUID(string UID, CometChatFormCallback callback);
        void launchCometChatWindow(bool isFullScreen, CometChatLaunchCallbacks launchCallbacks);
    }
}
