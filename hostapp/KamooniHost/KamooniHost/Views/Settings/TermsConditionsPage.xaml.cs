using Acr.UserDialogs;
using KamooniHost.Constants;
using KamooniHost.Models;
using KamooniHost.Models.Local;
using Newtonsoft.Json.Linq;          
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KamooniHost.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TermsConditionsPage : ContentPage
    {
        //private LocalHost host;
        public TermsConditionsPage()
        {
            //LiveReload.Init();
            InitializeComponent();

            TAndC.Completed += (s, e) => URLTerm.Focus();             
        }

        private void ChangeTerm(object sender, EventArgs e)
        {           
            enableChangeNewTerm.IsVisible = false;
            saveNewTerm.IsVisible = true;
            TAndC.InputTransparent = false;
            URLTerm.InputTransparent = false;
        }

        //private async void SaveNewTerm(object sender, EventArgs e)
        //{
        //    string msgSaveNewTerm;
        //    UserDialogs.Instance.Loading(title: "Save New Terms and Conditions...").Show();
        //    var client = new HttpClient()
        //    {
        //        Timeout = TimeSpan.FromSeconds(20)
        //    };

        //    JObject @params = new JObject()
        //    {
        //        new JProperty("params", new JObject()
        //        {
        //            new JProperty("auth", new JObject()
        //            {
        //                new JProperty("login", host.Email),
        //                new JProperty("password", host.Token)
        //            }),
        //            new JProperty("host", new JObject()
        //            {
        //                new JProperty("terms", host.Terms),
        //                new JProperty("url_terms", host.UrlTerms)
        //            })
        //        })
        //    };

        //    var data = @params.ToString();
        //    var content = new StringContent(data, Encoding.UTF8, "application/json");
        //    Debug.WriteLine("REQUEST-UpdateTermsAndConditions: " + data);

        //    try
        //    {
        //        HttpResponseMessage response = await client.PostAsync(ApiURI.URL_MAIN + ApiURI.UPDATE_TERMS, content);
        //        string responseContent = await response.Content.ReadAsStringAsync();

        //        if (response.StatusCode == HttpStatusCode.OK)
        //        {
        //            msgSaveNewTerm = "Successfully";
        //            await DisplayAlert("Save New Term Successfully", msgSaveNewTerm, TranslateExtension.GetValue("ok"));
        //            enableChangeNewTerm.IsVisible = true;
        //            saveNewTerm.IsVisible = false;
        //            TAndC.InputTransparent = true;
        //            URLTerm.InputTransparent = true;
        //        }
        //        else
        //        {
        //            msgSaveNewTerm = "Unsuccessfully";
        //            await DisplayAlert("Save New Term Unsuccessfully", msgSaveNewTerm, TranslateExtension.GetValue("ok"));
        //        }
        //    }
        //    catch (TaskCanceledException ex)
        //    {
        //        UserDialogs.Instance.Loading().Hide();
        //        UserDialogs.Instance.Toast(new ToastConfig("Bad Connection Error. Try Again"));
        //        Debug.WriteLine(ex.Message);
        //    }
        //    catch (Exception exx)
        //    {
        //        Debug.WriteLine(exx.Message);
        //        UserDialogs.Instance.Loading().Hide();
        //        //Notifications.Internal.ServerError();
        //    }
        //}          
    }
}