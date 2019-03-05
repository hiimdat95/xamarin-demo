using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TravellerApp.Constants;
using TravellerApp.Interfaces;
using TravellerApp.Models.Response;
using TravellerApp.Notifications;
using Xamarin.Forms;

namespace TravellerApp.Views
{
    public partial class TestPage : ContentPage
    {
        String checkoutId;
        async void Handle_Clicked(object sender, System.EventArgs e)
        {
            checkoutId = await GetCheckoutId("7", "58", "22", "99", false, "ZAR");
            bool payment = await DependencyService.Get<IPeachPayment>().OpenPaymentUi(checkoutId);
            if (payment)
            {
                bool status = await NotifyPaymentComplete("7", "58", checkoutId);
                if (status)
                {
                    DependencyService.Get<IToast>().LongAlert("Payment success");
                }
                else
                {
                    DependencyService.Get<IToast>().LongAlert("Payment fail");
                }
            }
        }

        public TestPage()
        {
            InitializeComponent();

        }

        private async Task<String> GetCheckoutId(String host_id, String kamooni_box_id, String traveller_id, String amount, bool booking_token, String currency_code)
        {
            String id = "";
            try
            {
                UserDialogs.Instance.Loading(title: "Get chechout id ...").Show();
                var client = new HttpClient()
                {
                    Timeout = TimeSpan.FromSeconds(20)
                };


                JObject jsonDataObject = new JObject
                {
                    { "host_id", host_id },
                    { "kamooni_box_id", kamooni_box_id },
                    { "traveller_id", traveller_id },
                    { "amount", amount },
                    { "booking_token", booking_token },
                    { "currency_code", currency_code }

                };

                JObject jsonData = new JObject
                {
                    { "params", jsonDataObject }
                };

                var data = jsonData.ToString();
                var content = new StringContent(data, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(ApiUri.BASE_URL + ApiUri.CheckoutId, content);
                string responseContent = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    SuccessCheckoutId successCheckoutId = JsonConvert.DeserializeObject<SuccessCheckoutId>(responseContent, App.DefaultSerializerSettings);
                    if (successCheckoutId != null && successCheckoutId.Result != null)
                    {
                        id = successCheckoutId.Result.CheckoutId;
                    }
                }
                else
                {
                    Internal.ServerError();
                }
            }
            catch (TaskCanceledException e)
            {
                UserDialogs.Instance.Toast(new ToastConfig("Bad Connection Error. Try Again"));
                Debug.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                Internal.ServerError();
                Debug.WriteLine(e.Message);
            }
            finally
            {
                UserDialogs.Instance.Loading().Hide();
            }   

            return id;
        }

        private async Task<bool> NotifyPaymentComplete(String host_id, String kamooni_box_id, String checkout_id)
        {
            bool success = false;
            try
            {
                UserDialogs.Instance.Loading(title: "Get chechout id ...").Show();
                var client = new HttpClient()
                {
                    Timeout = TimeSpan.FromSeconds(20)
                };


                JObject jsonData = new JObject
                {
                    { "host_id", host_id },
                    { "kamooni_box_id", kamooni_box_id },
                    { "checkout_id", checkout_id }

                };


                var data = jsonData.ToString();
                var content = new StringContent(data, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(ApiUri.BASE_URL + ApiUri.NotifyPaymentComplete, content);
                string responseContent = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    // TODO
                    success = true;
                }
                else
                {
                    Internal.ServerError();
                }
            }
            catch (TaskCanceledException e)
            {
                UserDialogs.Instance.Toast(new ToastConfig("Bad Connection Error. Try Again"));
                Debug.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                Internal.ServerError();
                Debug.WriteLine(e.Message);
            }
            finally
            {
                UserDialogs.Instance.Loading().Hide();
            }

            return success;
        }
    }

}
