using Acr.UserDialogs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TravellerApp.Constants;
using TravellerApp.Interfaces;
using TravellerApp.Models;
using TravellerApp.Models.Response;
using TravellerApp.Notifications;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TravellerApp.Views.PopupPage
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HostDetailPopup : Rg.Plugins.Popup.Pages.PopupPage
    {
        public event OnMapNavigateHandler NavigateMap;

        public delegate void OnMapNavigateHandler(object sender, object args);
        public HostDetailPopup(object bindingContext)
        {
            InitializeComponent();
            this.BindingContext = bindingContext;
        }

        private async void ImageButton_Clicked(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync();
        }

        private async void Bookmark_Clicked(object sender, EventArgs e)
        {
            try
            {
                var client = new HttpClient()
                {
                    Timeout = TimeSpan.FromSeconds(30)
                };

                var AuthService = DependencyService.Get<IAuthService>();
                var token = new JProperty("token", (this.BindingContext as ListInfo).usr_token);
                JObject jsonAuth = new JObject
                {
                    { "login",  AuthService.UserName},
                    { "password",  AuthService.Password},
                    { "db",  ServerAuth.DB}
                };

                JObject jsonDataObject = new JObject
                {
                    { "auth",  jsonAuth},
                    token
                };

                JObject jsonData = new JObject
                {
                    { "params", jsonDataObject }
                };

                var data = jsonData.ToString();
                var content = new StringContent(data, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(ApiUri.BASE_URL + ApiUri.TRAVELLER_CLICK, content);
                string responseContent = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    CheckStatusResponse successResponse = JsonConvert.DeserializeObject<CheckStatusResponse>(responseContent, App.DefaultSerializerSettings);

                    if (successResponse != null && successResponse.Result != null)
                    {
                        if (successResponse.Result.Success)
                        {
                            Bookmark.TextColor = Color.White;
                            Bookmark.BackgroundColor = Color.Yellow;
                            Bookmark.Source = ImageSource.FromFile("icon_heartw");

                        }
                        else if (!string.IsNullOrWhiteSpace(successResponse.Result.Message))
                        {
                            UserDialogs.Instance.Toast(new ToastConfig(successResponse.Result.Message));
                        }
                    }
                    else
                    {
                        Internal.ServerError();
                    }
                }
                else
                {
                    Internal.ServerError();
                }
            }
            catch (TaskCanceledException)
            {
                UserDialogs.Instance.Toast(new ToastConfig("Bad Connection Error. Try Again"));
            }
            catch (Exception)
            {
                Internal.ServerError();
            }
            finally
            {
                UserDialogs.Instance.Loading().Hide();
            }
        }

        private void WebSite_Clicked(object sender, EventArgs e)
        {
            Device.OpenUri(new Uri((this.BindingContext as ListInfo).host_url));
        }

        private void Direction_Clicked(object sender, EventArgs e)
        {
            var nav = NavigateMap;
            if (nav != null)
            {
                NavigateMap(this, BindingContext);
            }
        }

        private void Call_Clicked(object sender, EventArgs e)
        {
            Device.OpenUri((new Uri("tel://" + (this.BindingContext as ListInfo).mobile)));
        }
    }
}