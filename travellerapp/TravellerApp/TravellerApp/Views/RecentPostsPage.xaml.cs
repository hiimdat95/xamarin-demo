using Acr.UserDialogs;
using FFImageLoading.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TravellerApp.Constants;
using TravellerApp.Interfaces;
using TravellerApp.Models;
using TravellerApp.Models.DTO;
using TravellerApp.Models.Response;
using TravellerApp.Models.Result;
using TravellerApp.Notifications;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TravellerApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RecentPostsPage : ContentPage
    {
        private List<AllPosts> listAllPosts = new List<AllPosts>();
        private List<ListInfo> listHostOriginal;

        public RecentPostsPage()
        {
            //LiveReload.Init();
            InitializeComponent();

            GetAllPostsAsync();
        }

        private async void GetAllPostsAsync()
        {
            ///UserDialogs.Instance.Loading(show: false, title: "Getting All Posts...").Show();
            var client = new HttpClient()
            {
                Timeout = TimeSpan.FromSeconds(20)
            };

            JObject @params = new JObject()
            {
                new JProperty("params", new JObject()
                {
                    new JProperty("auth", "H6V$36A*!?L^G2NXX7U%=GY@")
                })
            };

            var data = @params.ToString();
            var content = new StringContent(data, Encoding.UTF8, "application/json");
            Debug.WriteLine("REQUEST-GetAllPosts: " + data);

            try
            {
                HttpResponseMessage response = await client.PostAsync(ApiUri.BASE_URL + ApiUri.FetchAllPosts, content);
                string responseContent = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Debug.WriteLine("RESPONSE-GetAllPosts: " + responseContent);

                    AllPostsResponse allPostsResponse = JsonConvert.DeserializeObject<AllPostsResponse>(responseContent, App.DefaultSerializerSettings);
                    AllPostsResult allPostsResult = allPostsResponse.result;

                    if (allPostsResult.success)
                    {
                        //AllPosts.ItemsSource = allPostsResult.posts.ToList();

                        AllPosts.ItemsSource = allPostsResult.posts.Where(r => !string.IsNullOrWhiteSpace(r.text) && r.text.Length >= 180).OrderByDescending(r => r.date).ToList();
                        AllPosts_full.ItemsSource = allPostsResult.posts.Where(r => string.IsNullOrWhiteSpace(r.text) || r.text.Length < 180).OrderByDescending(r => r.date).ToList();

                        listAllPosts = allPostsResult.posts.OrderByDescending(r => r.date).ToList();
                    }
                }
                UserDialogs.Instance.Loading().Hide();
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
                Internal.ServerError();
            }
        }

        private void hostToOpenPlace(object sender, EventArgs e)
        {
            //var selectedItem = ((Button)sender).BindingContext as AllPosts;
            if (((Button)sender).BindingContext is AllPosts selectedItem)
            {
                if (!string.IsNullOrWhiteSpace(selectedItem.place_of_interest_id) && !"false".Equals(selectedItem.place_of_interest_id))
                {
                    OpenPlace(Convert.ToInt32(selectedItem.place_of_interest_id));
                }
            }
        }

        private async void OpenPlace(int id)
        {
            UserDialogs.Instance.Loading(title: "Processing Get Information...").Show();

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

                    OpenPlaceResponse openPlaceResponse = JsonConvert.DeserializeObject<OpenPlaceResponse>(responseContent);
                    OpenPlaceResult openPlaceResult = openPlaceResponse.Result;

                    if (openPlaceResult.Success)
                    {
                        var page = new OpenPlacesPage(openPlaceResult.Founder, id);
                        await Navigation.PushAsync(page, true);
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
                Internal.ServerError();
            }
        }

        private void OnRatingChanged(object sender, float e)
        {
            //customRatingBar.Rating = e;
        }

        private void ReadMoreClick(object sender, EventArgs e)
        {
            AllPosts.IsVisible = false;
            AllPosts_full.IsVisible = true;
            AllPosts.ItemsSource = listAllPosts;
            AllPosts_full.ItemsSource = listAllPosts;
        }

  
        private async void Add_Post_Clicked(object sender, EventArgs e)
        {
            //await Navigation.PushAsync(new WriteReviewPage(new Review { Host = CurrentlyAt.Host, Token = CurrentlyAt.Token, VisitToken = CurrentlyAt.visit_token }), true);
            var page = new WriteReviewPage(new Review());
          //  await PopupNavigation.Instance.PushAsync(new p(CurrentlyAt));
            await Navigation.PushAsync(page, true);
        }

        private async void hostToBooking(object sender, EventArgs e)
        {
            if (((Button)sender).BindingContext is AllPosts selectedItem)
            {
                using (UserDialogs.Instance.Loading(title: "Processing Get Information..."))
                {
                    await InitContentAsync();
                }
                if (listHostOriginal != null)
                {
                    if (!string.IsNullOrWhiteSpace(selectedItem.host_partner_name.Trim()) && !"false".Equals(selectedItem.host_partner_name.Trim()))
                    {
                        var host = listHostOriginal.Where(x => x.name.Trim().Equals(selectedItem.host_partner_name.Trim())).FirstOrDefault();
                        if (host != null)
                        {
                            var page = new BookingPage(host);
                            await Navigation.PushAsync(page, true);
                        }
                    }
                    else
                    {
                        var msgCreatBooking = "Sorry. Room not available. Please try again later.";
                        await DisplayAlert("Warning", msgCreatBooking, "OK");
                    }
                }
            }
        }

        private async Task InitContentAsync()
        {
            var client = new HttpClient()
            {
                Timeout = TimeSpan.FromSeconds(20)
            };

            var auth = "H6V$36A*!?L^G2NXX7U%=GY@";
            JObject jsonDataObject = new JObject {
                { "auth",  auth}
            };

            JObject jsonData = new JObject {
                { "params", jsonDataObject }
            };

            var data = jsonData.ToString();
            var content = new StringContent(data, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await client.PostAsync(ApiUri.BASE_URL + ApiUri.FETCH_ALL_HOSTS, content);
                string responseContent = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    ListInfoResponse listInfoResponse = JsonConvert.DeserializeObject<ListInfoResponse>(responseContent, App.DefaultSerializerSettings);
                    ListInfoResult listResult = listInfoResponse.result;

                    if (listResult.success)
                    {
                        listHostOriginal = listResult.hosts;
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
                Internal.ServerError();
            }
        }

        private void LikeClicked(object sender, EventArgs e)
        {
            var selectedItem = ((CachedImage)sender).BindingContext as AllPosts;
            var index = listAllPosts.IndexOf(selectedItem);
            if (selectedItem != null)
            {
                Like(selectedItem.rating_token, index);
            }
        }

        private void ExpanClicked(object sender, EventArgs e)
        {
            if (((StackLayout)sender).BindingContext is AllPosts selectedItem)
            {
                var index = listAllPosts.IndexOf(selectedItem);
                listAllPosts[index].tempVisibleListComment = listAllPosts[index].tempVisibleListComment ? false : true;
            }
        }

        private void OnCommentTextChanged(object sender, TextChangedEventArgs e)
        {
            if (((Editor)sender).BindingContext is AllPosts selectedItem)
            {
                var index = listAllPosts.IndexOf(selectedItem);
                listAllPosts[index].editorText = ((Editor)sender).Text;
            }
        }

        private void CommentClicked(object sender, EventArgs e)
        {
            if (((CachedImage)sender).BindingContext is AllPosts selectedItem)
            {
                var index = listAllPosts.IndexOf(selectedItem);
                listAllPosts[index].tempVisibleComment = listAllPosts[index].tempVisibleComment ? false : true;
            }
        }

        private void SendChatClicked(object sender, EventArgs e)
        {
            if (((CachedImage)sender).BindingContext is AllPosts selectedItem)
            {
                var index = listAllPosts.IndexOf(selectedItem);
                Comment(listAllPosts[index].rating_token, listAllPosts[index].editorText);
            }
        }

        private async void Like(string rating_token, int pos)
        {
            UserDialogs.Instance.Loading(title: "Processing Like...").Show();

            var client = new HttpClient()
            {
                Timeout = TimeSpan.FromSeconds(20)
            };

            var AuthService = DependencyService.Get<IAuthService>();

            JObject @params = new JObject()
            {
                new JProperty("params", new JObject()
                {
                    new JProperty("auth", new JObject()
                    {
                        { "login", AuthService.UserName },
                        { "password", AuthService.Password },
                        { "db", ServerAuth.DB }
                    }),
                    { "rating_token", rating_token}
                })
            };

            var data = @params.ToString();
            var content = new StringContent(data, Encoding.UTF8, "application/json");
            Debug.WriteLine("REQUEST-FetchPlaces: " + data);

            try
            {
                HttpResponseMessage response = await client.PostAsync(ApiUri.BASE_URL + ApiUri.LikeReview, content);
                string responseContent = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    UserDialogs.Instance.Loading().Hide();
                    Debug.WriteLine("RESPONSE-FetchPlaces: " + responseContent);

                    SuccessResponse likeResult = JsonConvert.DeserializeObject<SuccessResponse>(responseContent);
                    if (likeResult != null && likeResult.Result != null)
                    {
                        if (likeResult.Result.Success)
                        {
                            listAllPosts[pos].tempLike += 1;
                        }
                    }
                    else
                    {
                        Internal.ServerError();
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
                Internal.ServerError();
            }
        }


        private async void Comment(string rating_token, string comment)
        {
            UserDialogs.Instance.Loading(title: "Processing Comment...").Show();

            var client = new HttpClient()
            {
                Timeout = TimeSpan.FromSeconds(20)
            };

            var AuthService = DependencyService.Get<IAuthService>();

            JObject @params = new JObject()
            {
                new JProperty("params", new JObject()
                {
                    new JProperty("auth", new JObject()
                    {
                        { "login", AuthService.UserName },
                        { "password", AuthService.Password },
                        { "db", ServerAuth.DB }
                    }),
                    { "rating_token", rating_token},
                    { "comment", comment}
                })
            };

            var data = @params.ToString();
            var content = new StringContent(data, Encoding.UTF8, "application/json");
            Debug.WriteLine("REQUEST-FetchPlaces: " + data);

            try
            {
                HttpResponseMessage response = await client.PostAsync(ApiUri.BASE_URL + ApiUri.CommentReview, content);
                string responseContent = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    UserDialogs.Instance.Loading().Hide();
                    Debug.WriteLine("RESPONSE-FetchPlaces: " + responseContent);

                    SuccessResponse commentResult = JsonConvert.DeserializeObject<SuccessResponse>(responseContent);
                    if (commentResult != null && commentResult.Result != null)
                    {
                        if (commentResult.Result.Success)
                        {
                            // Reload list.
                            GetAllPostsAsync();
                        }
                    }
                    else
                    {
                        Internal.ServerError();
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
                Internal.ServerError();
            }
        }
    }
}