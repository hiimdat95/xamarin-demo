using PremindSDK.Core;
using System.Linq;
using TravellerApp.Droid;
using TravellerApp.Interfaces;
using Xamarin.Auth;
using Xamarin.Forms;

[assembly: Dependency(typeof(AuthService))]

namespace TravellerApp.Droid
{
    internal class AuthService : IAuthService
    {
        private const string accountStorePassword = "travellerAccountStorePass";

        public string UserName
        {
            get
            {
                var accountStore = AccountStore.Create(Android.App.Application.Context, accountStorePassword);

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
                var accountStore = AccountStore.Create(Android.App.Application.Context, accountStorePassword);

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
                    Username = UserName.Trim()
                };
                account.Properties.Add("Password", Password);

                var accountStore = AccountStore.Create(Android.App.Application.Context, accountStorePassword);
                accountStore.Save(account, App.AppName);
            }
        }

        public void ClearAllAccounts()
        {
            var accountStore = AccountStore.Create(Android.App.Application.Context, accountStorePassword);
            var accounts = accountStore.FindAccountsForService(App.AppName);
            foreach (var account in accounts)
            {
                accountStore.Delete(account, App.AppName);
            }
        }

        public void onLoginSuccess(string token)
        {
            PrimindSDKWorking(token);
        }

        private void PrimindSDKWorking(string token)
        {
            var premindSdk = Xamarin.Forms.DependencyService.Get<IPremind>();
            var result = premindSdk.Authenticate(token);
            if (result)
            {
                //////////////////////////
            }
        }
    }
}