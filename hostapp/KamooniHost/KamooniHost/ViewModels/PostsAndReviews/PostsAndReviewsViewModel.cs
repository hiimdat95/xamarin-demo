using KamooniHost.Helpers;
using KamooniHost.IServices;
using Xamarin.Forms;
using Xamarin.Forms.Extensions;
using System.Threading.Tasks;
using Xamarin.Forms.TinyMVVM;
using KamooniHost.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System;
using Acr.UserDialogs;
using System.Net.Http;
using System.Text;
using KamooniHost.Constants;
using System.Net;
using KamooniHost.Models.Response;
using Newtonsoft.Json;
using KamooniHost.Notifications;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using KamooniHost.Models.Result;

namespace KamooniHost.ViewModels
{
    public class PostsAndReviewsViewModel : TinyViewModel
    {
        private readonly IPostsAndReviewsService postsAndReviewsService;

        public Host Host => Helpers.Settings.CurrentHost;

        public List<Post> Post => Helpers.Settings.CurrentPost;

        public string email;
        public string Email { get => email; set => SetProperty(ref email, value); }

        public string password;
        public string Password { get => password; set => SetProperty(ref password, value); }

        private ObservableCollection<Posts> listPost = new ObservableCollection<Posts>();
        public ObservableCollection<Posts> ListPost { get => listPost; set => SetProperty(ref listPost, value); }

        public PostsAndReviewsViewModel(IPostsAndReviewsService postsAndReviewsService)
        {
            //LiveReload.Init();
            this.postsAndReviewsService = postsAndReviewsService;
        }

        public override void Init(object data)
        {
            //LiveReload.Init();
            base.Init(data);
            //GetPost();
            GetHostDetailAsync();
        }
        
        private async void GetHostDetailAsync()
        {

            if (IsBusy || !ConnectivityHelper.IsNetworkAvailable())
            {
                return;
            }

            try
            {
               
                using (UserDialogs.Instance.Loading(title: "Fetching Data..."))
                    {
                    var client = new HttpClient()
                        {
                            Timeout = TimeSpan.FromSeconds(20)
                        };

                    JObject jsonDataObject = new JObject
                    {
                        { "host_token", Host.Token }
                    };

                    JObject jsonData = new JObject
                    {
                        { "params", jsonDataObject }
                    };

                    var data = jsonData.ToString();
                    var content = new StringContent(data, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(ApiURI.URL_MAIN + ApiURI.FETCH_POSTS_FOR_HOST, content);
                    string responseContent = await response.Content.ReadAsStringAsync();

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        PostsResponse postResponse = JsonConvert.DeserializeObject<PostsResponse>(responseContent, App.DefaultSerializerSettings);
                        PostsResult postsResult = postResponse.Result;

                        if (postResponse != null && postResponse.Result != null)
                        {
                            if (postsResult.success)
                            {
                                ListPost = new ObservableCollection<Posts>(postsResult.posts.OrderByDescending(x => x.date).ToList());
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
            }
            catch (TaskCanceledException e)
            {
                UserDialogs.Instance.Toast(new ToastConfig("Bad Connection Error. Try Again"));
                Debug.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                GetHostDetailAsync1();
                Debug.WriteLine(e.Message);
            }
            finally
            {
               

            }
        }

        private async void GetHostDetailAsync1()
        {

            if (IsBusy || !ConnectivityHelper.IsNetworkAvailable())
            {
                return;
            }

            try
            {

                var client = new HttpClient()
                    {
                        Timeout = TimeSpan.FromSeconds(20)
                    };

                    JObject jsonDataObject = new JObject
                    {
                        { "host_token", Host.Token }
                    };

                    JObject jsonData = new JObject
                    {
                        { "params", jsonDataObject }
                    };

                    var data = jsonData.ToString();
                    var content = new StringContent(data, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(ApiURI.URL_MAIN + ApiURI.FETCH_POSTS_FOR_HOST, content);
                    string responseContent = await response.Content.ReadAsStringAsync();

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        PostsResponse postResponse = JsonConvert.DeserializeObject<PostsResponse>(responseContent, App.DefaultSerializerSettings);
                        PostsResult postsResult = postResponse.Result;

                        if (postResponse != null && postResponse.Result != null)
                        {
                            if (postsResult.success)
                            {
                                ListPost = new ObservableCollection<Posts>(postsResult.posts.OrderByDescending(x => x.date).ToList());
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
                //UserDialogs.Instance.Toast(new ToastConfig("Bad Connection Error. Try Again"));
                Debug.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                //Internal.ServerError();
                Debug.WriteLine(e.Message);
            }
            finally
            {


            }
        }
    }
}
