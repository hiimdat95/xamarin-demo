using System.Linq;
using TravellerApp.Interfaces;
using TravellerApp.iOS;
using Xamarin.Auth;
using Xamarin.Forms;
using PremindSDK;
using System;

[assembly: Dependency(typeof(AuthService))]

namespace TravellerApp.iOS
{
    internal class AuthService : IAuthService
    {
        private const string accountStorePassword = "travellerAccountStorePass";

        public string UserName
        {
            get
            {
                var accountStore = AccountStore.Create();

                var account = accountStore.FindAccountsForService(App.AppName).FirstOrDefault();
                if (account != null)
                {
                    return account.Username;
                }
                return null;
            }
        }

        public string Password
        {
            get
            {
                var accountStore = AccountStore.Create();

                var account = accountStore.FindAccountsForService(App.AppName).FirstOrDefault();
                if (account != null)
                {
                    return account.Properties["Password"];
                }
                return null;
            }
        }

        public void SaveCredentials(string UserName, string Password)
        {
            if (!string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(Password))
            {
                ClearAllAccounts();

                Account account = new Account
                {
                    Username = UserName
                };
                account.Properties.Add("Password", Password);

                var accountStore = AccountStore.Create();

                accountStore.Save(account, App.AppName);
                
            }
        }

        public void ClearAllAccounts()
        {
            var accountStore = AccountStore.Create();
            var accounts = accountStore.FindAccountsForService(App.AppName);
            foreach (var account in accounts)
            {
                accountStore.Delete(account, App.AppName);
            }
            // remove scanning 
          //Premind.Shared.Stop(); //vai lon quen stop no mat o day roi 
        }
        public void onLoginSuccess(string token){
            
            PrimindSDKWorking(token);
        }

        private async void PrimindSDKWorking(string token){

          
            var ok = await Premind.Shared.Authenticate(token);
            var alert = new ToastAlert();
            //alert.ShortAlert(ok.ToString());
            //start scaning 
            var startScaning = Premind.Shared.Start(
                  (device) =>
                  {
                      
                      //alert.ShortAlert(device.UUID);
                    //show code 
                  },
                  (device) =>
                  {
                     
                      //alert.ShortAlert(device.UUID);
                  });
        }
    }
}