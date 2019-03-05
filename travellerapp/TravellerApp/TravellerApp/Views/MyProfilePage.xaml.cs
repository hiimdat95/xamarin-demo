using Acr.UserDialogs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Realms;
using Rg.Plugins.Popup.Services;
using Stormlion.ImageCropper;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TravellerApp.Constants;
using TravellerApp.Helpers;
using TravellerApp.Interfaces;
using TravellerApp.Interfaces.ComectChatCallback;
using TravellerApp.Models;
using TravellerApp.Models.Response;
using TravellerApp.Models.Result;
using TravellerApp.Utils;
using TravellerApp.Views.PopupPage;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TravellerApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyProfilePage : ContentPage
    {
        private const string screen = "MyProfilePage";
        private User user;
        private User userEditTemp;
        private bool hasAppeared;
        // FLAGS //
        private bool listEnable = false;

        //bool listInfo = false;
        private bool profilePicShouldLoad = true;

        private bool countryFlagShouldLoad = true;

        public MyProfilePage()
        {
            InitializeComponent();

            var realm = Realm.GetInstance();
            user = realm.Find<User>(DBLocalID.USER);

            InitContent();
            InitObjects();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (hasAppeared)
                return;
            hasAppeared = true;
            await GetCurrently();

        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            hasAppeared = false;
        }

        bool isFetching = false;
        private async Task GetCurrently()
        {
            if (isFetching)
                return;
            isFetching = true;
            var AuthService = DependencyService.Get<IAuthService>();

            JObject jsonAuth = new JObject {
                { "login",  AuthService.UserName},
                { "password",  AuthService.Password},
                { "db",  ServerAuth.DB}
            };

            JObject jsonDataObject = new JObject {
                { "auth",  jsonAuth}
            };

            JObject jsonData = new JObject {
                { "params", jsonDataObject }
            };

            await WebService.Instance.PostAsync<CurrentlyAtResponse>(ApiUri.BASE_URL + ApiUri.CURRENTLY_AT, content: jsonData, onSuccess: async (res) =>
            {
                if (res.result != null)
                {
                    if (res.result.success)
                    {
                        var CurrentlyAt = res.result;
                        if (CurrentlyAt != null)
                        {

                            //If not checked in have popup message "You aren't currently checked in anywhere"
                            if (!string.IsNullOrEmpty(CurrentlyAt.state) && !States.Sale.Equals(CurrentlyAt.state.ToLower()))
                                return;

                            // display the pop up in case user is currenlty at
                            await PopupNavigation.Instance.PushAsync(new RecommendedPlacesPopup(CurrentlyAt));

                        }
                    }
                }
            });
        }

        private void InitContent()
        {
            //CardHeading.FontAttributes = FontAttributes.Bold | FontAttributes.Italic;

            Name.Text = user.name;
            //PassportId.Text = user.passport_id;
            //Email.Text = user.email;
            //Mobile.Text = user.mobile;

            if (countryFlagShouldLoad)
            {
                countryFlagShouldLoad = false;

                Country country = Country.GetCountryByISO(user.country);
                FlagImage.Source = country.FlagPath;
            }

            if (profilePicShouldLoad)
            {
                profilePicShouldLoad = false;

                byte[] imageBytes = Convert.FromBase64String(user.profile_pic);
                ProfilePicture.Source = ImageSource.FromStream(() => new MemoryStream(imageBytes));
            }

            qrContainer.Source = ImageSource.FromStream(() =>
            {
                return DependencyService.Get<IBarcodeService>()?.GenerateQR(Realm.GetInstance().Find<User>(DBLocalID.USER).traveller_token);
            });
        }

        private void InitObjects()
        {
            //NameForm.Completed += (s, e) => MobileForm.Focus();

            NameForm.TextChanged += (s, e) => OnNameTextChanged(s, e);
            //MobileForm.TextChanged += (s, e) => OnMobileTextChanged(s, e);

            //AcceptButton.GestureRecognizers.Add(new TapGestureRecognizer
            //{
            //    Command = new Command(() => OnAcceptButton())
            //});

            //CancelButton.GestureRecognizers.Add(new TapGestureRecognizer
            //{
            //    Command = new Command(() => OnCancelButton())
            //});
        }

        //---start-------------------------------------------------------------------------------------CLICK EVENTS---//

      
        private void OnEditProfileButton(object sender, EventArgs e)
        {
            listEnable = !listEnable;
            ToggleEditState(listEnable);
          
        }


        private async void OnLogoutButton(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert(
                title: "Logout",
                message: "Are you sure you want to log out?",
                accept: "YES",
                cancel: "NO");

            if (confirm)
            {
                // Logout comet chat.
                if (DependencyService.Get<ICometChatService>().isCometChaInitialize())
                {
                    if (DependencyService.Get<ICometChatService>().isCometChatLogin())
                    {
                        // Logout comet chat.
                        DependencyService.Get<ICometChatService>().logout(new Callbacks(successlogin => logoutCometChatSuccess(successlogin), fail => logoutCometChatFail(fail)));
                    }
                }
                else
                {
                    // Init comet chat.
                    DependencyService.Get<ICometChatService>().initializeCometChat(CometChatConstants.siteurl, CometChatConstants.licenseKey, CometChatConstants.apiKey, CometChatConstants.isCometOnDemand, new Callbacks(success => initCometChatSuccess(success), fail => initCometChatFail(fail)));
                }


                AppSettings.LoggedIn = false;
                Application.Current.MainPage = new NavigationPage(new LoginPage());
            }
        }

        private void OnToastActionUpdateProfile()
        {
            UpdateProfile();
        }

        private void initCometChatSuccess(string success)
        {
            if (success != null)
            {
                System.Console.WriteLine("initCometChatSuccess" + success.ToString());
                // Logout comet chat.
                DependencyService.Get<ICometChatService>().logout(new Callbacks(successlogin => logoutCometChatSuccess(successlogin), fail => logoutCometChatFail(fail)));

            }
        }

        private void initCometChatFail(string fail)
        {
            UserDialogs.Instance.Loading().Hide();
            if (fail != null)
            {
                System.Console.WriteLine("initCometChatFail" + fail.ToString());
            }
        }

        private void logoutCometChatSuccess(string success)
        {
            if (success != null)
            {
                System.Console.WriteLine("loginCometChatSuccess" + success.ToString());
            }
        }

        private void logoutCometChatFail(string fail)
        {
            UserDialogs.Instance.Loading().Hide();
            if (fail != null)
            {
                System.Console.WriteLine("loginCometChatSuccess" + fail.ToString());
            }
        }

        //////////////////
        // EDIT PROFILE //
        //////////////////

        private void OnNationalityButton(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new SignUpNationalityPage(this));
        }

        private async void OnChangePictureButton(object sender, EventArgs e)
        {
            var imagePath = await PhotoHelper.TakePhotoPathAsync(screen);

            if (imagePath == null)
                return;

            new ImageCropper()
            {
                PageTitle = "Profile Picture",
                AspectRatioX = 1,
                AspectRatioY = 1,
                CropShape = ImageCropper.CropShapeType.Oval,
                Success = (imageFile) =>
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        ProfilePicture.Source = ImageSource.FromFile(imageFile);

                        GetImageString(imageFile);
                    });
                }
            }.Show(this, imagePath);
        }

        private async void GetImageString(string imageFile)
        {
            byte[] imageBytes = await StorageHelper.LoadImage(imageFile);
            userEditTemp.profile_pic = Convert.ToBase64String(imageBytes);

            profilePicShouldLoad = true;
        }

        private void OnAcceptButton()
        {
            if (AllValid())
            {
                userEditTemp.name = NameForm.Text;
                userEditTemp.email = EmailForm.Text;
                //userEditTemp.mobile = MobileForm.Text;

                UpdateProfile();
            }
        }

        private void OnCancelButton()
        {
            ToggleEditState(false);
        }

        //---end---------------------------------------------------------------------------------------CLICK EVENTS---//

        //---start----------------------------------------------------------------------------TEXT CHANGE LISTENERS---//

        private void OnNameTextChanged(object sender, TextChangedEventArgs e)
        {
            //Name.Text = NameForm.Text;
        }

        private void OnMobileTextChanged(object sender, TextChangedEventArgs e)
        {
            //Mobile.Text = MobileForm.Text;
        }

        //---end------------------------------------------------------------------------------TEXT CHANGE LISTENERS---//

        //---start----------------------------------------------------------------------------------FORM VALIDATION---//

        private bool AllValid()
        {
            ValidationError.Text = "";
            if (!NameEmpty() && !MobileNumberEmpty())
            {
                return true;
            }

            return false;
        }

        private bool NameEmpty()
        {
            if (string.IsNullOrWhiteSpace(NameForm.Text))
            {
                ValidationError.Text = "Please enter Full Name";
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool MobileNumberEmpty()
        {
            //if (string.IsNullOrWhiteSpace(MobileForm.Text))
            {
                ValidationError.Text = "Please enter Mobile Number";
                return true;
            }
            //else
            {
                return false;
            }
        }

        //---end------------------------------------------------------------------------------------FORM VALIDATION---//

        //---start--------------------------------------------------------------------------------------------OTHER---//

        private void ToggleEditState(bool editEnable)
        {

            if (editEnable)
            {
                //ButtonContainer.IsVisible = false;
                //EditContainer.IsVisible = true;
                EditLayout.IsVisible = true;
                ViewDetailLayout.IsVisible = false;
                EditFinish.IsVisible = true;
                Edit.Text = "Save";
                NameForm.Text = user.name;
                //MobileForm.Text = user.mobile;

                userEditTemp = new User
                {
                    name = user.name,
                    mobile = user.mobile,
                    country = user.country,
                    profile_pic = user.profile_pic
                };
            }
            else
            {
                EditLayout.IsVisible = false;
                ViewDetailLayout.IsVisible = true;
                Edit.Text = "Edit";
                EditFinish.IsVisible = false;
                ValidationError.Text = "";

                //EditContainer.IsVisible = false;
                //ButtonContainer.IsVisible = true;

                InitContent();
            }
        }

        //---end----------------------------------------------------------------------------------------------OTHER---//

        //---start----------------------------------------------------------------------------------------OVERRIDES---//

        protected override bool OnBackButtonPressed()
        {
            if (listEnable)
            {
                ToggleEditState(false);
                return true;
            }
            else
            {
                return base.OnBackButtonPressed();
            }
        }

        //---end------------------------------------------------------------------------------------------OVERRIDES---//

        //---start----------------------------------------------------------------------------------MODAL CALLBACKS---//

        public void OnNationalitySelected(string countryISO)
        {
            userEditTemp.country = countryISO;

            Country country = Country.GetCountryByISO(countryISO);
            FlagImage.Source = country.FlagPath;

            countryFlagShouldLoad = true;
        }

        //---end------------------------------------------------------------------------------------MODAL CALLBACKS---//

        //---start-------------------------------------------------------------------------------------SERVER COMMS---//

        private async void UpdateProfile()
        {
            UserDialogs.Instance.Loading(title: "Updating Profile...").Show();
            var client = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(20)
            };

            var AuthService = DependencyService.Get<IAuthService>();

            JObject jsonAuth = new JObject {
                { "db",  ServerAuth.DB},
                { "login",  AuthService.UserName},
                { "password",  AuthService.Password}
            };

            JObject jsonTraveller = new JObject {
                { "name", userEditTemp.name },
                { "mobile", userEditTemp.mobile },
                { "country", userEditTemp.country },
                { "image", userEditTemp.profile_pic }
            };

            JObject jsonDataObject = new JObject {
                { "auth",  jsonAuth},
                { "traveller",  jsonTraveller}
            };
            JObject jsonData = new JObject {
                { "params", jsonDataObject }
            };

            var data = jsonData.ToString();
            var content = new StringContent(data, Encoding.UTF8, "application/json");
            Debug.WriteLine("REQUEST-UpdateProfile: " + data);

            try
            {
                HttpResponseMessage response = await client.PostAsync(ApiUri.BASE_URL + ApiUri.UPDATE_TRAVELLER, content);
                string responseContent = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    UserDialogs.Instance.Loading().Hide();
                    Debug.WriteLine("RESPONSE-UpdateProfile: " + responseContent);

                    SuccessCheckResponse successCheckResponse = JsonConvert.DeserializeObject<SuccessCheckResponse>(responseContent, App.DefaultSerializerSettings);
                    SuccessCheckResult successCheckResult = successCheckResponse.result;

                    if (successCheckResult.success)
                    {
                        var realm = Realm.GetInstance();
                        realm.Write(() =>
                        {
                            user.name = userEditTemp.name;
                            user.mobile = userEditTemp.mobile;
                            user.country = userEditTemp.country;
                            user.profile_pic = userEditTemp.profile_pic;
                            realm.Add(user, update: true);
                        });

                        UserDialogs.Instance.Toast("Profile Updated Successfully");

                        ToggleEditState(false);
                    }
                    else
                    {
                        var toastAction = new ToastAction();
                        toastAction.SetText("RETRY");
                        toastAction.SetAction(OnToastActionUpdateProfile);

                        UserDialogs.Instance.Toast(
                            new ToastConfig("Profile Update Failed")
                            .SetAction(toastAction)
                        );
                    }
                }
                else
                {
                    UserDialogs.Instance.Loading().Hide();
                    Debug.WriteLine("RESPONSE_ERROR-UpdateProfile: " + responseContent);
                    Notifications.Internal.ServerError();
                }
            }
            catch (TaskCanceledException e)
            {
                UserDialogs.Instance.Loading().Hide();
                UserDialogs.Instance.Toast(new ToastConfig("Bad Connection Error. Try Again"));
                Debug.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                UserDialogs.Instance.Loading().Hide();
                Notifications.Internal.ServerError();
            }
        }

        //---end---------------------------------------------------------------------------------------SERVER COMMS---//
    }
}