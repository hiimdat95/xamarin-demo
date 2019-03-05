using Acr.UserDialogs;
using KamooniHost.Constants;
using KamooniHost.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using KamooniHost.IDependencyServices;
using TravellerApp.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using KamooniHost.Notifications;
using KamooniHost.Views;
using TravellerApp.Models.Response;
using TravellerApp.Models.Result;

namespace TravellerApp.Views
{
    public interface IUpdate
    {
        void OnUpdate(bool update);
    }

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddPlacePage : ContentPage
    {
        private Types typePlace;
        private MediaFile imagefile;
        private List<Types> types { get; set; }
        private IUpdate _updateResultListener;
        private PostsAndReviewsPage postsAndReviewsPage;

        public AddPlacePage()
        {
            //LiveReload.Init();
            InitializeComponent();
        }

        public AddPlacePage(IUpdate update)
        {
            InitializeComponent();
            this._updateResultListener = update;
            Title = "Add new place";
            image.LongPressed += (sender) => imgDelete.IsVisible = true;
            //FetchPlaceTypes();
            
            // Click outsize popup.
            var tapGesturePopupTypes = new TapGestureRecognizer();
            tapGesturePopupTypes.Tapped += (s, e) => {
                popupTypes.IsVisible = false;
            };
            popupTypes.GestureRecognizers.Add(tapGesturePopupTypes);

            // Type click.
            var tapGestureTypes = new TapGestureRecognizer();
            tapGestureTypes.Tapped += (s, e) => {
                popupTypes.IsVisible = true;
            };
            stackTypes.GestureRecognizers.Add(tapGestureTypes);

            // lbCancel click.
            var tapGestureCancel = new TapGestureRecognizer();
            tapGestureCancel.Tapped += (s, e) => {
                popupTypes.IsVisible = false;
            };
            lbCancel.GestureRecognizers.Add(tapGestureCancel);

            // Click insize popup.
            popupChil.GestureRecognizers.Add(new TapGestureRecognizer());
        }

        public AddPlacePage(PostsAndReviewsPage postsAndReviewsPage)
        {
            this.postsAndReviewsPage = postsAndReviewsPage;
            InitializeComponent();

            Title = "Write Review";
            image.LongPressed += (sender) => imgDelete.IsVisible = true;
            FetchPlaceTypes();

            // Click outsize popup.
            var tapGesturePopupTypes = new TapGestureRecognizer();
            tapGesturePopupTypes.Tapped += (s, e) => {
                popupTypes.IsVisible = false;
            };
            popupTypes.GestureRecognizers.Add(tapGesturePopupTypes);

            // Type click.
            var tapGestureTypes = new TapGestureRecognizer();
            tapGestureTypes.Tapped += (s, e) => {
                popupTypes.IsVisible = true;
            };
            stackTypes.GestureRecognizers.Add(tapGestureTypes);

            // lbCancel click.
            var tapGestureCancel = new TapGestureRecognizer();
            tapGestureCancel.Tapped += (s, e) => {
                popupTypes.IsVisible = false;
            };
            lbCancel.GestureRecognizers.Add(tapGestureCancel);

            // Click insize popup.
            popupChil.GestureRecognizers.Add(new TapGestureRecognizer());
        }

        private void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            popupTypes.IsVisible = false;
            if (e.SelectedItem == null) return;
            this.typePlace = e.SelectedItem as Types;
            this.typesPicker.TextColor = Color.Black;
            this.typesPicker.Text = this.typePlace.name;
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

        private void AddPlace_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                DependencyService.Get<IToast>().LongAlert("Please write name of place!");
                return;
            }

            if (this.typePlace == null)
            {
                DependencyService.Get<IToast>().LongAlert("Please select type of place!");
                return;
            }


            if (string.IsNullOrWhiteSpace(txtContent.Text))
            {
                DependencyService.Get<IToast>().LongAlert("Please write review for place!");
                return;
            }

            if (this.imagefile == null)
            {
                DependencyService.Get<IToast>().LongAlert("Please add image for review!");
                return;
            }

