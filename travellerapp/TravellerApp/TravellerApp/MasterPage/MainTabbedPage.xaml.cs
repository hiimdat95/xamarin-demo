using Acr.UserDialogs;
using BottomBar.XamarinForms;
using Realms;
using System;
using TravellerApp.Constants;
using TravellerApp.Interfaces;
using TravellerApp.Interfaces.ComectChatCallback;
using TravellerApp.Views;
using Xamarin.Forms;

namespace TravellerApp
{
    public partial class MainTabbedPage : BottomBarPage
    {
        public MainTabbedPage()
        {
            App.MainTabbedPage = this;

            BarBackgroundColor = Color.White;
            TabNormalColor = Color.Gray;
            TabActiveColor = Color.Orange;
            BarTextColor = Color.FromHex("#666666");
            FixedMode = true;

            var page1 = new NavigationPage(new RecentPostsPage()
            {
                Icon = new FileImageSource()
                {
                    File = "home1.png"
                },
                Title = "Home"
            })
            {
                BarTextColor = Color.White,
                Icon = new FileImageSource()
                {
                    File = "home1.png"
                },
                Title = "Home"
            };

            var page2 = new NavigationPage(new CurrentlyAtPage())
            {
                Icon = new FileImageSource()
                {
                    File = "wallet.png"
                },
                Title = "Wallet"
            };

            var page3 = new NavigationPage(new ExplorePage())
            {
                Icon = new FileImageSource()
                {
                    File = "explore.png"
                },
                Title = "Explore"
            };

            var page4 = new NavigationPage(new MessagePage())
            {
                Icon = new FileImageSource()
                {
                    File = "folder.png"
                },
                Title = "Messages"
            };

            var page5 = new NavigationPage(new MyBookingsPage())
            {
                Icon = new FileImageSource()
                {
                    File = "profile.png"
                },
                Title = "Feed"
            };

            var page6 = new MyProfilePage()
            {
                Icon = new FileImageSource()
                {
                    File = "feed.png"
                },
                Title = "Profile"
            };

            Children.Add(page1);
            Children.Add(page2);
            Children.Add(page3);
            Children.Add(page4);
            Children.Add(page5);
            Children.Add(page6);


        }

        protected override void OnCurrentPageChanged()
        {
            base.OnCurrentPageChanged();
            if (typeof(MessagePage).Equals(CurrentPage.GetType()))
            {
                if (DependencyService.Get<ICometChatService>().isCometChaInitialize())
                {
                    if (DependencyService.Get<ICometChatService>().isCometChatLogin())
                    {
                        DependencyService.Get<ICometChatService>().launchCometChatWindow(true, new LaunchCallbackImplementation(successObj => OnSuccessCall(successObj), fail => OnFailCall(fail), onChatroomInfo => OnChatroomInfo(onChatroomInfo), onError => OnError(onError), onLogout => OnLogout(onLogout), onMessageReceive => OnMessageReceive(onMessageReceive), onUserInfo => OnUserInfo(onUserInfo), onWindowClose => OnWindowClose(onWindowClose)));
                    }
                    else
                    {
                        TravellerApp.Models.User User = Realm.GetInstance().Find<TravellerApp.Models.User>(DBLocalID.USER);
                        // Login comet chat.
                        DependencyService.Get<ICometChatService>().loginWithUID(User.traveller_token, new Callbacks(success => loginCometChatSuccess(success), fail => loginCometChatFail(fail)));
                    }
                }
                else
                {
                    // Init comet chat.
                    DependencyService.Get<ICometChatService>().initializeCometChat(CometChatConstants.siteurl, CometChatConstants.licenseKey, CometChatConstants.apiKey, CometChatConstants.isCometOnDemand, new Callbacks(success => initCometChatSuccess(success), fail => initCometChatFail(fail)));
                }
            }
        }

