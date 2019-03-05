using Acr.UserDialogs;
using Newtonsoft.Json.Linq;
using TravellerApp.Constants;
using TravellerApp.Models.Response;
using TravellerApp.Models.Result;
using TravellerApp.Notifications;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TravellerApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OurStoryPage : ContentPage
    {
        public OurStoryPage()
        {
            InitializeComponent();

            FetchTimeLine();
        }

        private void FetchTimeLine()
        {
            JObject jsonDataObject = new JObject
            {
                { "auth", "H6V$36A*!?L^G2NXX7U%=GY@" }
            };

            JObject jsonData = new JObject
            {
                { "params", jsonDataObject }
            };

            WebService.Instance.PostAsyncWithTask<BaseResponse<TimeLineResult>>(ApiUri.BASE_URL + ApiUri.FetchTimeLine, jsonData, null, (res) =>
            {
                if (res.Result != null)
                {
                    if (res.Result.Success)
                    {
                        timeLine.ItemsSource = res.Result.TimeLines;
                    }
                    else if (!string.IsNullOrWhiteSpace(res.Result.Message))
                    {
                        UserDialogs.Instance.Toast(new ToastConfig(res.Result.Message));
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
            });
        }
    }
}