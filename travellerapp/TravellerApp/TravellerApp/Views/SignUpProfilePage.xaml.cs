using Acr.UserDialogs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Realms;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TravellerApp.Constants;
using TravellerApp.Models;
using TravellerApp.Models.Response;
using TravellerApp.Models.Result;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TravellerApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignUpProfilePage : ContentPage
    {
        ///////////
        // FLAGS //
        ///////////

        private bool genderSelected = false;
        private bool dateSelected = false;

        //////////////////
        // DYNAMIC VARS //
        //////////////////

        private string gender;
        private string birthDate;

        public SignUpProfilePage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, true);

            InitContent();
            InitObjects();
        }

        private void InitContent()
        {
            var realm = Realm.GetInstance();
            User user = realm.Find<User>(DBLocalID.USER_TEMP_SIGN_UP);

            byte[] imageBytes = Convert.FromBase64String(user.profile_pic);
            profilePicture.Source = ImageSource.FromStream(() => new MemoryStream(imageBytes));

            Country country = Country.GetCountryByISO(user.country);
            flagImage.Source = country.FlagPath;
        }

        private void InitObjects()
        {
            passportIdForm.Completed += (s, e) => mobileForm.Focus();

            maleSelector.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => OnMaleSelected())
            });

            femaleSelector.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => OnFemaleSelected())
            });
        }

        //---start-------------------------------------------------------------------------------------CLICK EVENTS---//

        private void OnDateSelected(object sender, EventArgs e)
        {
            if (!dateSelected)
                dateSelected = true;

            DateTime dateFromPicker = datePicker.Date;
            birthDate = string.Format("{0:yyyy-MM-dd}", dateFromPicker);
        }

        private void OnMaleSelected()
        {
            femaleSelector.BackgroundColor = Color.White;
            femaleSelector.HasShadow = true;
            femaleText.TextColor = Color.Accent;

            maleSelector.BackgroundColor = Color.Accent;
            maleSelector.HasShadow = false;
            maleText.TextColor = Color.White;

            gender = "male";

            if (!genderSelected)
                genderSelected = true;
        }

        private void OnFemaleSelected()
        {
            maleSelector.BackgroundColor = Color.White;
            maleSelector.HasShadow = true;
            maleText.TextColor = Color.Accent;

            femaleSelector.BackgroundColor = Color.Accent;
            femaleSelector.HasShadow = false;
            femaleText.TextColor = Color.White;

            gender = "female";

            if (!genderSelected)
                genderSelected = true;
        }

        private void OnFinishButton(object sender, EventArgs e)
        {
            if (AllValid())
            {
                var realm = Realm.GetInstance();
                User user = realm.Find<User>(DBLocalID.USER_TEMP_SIGN_UP);

                realm.Write(() =>
                {
                    user.gender = gender;
                    user.birthdate = birthDate;
                    user.passport_id = passportIdForm.Text.Trim();
                    user.mobile = mobileForm.Text.Trim();
                    realm.Add(user, update: true);
                });

                SignUp();
            }
        }

        //---end---------------------------------------------------------------------------------------CLICK EVENTS---//

        //---start----------------------------------------------------------------------------------FORM VALIDATION---//

        private bool AllValid()
        {
            validationError.Text = "";
            if (!PassportIdEmpty() && !MobileNumberEmpty())
            {
                if (dateSelected)
                {
                    if (genderSelected)
                    {
                        return true;
                    }
                    else
                    {
                        validationError.Text = "Please select Gender";
                    }
                }
                else
                {
                    validationError.Text = "Please select Date of Birth";
                }
            }

            return false;
        }

        private bool PassportIdEmpty()
        {
            if (string.IsNullOrWhiteSpace(passportIdForm.Text))
            {
                validationError.Text = "Please enter Passport/ID";
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool MobileNumberEmpty()
        {
            if (string.IsNullOrWhiteSpace(mobileForm.Text))
            {
                validationError.Text = "Please enter Mobile Number";
                return true;
            }
            else
            {
                return false;
            }
        }


        //---end------------------------------------------------------------------------------------FORM VALIDATION---//

        //---start-------------------------------------------------------------------------------------SERVER COMMS---//

        private async void SignUp()
        {
            UserDialogs.Instance.Loading(title: "Processing...").Show();
            var client = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(120)
            };

            var realm = Realm.GetInstance();
            User user = realm.Find<User>(DBLocalID.USER_TEMP_SIGN_UP);

            JObject jsonTraveller = new JObject {
                { "name", user.name },
                { "mobile", user.mobile },
                { "email", user.email },
                { "passport_id", user.passport_id },
                { "country", user.country },
                { "image", user.profile_pic },
                { "birthdate", user.birthdate },
                { "gender", user.gender },
                { "passport_image", user.passport_pic },
            };

            JObject jsonDataObject = new JObject {
                { "traveller",  jsonTraveller}
            };
            JObject jsonData = new JObject {
                { "params", jsonDataObject }
            };

            var data = jsonData.ToString();
            var content = new StringContent(data, Encoding.UTF8, "application/json");
            Debug.WriteLine("REQUEST-SignUp: " + data);

            try
            {
                HttpResponseMessage response = await client.PostAsync(ApiUri.BASE_URL + ApiUri.CREATE_TRAVELLER, content);
                string responseContent = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    UserDialogs.Instance.Loading().Hide();
                    Debug.WriteLine("RESPONSE-SignUp: " + responseContent);

                    SuccessCheckResponse successCheckResponse = JsonConvert.DeserializeObject<SuccessCheckResponse>(responseContent, App.DefaultSerializerSettings);
                    SuccessCheckResult successCheckResult = successCheckResponse.result;

                    if (successCheckResult.success)
                    {
                        var okPressed = await DisplayAlert(
                            "Welcome to Kamooni Traveller",
                            "We sent a link to your email.\n\nPlease finish Sign Up from the link provided.\nAfter, you will be able to log into the app.",
                            "GOT IT",
                            " ");

                        if (okPressed)
                        {
                            await Navigation.PopToRootAsync();
                        }
                        else
                        { // To accomodate if the blank cancel text is pressed
                            await Navigation.PopToRootAsync();
                        }
                    }
                    else
                    {
                        Notifications.Internal.ServerError();
                    }
                }
                else
                {
                    UserDialogs.Instance.Loading().Hide();
                    Debug.WriteLine("RESPONSE_ERROR-SignUp: " + responseContent);
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