        public void NavigateToPage(Type page)
        {
            var tabbedPage = this as TabbedPage;

            if (page == typeof(RecentPostsPage))
            {
                tabbedPage.CurrentPage = tabbedPage.Children[0];
            }
            else if (page == typeof(BillingPage))
            {
                tabbedPage.CurrentPage = tabbedPage.Children[1];
            }
            else if (page == typeof(OurStoryPage))
            {
                tabbedPage.CurrentPage = tabbedPage.Children[2];
            }
            else if (page == typeof(MessagePage))
            {
                tabbedPage.CurrentPage = tabbedPage.Children[3];
            }
            else if (page == typeof(MyBookingsPage))
            {
                tabbedPage.CurrentPage = tabbedPage.Children[4];
            }
            else if (page == typeof(MyProfilePage))
            {
                tabbedPage.CurrentPage = tabbedPage.Children[5];
            }
        }


        private void OnLaunchCometChat(object sender, EventArgs args)
        {
            var cometchat = DependencyService.Get<ICometChatService>();
            cometchat.launchCometChatWindow(true, new LaunchCallbackImplementation(successObj => OnSuccessCall(successObj), fail => OnFailCall(fail), onChatroomInfo => OnChatroomInfo(onChatroomInfo), onError => OnError(onError), onLogout => OnLogout(onLogout), onMessageReceive => OnMessageReceive(onMessageReceive), onUserInfo => OnUserInfo(onUserInfo), onWindowClose => OnWindowClose(onWindowClose)));
        }

        private void OnSuccessCall(String successObj)
        {

            if (successObj != null)
            {
                System.Console.WriteLine("loginSuccess " + successObj.ToString());
            }
        }

        private void OnFailCall(String fail)
        {

            if (fail != null)
            {
                System.Console.WriteLine("OnFailCall " + fail.ToString());
            }
        }

        private void OnChatroomInfo(String onChatroomInfo)
        {
            if (onChatroomInfo != null)
            {
                System.Console.WriteLine("OnChatroomInfo " + onChatroomInfo.ToString());
            }
        }

        private void OnError(String onError)
        {
            if (onError != null)
            {
                System.Console.WriteLine("OnError " + onError.ToString());
            }
        }

        private void OnLogout(String onError)
        {
            if (onError != null)
            {
                System.Console.WriteLine("OnLogout " + onError.ToString());
            }
        }

        private void OnMessageReceive(String onMessageReceive)
        {
            if (onMessageReceive != null)
            {
                System.Console.WriteLine("OnMessageReceive " + onMessageReceive.ToString());
            }
        }
        private void loginCometChatSuccess(string success)
        {
            if (success != null)
            {
                System.Console.WriteLine("loginCometChatSuccess" + success.ToString());
                DependencyService.Get<ICometChatService>().launchCometChatWindow(true, new LaunchCallbackImplementation(successObj => OnSuccessCall(successObj), fail => OnFailCall(fail), onChatroomInfo => OnChatroomInfo(onChatroomInfo), onError => OnError(onError), onLogout => OnLogout(onLogout), onMessageReceive => OnMessageReceive(onMessageReceive), onUserInfo => OnUserInfo(onUserInfo), onWindowClose => OnWindowClose(onWindowClose)));
            }
        }
        private void loginCometChatFail(string fail)
        {

            if (fail != null)
            {
                System.Console.WriteLine("loginCometChatSuccess" + fail.ToString());
            }
        }

        private void OnUserInfo(String onUserInfo)
        {
            if (onUserInfo != null)
            {
                System.Console.WriteLine("OnUserInfo " + onUserInfo.ToString());
            }
        }

        private void OnWindowClose(String onWindowClose)
        {
            if (onWindowClose != null)
            {
                System.Console.WriteLine("OnWindowClose " + onWindowClose.ToString());
            }
        }
        private void initCometChatFail(string fail)
        {

            if (fail != null)
            {
                System.Console.WriteLine("initCometChatFail" + fail.ToString());
            }
            UserDialogs.Instance.Loading().Hide();
        }

        private void initCometChatSuccess(string success)
        {
            if (success != null)
            {
                System.Console.WriteLine("initCometChatSuccess" + success.ToString());
                TravellerApp.Models.User User = Realm.GetInstance().Find<TravellerApp.Models.User>(DBLocalID.USER);
                // Login comet chat.
                DependencyService.Get<ICometChatService>().loginWithUID(User.traveller_token, new Callbacks(successlogin => loginCometChatSuccess(successlogin), fail => loginCometChatFail(fail)));
                UserDialogs.Instance.Loading().Hide();
            }
        }
    }
}