            SubmitPlace();
        }


        private async void FetchPlaceTypes()
        {
            this.types = new List<Types>();
            UserDialogs.Instance.Loading(title: "Processing Get Information...").Show();
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
                HttpResponseMessage response = await client.PostAsync(ApiURI.URL_MAIN + ApiURI.FETCH_PLACE_TYPES, content);
                string responseContent = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    UserDialogs.Instance.Loading().Hide();

                    ListPlaceTypesResponse listPlaceTypesResponse = JsonConvert.DeserializeObject<ListPlaceTypesResponse>(responseContent);
                    ListPlaceTypesResult listPlaceTypesResult = listPlaceTypesResponse.Result;
                    this.types.AddRange(listPlaceTypesResult.Types.ToList());
                    this.lstViewTypes.ItemsSource = this.types;
                }
            }
            catch (TaskCanceledException ex)
            {
                UserDialogs.Instance.Loading().Hide();
                UserDialogs.Instance.Toast(new ToastConfig("Bad Connection Error. Try Again"));
            }
            catch (Exception exx)
            {
                UserDialogs.Instance.Loading().Hide();
                //Notifications.Internal.ServerError();
            }
        }


        private void SubmitPlace()
        {
            try
            {
                //UserDialogs.Instance.Loading(title: "Add place...").Show();
                //var client = new HttpClient()
                //{
                //    Timeout = TimeSpan.FromSeconds(30)
                //};

                //var AuthService = DependencyService.Get<IAuthService>();

                //JObject jsonAuth = new JObject
                //{
                //    { "login", AuthService.UserName },
                //    { "password", AuthService.Password },
                //    { "db", ServerAuth.DB }
                //};
                //string imageBase64 = "";
                //if (imagefile != null)
                //{
                //    byte[] imageBytes = await StorageHelper.LoadImage(imagefile.Path);
                //    imageBase64 = Convert.ToBase64String(imageBytes);
                //}

                //JObject jsonReview = new JObject
                //{
                //    { "title", txtName.Text?.Trim() ?? ""  },
                //    { "text", txtContent.Text?.Trim() ?? "" },
                //    { "image", imageBase64 },
                //    { "rating", rating.Rating }
                //};

                //var location = await LocationHelper.GetCurrentPosition(50);
                //JObject jsonPlace = new JObject
                //{
                //    { "name", txtName.Text?.Trim() ?? ""  },
                //    { "place_id", typePlace == null? "" : typePlace.id + "" },
                //    { "latitude", location == null? 0 : location.Latitude},
                //    { "longitude", location == null? 0 : location.Longitude }
                //};

                //JObject jsonDataObject = new JObject
                //{
                //    { "auth", jsonAuth },
                //    { "review", jsonReview },
                //    { "place", jsonPlace }
                //};

                //JObject jsonData = new JObject
                //{
                //    { "params", jsonDataObject }
                //};


                //var data = jsonData.ToString();
                //var content = new StringContent(data, Encoding.UTF8, "application/json");
                //var response = await client.PostAsync(ApiURI.BASE_URL + ApiUri.CREATE_PLACE, content);
                //string responseContent = await response.Content.ReadAsStringAsync();

                //if (response.StatusCode == HttpStatusCode.OK)
                //{
                //    SuccessResponse successResponse = JsonConvert.DeserializeObject<SuccessResponse>(responseContent, App.DefaultSerializerSettings);

                //    if (successResponse != null && successResponse.Result != null)
                //    {
                //        if (successResponse.Result.Success)
                //        {
                //            DependencyService.Get<IToast>().LongAlert("Add new place success!");
                //            await Navigation.PopAsync(true);
                //            if (this._updateResultListener != null)
                //                this._updateResultListener.OnUpdate(true);

                //        }
                //        else if (!string.IsNullOrWhiteSpace(successResponse.Result.Message))
                //        {
                //            UserDialogs.Instance.Toast(new ToastConfig(successResponse.Result.Message));
                //        }
                //    }
                //    else
                //    {
                //        Internal.ServerError();
                //    }
                //}
                //else
                //{
                //    Internal.ServerError();
                //}
            }
            catch (TaskCanceledException e)
            {
                UserDialogs.Instance.Toast(new ToastConfig("Bad Connection Error. Try Again"));
            }
            catch (Exception e)
            {
                Internal.ServerError();
            }
            finally
            {
                UserDialogs.Instance.Loading().Hide();
            }
        }
    }
}