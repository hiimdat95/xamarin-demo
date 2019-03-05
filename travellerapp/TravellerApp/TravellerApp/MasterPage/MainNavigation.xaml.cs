using System;
using System.Collections;
using Acr.UserDialogs;
using Realms;
using TravellerApp.Constants;
using TravellerApp.Interfaces;
using TravellerApp.Interfaces.ComectChatCallback;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TravellerApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainNavigation : MasterDetailPage
    {
        private int lastSelect = 0;
        public MainNavigation()
        {
            InitializeComponent();

            MasterPage.ListView.ItemSelected += ListView_ItemSelected;

        //    MasterPage.ListView.SelectedItem = (MasterPage.ListView.ItemsSource as IList)?[0];
        }



        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (!(e.SelectedItem is MainNavigationMenuItem selectedItem))
                return;

            selectedItem.TextColor = (Color)Application.Current.Resources["Color_Accent"];

            // Work around for nav drawer hide lag [Android]
            /*var root = Detail.Navigation.NavigationStack[0];
            Detail.Navigation.InsertPageBefore(page, root);
            await Detail.Navigation.PopToRootAsync(false);*/

            MasterPage.ListView.SelectedItem = null;

            var i = (MasterPage.ListView.ItemsSource as IList).IndexOf(e.SelectedItem as MainNavigationMenuItem);
            if (i == 7)
            {
                MasterPage.ListView.SelectedItem = (MasterPage.ListView.ItemsSource as IList)?[this.lastSelect];
                //UserDialogs.Instance.Loading(title: "Open chat ...").Show();
                //if (DependencyService.Get<ICometChatService>().isCometChaInitialize())
                //{
                //    if (DependencyService.Get<ICometChatService>().isCometChatLogin())
                //    {
                //        DependencyService.Get<ICometChatService>().launchCometChatWindow(true, new LaunchCallbackImplementation(successObj => OnSuccessCall(successObj), fail => OnFailCall(fail), onChatroomInfo => OnChatroomInfo(onChatroomInfo), onError => OnError(onError), onLogout => OnLogout(onLogout), onMessageReceive => OnMessageReceive(onMessageReceive), onUserInfo => OnUserInfo(onUserInfo), onWindowClose => OnWindowClose(onWindowClose)));
                //    }
                //    else
                //    {
                //        TravellerApp.Models.User User = Realm.GetInstance().Find<TravellerApp.Models.User>(DBLocalID.USER);
                //        // Login comet chat.
                //        DependencyService.Get<ICometChatService>().loginWithUID(User.traveller_token, new Callbacks(success => loginCometChatSuccess(success), fail => loginCometChatFail(fail)));
                //    }
                //}
                //else
                //{
                //    // Init comet chat.
                //    DependencyService.Get<ICometChatService>().initializeCometChat(CometChatConstants.siteurl, CometChatConstants.licenseKey, CometChatConstants.apiKey, CometChatConstants.isCometOnDemand, new Callbacks(success => initCometChatSuccess(success), fail => initCometChatFail(fail)));
                //}

            }
            else
            {
                //this.lastSelect = i;
                //Page page;

                //if (selectedItem.Params != null)
                //    page = (Page)Activator.CreateInstance(selectedItem.TargetType, selectedItem.Params);
                //else
                //    page = (Page)Activator.CreateInstance(selectedItem.TargetType);

                //page.Title = selectedItem.Title;

                //Detail = new NavigationPage(page);
                //if (i != 0 && i != 1 && i != 4)
                //{
                //    Detail = new NavigationPage(new MainTabbedPage());
                //}
                //else
                //{
                //    Detail = new NavigationPage(page);
                //}
                App.MainTabbedPage.NavigateToPage(selectedItem.TargetType);

            }

            IsPresented = false;

        }

        private void initCometChatSuccess(string success)
        {
            if (success != null)
            {
                System.Console.WriteLine("initCometChatSuccess" + success.ToString());
                TravellerApp.Models.User User = Realm.GetInstance().Find<TravellerApp.Models.User>(DBLocalID.USER);
                // Login comet chat.
                DependencyService.Get<ICometChatService>().loginWithUID(User.traveller_token, new Callbacks(successlogin => loginCometChatSuccess(successlogin), fail => loginCometChatFail(fail)));

            }
        }

        private void initCometChatFail(string fail)
        {
            UserDialogs.Instance.Loading().Hide();
            MasterPage.ListView.SelectedItem = (MasterPage.ListView.ItemsSource as IList)?[this.lastSelect];
            if (fail != null)
            {
                System.Console.WriteLine("initCometChatFail" + fail.ToString());
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
            UserDialogs.Instance.Loading().Hide();
            MasterPage.ListView.SelectedItem = (MasterPage.ListView.ItemsSource as IList)?[this.lastSelect];
            if (fail != null)
            {
                System.Console.WriteLine("loginCometChatSuccess" + fail.ToString());
            }
        }

        void OnLaunchCometChat(object sender, EventArgs args)
        {

            var cometchat = DependencyService.Get<ICometChatService>();
            cometchat.launchCometChatWindow(true, new LaunchCallbackImplementation(successObj => OnSuccessCall(successObj), fail => OnFailCall(fail), onChatroomInfo => OnChatroomInfo(onChatroomInfo), onError => OnError(onError), onLogout => OnLogout(onLogout), onMessageReceive => OnMessageReceive(onMessageReceive), onUserInfo => OnUserInfo(onUserInfo), onWindowClose => OnWindowClose(onWindowClose)));
        }

        private void OnSuccessCall(String successObj)
        {
            UserDialogs.Instance.Loading().Hide();
            if (successObj != null)
            {
                MasterPage.ListView.SelectedItem = (MasterPage.ListView.ItemsSource as IList)?[this.lastSelect];
                System.Console.WriteLine("loginSuccess " + successObj.ToString());
            }
        }

        private void OnFailCall(String fail)
        {
            UserDialogs.Instance.Loading().Hide();
            MasterPage.ListView.SelectedItem = (MasterPage.ListView.ItemsSource as IList)?[this.lastSelect];
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

    }
}