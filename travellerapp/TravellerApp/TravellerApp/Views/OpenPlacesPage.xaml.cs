using Acr.UserDialogs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TravellerApp.Constants;
using TravellerApp.Helpers;
using TravellerApp.Interfaces;
using TravellerApp.Models;
using TravellerApp.Models.Response;
using TravellerApp.Models.Result;
using TravellerApp.Notifications;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TravellerApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OpenPlacesPage : ContentPage
    {
        private List<review> listReviewsData = new List<review>();
        private Founder founderData;
        private MediaFile imagefile;
        private int id;

        public OpenPlacesPage()
        {
            //LiveReload.Init();
            InitializeComponent();
        }

        public OpenPlacesPage(Founder founder, int id)
        {
            InitializeComponent();

            GetDataOpenPlace(id);

            this.id = id;
            image.LongPressed += (sender) => imgDelete.IsVisible = true;
        }

        private void SubmitReview_Clicked(object sender, EventArgs e)
        {
            if (this.founderData == null) return;
            if (this.imagefile == null)
            {
                DependencyService.Get<IToast>().LongAlert("Please add image for review!");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtContent.Text))
            {
                DependencyService.Get<IToast>().LongAlert("Please write review!");
                return;
            }

            SubmitReview();
        }

        private void InitFillData()
        {
            try
            {
                var reviews = listReviewsData.ToList();
                var founder = founderData;
                //var model = Founder;
                //var listFounder = new List<Founder>
                //{
                //    model
                //};

                name_header_place.Text = founder.name;
                img_header_place.Source = founder.image_url;

                foreach (var item in reviews)
                {
                    if (founder.traveller_partner_id == item.traveller_partner_id)
                    {
                        ImageSource image = !(item.image_medium is string base64) ? null : ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(base64)));

                        image_medium_first.Source = image;
                        traveller_partner_id_first.Text = item.traveller_partner_id;
                        dateDisplay_first.Text = item.dateDisplay;
                        total_visits_first.Text = item.totalVisitedDisplay;
                        text_first.Text = item.text;
                        
                    }
                }

                OpenPlaces.ItemsSource = reviews.Where(x => x.traveller_partner_id != founder.traveller_partner_id).OrderByDescending(r => r.date).ToList();

            }
            catch (TaskCanceledException ex)
            {
                UserDialogs.Instance.Loading().Hide();
                UserDialogs.Instance.Toast(new ToastConfig("Bad Connection Error. Try Again"));
                Debug.WriteLine(ex.Message);
            }
            catch (Exception exx)
            {
                Debug.WriteLine(exx.Message);
                UserDialogs.Instance.Loading().Hide();
                Notifications.Internal.ServerError();
            }
        }

        private async void GetDataOpenPlace(int id)
        {
            UserDialogs.Instance.Loading(title: "Getting all posts about this place").Show();

            var client = new HttpClient()
            {
                Timeout = TimeSpan.FromSeconds(20)
            };

            JObject @params = new JObject()
            {
                new JProperty("params", new JObject()
                {
                    new JProperty("auth", new JObject()
                    {
                        new JProperty("token", "H6V$36A*!?L^G2NXX7U%=GY@"),
                        new JProperty("id", id)
                    })
                })
            };

            var data = @params.ToString();
            var content = new StringContent(data, Encoding.UTF8, "application/json");
            Debug.WriteLine("REQUEST-FetchPlaces: " + data);

            try
            {
                HttpResponseMessage response = await client.PostAsync(ApiUri.BASE_URL + ApiUri.OPEN_PLACE, content);
                string responseContent = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    UserDialogs.Instance.Loading().Hide();
                    Debug.WriteLine("RESPONSE-FetchPlaces: " + responseContent);

                    OpenPlaceResponse openPlaceResponse = JsonConvert.DeserializeObject<OpenPlaceResponse>(responseContent, App.DefaultSerializerSettings);
                    OpenPlaceResult openPlaceResult = openPlaceResponse.Result;

                    if (openPlaceResult.Success)
                    {
                        listReviewsData = openPlaceResult.Reviews;
                        founderData = openPlaceResult.Founder;

                        InitFillData();
                    }
                }
            }
            catch (TaskCanceledException ex)
            {
                UserDialogs.Instance.Loading().Hide();
                UserDialogs.Instance.Toast(new ToastConfig("Bad Connection Error. Try Again"));
                Debug.WriteLine(ex.Message);
            }
            catch (Exception exx)
            {
                Debug.WriteLine(exx.Message);
                UserDialogs.Instance.Loading().Hide();
                Notifications.Internal.ServerError();
            }
        }

        private void OnRatingChanged(object sender, float e)
        {
            //customRatingBar.Rating = e;
        }

        private async void hostToBooking(object sender, EventArgs e)
        {
            //var selectedItem = ((Button)sender).BindingContext as AllPosts;
            //if (selectedItem != null)
            //{
            //    await InitContentAsync();
            //    if (listHostOriginal != null)
            //    {
            //        var host = listHostOriginal.Where(x => x.name.Trim().Equals(selectedItem.host_partner_id.Trim())).FirstOrDefault();
            //        if (host != null)
            //        {
            //            var page = new BookingPage(host);
            //            await Navigation.PushAsync(page, true);
            //        }
            //    }
            //}
        }

        private void Pins_Google_Maps_Tapped(object sender, EventArgs e)
        {
            if (this.founderData == null) return;
            var name = Uri.EscapeUriString(this.founderData.name);
            var loc = string.Format("{0},{1}", this.founderData.latitude, this.founderData.longitude);
            var addr = Uri.EscapeUriString(this.founderData.name);
            if (Device.RuntimePlatform == Device.iOS)
            {
                var request = string.Format("http://maps.apple.com/maps?q={0}&sll={1}", name.Replace(' ', '+'), loc);
                //var request = string.Format("http://maps.apple.com/?daddr=" + currentHost.partner_latitude + "," + currentHost.partner_longitude + "");
                Device.OpenUri(new Uri(request));
            }

            if (Device.RuntimePlatform == Device.Android)
            {
                var request = string.Format("geo:0,0?q={0}({1})", string.IsNullOrWhiteSpace(addr) ? loc : addr, name);
                //var request = string.Format("http://maps.google.com/?daddr=" + currentHost.partner_latitude + "," + currentHost.partner_longitude + "");
                Device.OpenUri(new Uri(request));
            }
        }

        void OnDeleteImageTapped(object sender, EventArgs args)
        {
            this.imagefile = null;
            image.Source = "";
            imgDelete.IsVisible = false;
        }

        async void OnCameraTapped(object sender, EventArgs args)
        {
            var action = await DisplayActionSheet("Add image", "Cancel", null, "Camera", "Library");
            if (action.Equals("Camera"))
            {
                this.imagefile = await PhotoHelper.TakePhotoFileAsync();
                if (this.imagefile != null)
                {
                    image.Source = ImageSource.FromFile(this.imagefile.Path);
                }
            }
            else if (action.Equals("Library"))
            {
                this.imagefile = await PhotoHelper.PickPhotoFileAsync();
                if (imagefile != null)
                {
                    this.image.Source = ImageSource.FromFile(this.imagefile.Path);
                }
            }
        }

        private async void SubmitReview()
        {
            try
            {
                UserDialogs.Instance.Loading(title: "Submiting Review...").Show();
                var client = new HttpClient()
                {
                    Timeout = TimeSpan.FromSeconds(30)
                };

                var AuthService = DependencyService.Get<IAuthService>();

                JObject jsonAuth = new JObject
                {
                    { "login", AuthService.UserName },
                    { "password", AuthService.Password },
                    { "db", ServerAuth.DB }
                };
                string imageBase64 = "";
                if (imagefile != null)
                {
                    byte[] imageBytes = await StorageHelper.LoadImage(imagefile.Path);
                    imageBase64 = Convert.ToBase64String(imageBytes);
                }

                JObject jsonReview = new JObject
                {
                    { "title", this.founderData == null ? "" : this.founderData.name },
                    { "text", txtContent.Text?.Trim() ?? "" },
                    { "image", imageBase64 },
                    { "rating", rating.Rating },
                    { "place_id", this.id }
                };

                JObject jsonDataObject = new JObject
                {
                    { "auth", jsonAuth },
                    { "review", jsonReview }
                };

                JObject jsonData = new JObject
                {
                    { "params", jsonDataObject }
                };

                var data = jsonData.ToString();
                var content = new StringContent(data, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(ApiUri.BASE_URL + ApiUri.REVIEW_PLACE, content);
                string responseContent = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    ReviewHostResponse successResponse = JsonConvert.DeserializeObject<ReviewHostResponse>(responseContent, App.DefaultSerializerSettings);

                    if (successResponse != null && successResponse.Result != null)
                    {
                        if (successResponse.Result.Success)
                        {
                            DependencyService.Get<IToast>().LongAlert("Post review success!");
                            // Clean data review.
                            txtContent.Text = "";
                            image.Source = "";
                            // Reload page.
                            this.GetDataOpenPlace(this.id);
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
        }
    }
}