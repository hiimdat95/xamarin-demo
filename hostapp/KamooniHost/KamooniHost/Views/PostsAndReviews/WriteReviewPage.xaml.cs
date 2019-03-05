using Acr.UserDialogs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.Media.Abstractions;
using System.Text.RegularExpressions;
using KamooniHost.Models;
using KamooniHost.Helpers;
using System.Collections.Generic;
using KamooniHost.IDependencyServices;
using KamooniHost.Notifications;
using KamooniHost.Constants;
using KamooniHost.Models.Response;
using KamooniHost;
using System.Web;
using KamooniHost.Utils;

namespace TravellerApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WriteReviewPage : ContentPage
    {
        //private User User => Realm.GetInstance().Find<User>(DBLocalID.USER);
        private Review review;
        private MediaFile imagefile;
        private String ReviewToken;

        public Host Host => Settings.CurrentHost;
        public List<Post> Post => Settings.CurrentPost;

        //public GuestBook GuestBook => Settings.
        public WriteReviewPage() { }
        public WriteReviewPage(Review review)
        {
            //LiveReload.Init();
            InitializeComponent();
            BindingContext = this;
            this.review = review;
            Title = "Review " + review.Host;
            lbName.Text = Host.Name;
            //byte[] imageBytes = Convert.FromBase64String(Host.Image_Url);
            profilePicture.Source = Host.Image_Url;
            image.LongPressed += (sender) => imgDelete.IsVisible = true;

            // Click labble share.
            var tapGestureLableShare= new TapGestureRecognizer();
            tapGestureLableShare.Tapped += async (s, e) => {
                popupShareIns.IsVisible = false;
                if (imagefile == null) return;
                bool share = await DependencyService.Get<IShareOnInstagramAPI>().OpenShareOnInstagram(imagefile.Path,
                    (txtContent.Text?.Trim() ?? "" )+ "\n\n " + (review.Host == null ? "" : ("#" + Regex.Replace(review.Host, " ", String.Empty))) + " #Kamooni #SouthAfrica #Travel #ConsciousTravel");
                if (share)
                {
                    SubmitReviewShared();
                }
            };
            lbShare.GestureRecognizers.Add(tapGestureLableShare);

            // Click outsize popup.
            var tapGesturePopupShare = new TapGestureRecognizer();
            tapGesturePopupShare.Tapped += (s, e) => {
                popupShareIns.IsVisible = false;
            };
            popupShareIns.GestureRecognizers.Add(tapGesturePopupShare);

            // Click insize popup.
            popupChil.GestureRecognizers.Add(new TapGestureRecognizer());
            lbTapAndHold.Text = " ー Tap & hold to paste your caption in instagram";
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
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

        private void SubmitReview_Clicked(object sender, EventArgs e)
        {
            SubmitReview();
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

                //var AuthService = DependencyService.Get<IAuthService>();

                JObject jsonAuth = new JObject
                {
                    { "login", Settings.Email },
                    { "password", StringFormatUtil.ToBase64(Settings.Password) },
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
                    { "title", Host.Name },
                    { "token", Host.Token },
                    { "text", txtContent.Text?.Trim() ?? "" },
                    { "image", imageBase64 },
                    { "rating", rating.Rating },
                    { "visit_token", "" }
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
                var response = await client.PostAsync(ApiURI.URL_MAIN + ApiURI.REVIEW_HOST, content);
                string responseContent = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    ReviewHostResponse successResponse = JsonConvert.DeserializeObject<ReviewHostResponse>(responseContent, App.DefaultSerializerSettings);

                    if (successResponse != null && successResponse.Result != null)
                    {
                        if (successResponse.Result.Success)
                        {
                            ReviewToken = successResponse.Result.ReviewToken;
                            popupShareIns1.IsVisible = true;

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

        private async void SubmitReviewShared()
        {
            try
            {
                UserDialogs.Instance.Loading(title: "Submiting Review Shared...").Show();
                var client = new HttpClient()
                {
                    Timeout = TimeSpan.FromSeconds(20)
                };

                //var AuthService = DependencyService.Get<IAuthService>();

                JObject jsonAuth = new JObject
                {
                    { "login", Settings.Email },
                    { "password", Settings.Password },
                    { "db", ServerAuth.DB }
                };

                JObject jsonDataObject = new JObject
                {
                    { "auth", jsonAuth },
                    { "rating_token", ReviewToken??"" }
                };

                JObject jsonData = new JObject
                {
                    { "params", jsonDataObject }
                };


                var data = jsonData.ToString();
                var content = new StringContent(data, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(ApiURI.URL_MAIN + ApiURI.REVIEW_SHARED, content);
                string responseContent = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    SuccessResponse successResponse = JsonConvert.DeserializeObject<SuccessResponse>(responseContent, App.DefaultSerializerSettings);

                    if (successResponse != null && successResponse.Result != null)
                    {
                        if (successResponse.Result.Success)
                        {
                            //await Navigation.PopAsync(true);
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

        private void shareOnInstagram_Tapped(object sender, EventArgs e)
        {

            DependencyService.Get<IToast>().LongAlert("Copied CAPTION");
            popupShareIns.IsVisible = true;
            popupShareIns1.IsVisible = false;
        }

        private void noThanhs_Tapped(object sender, EventArgs e)
        {
            popupShareIns.IsVisible = false;
            popupShareIns1.IsVisible = false;
        }
    }
}