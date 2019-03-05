using Acr.UserDialogs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Realms;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using TravellerApp.Constants;
using TravellerApp.Models;
using TravellerApp.Models.Response;
using TravellerApp.Models.Result;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Console = System.Diagnostics;

namespace TravellerApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignUpPage : ContentPage
    {
        public SignUpPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, true);

            InitObjects();
        }

        private void InitObjects()
        {
            emailForm.Completed += (s, e) => firstNameForm.Focus();
            firstNameForm.Completed += (s, e) => lastNameForm.Focus();

            loginText.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => OnLogin())
            });
        }

        //---start-------------------------------------------------------------------------------------CLICK EVENTS---//

        private void OnSignUpButton(object sender, EventArgs e)
        {
            validationError.Text = "";

            if (!EmailEmpty() && !FirstNameEmpty() && !LastNameEmpty())
            {
                if (EmailValid())
                {
                    UserExistCheck();
                }
            }
        }

        private async void OnLogin()
        {
            await Navigation.PopAsync();
        }

        private async void OnToastActionLogin()
        {
            await Navigation.PopAsync();
        }

        //---end---------------------------------------------------------------------------------------CLICK EVENTS---//

        //---start----------------------------------------------------------------------------------FORM VALIDATION---//

        private bool EmailEmpty()
        {
            if (string.IsNullOrWhiteSpace(emailForm.Text))
            {
                validationError.Text = "Please enter Email";
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool FirstNameEmpty()
        {
            if (string.IsNullOrWhiteSpace(firstNameForm.Text))
            {
                validationError.Text = "Please enter First Name";
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool LastNameEmpty()
        {
            if (string.IsNullOrWhiteSpace(lastNameForm.Text))
            {
                validationError.Text = "Please enter Last Name";
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool EmailValid()
        {
            bool valid = true;
            try
            {
                new MailAddress(emailForm.Text.Trim());
            }
            catch (Exception e)
            {
                Console.Debug.WriteLine(e.ToString());

                validationError.Text = "Email is invalid";
                valid = false;
            }

            return valid;
        }

        //---end------------------------------------------------------------------------------------FORM VALIDATION---//

        //---start-------------------------------------------------------------------------------------SERVER COMMS---//

        private async void UserExistCheck()
        {
            UserDialogs.Instance.Loading(title: "Processing...").Show();
            var client = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(15)
            };

            JObject jsonDataObject = new JObject {
                { "email", emailForm.Text.Trim()}
            };

            JObject jsonData = new JObject {
                { "params", jsonDataObject }
            };

            var data = jsonData.ToString();
            var content = new StringContent(data, Encoding.UTF8, "application/json");
            Console.Debug.WriteLine("REQUEST-UserExistCheck: " + data);

            try
            {
                HttpResponseMessage response = await client.PostAsync(ApiUri.BASE_URL + ApiUri.CHECK_USER, content);
                string responseContent = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    UserDialogs.Instance.Loading().Hide();
                    Console.Debug.WriteLine("RESPONSE-UserExistCheck: " + responseContent);

                    UserExistCheckResponse userExistCheckResponse = JsonConvert.DeserializeObject<UserExistCheckResponse>(responseContent);
                    UserExistCheckResult userExistCheckResult = userExistCheckResponse.result;

                    if (userExistCheckResult.success)
                    {
                        if (!userExistCheckResult.user_exist)
                        {
                            User user = new User
                            {
                                email = emailForm.Text.Trim(),
                                name = firstNameForm.Text.Trim() + " " + lastNameForm.Text.Trim()
                            };

                            user.dbID = DBLocalID.USER_TEMP_SIGN_UP;

                            var realm = Realm.GetInstance();
                            realm.Write(() =>
                            {
                                realm.Add(user, update: true);
                            });

                            await Navigation.PushAsync(new SignUpNationalityPage(this));
                        }
                        else
                        {
                            var toastAction = new ToastAction();
                            toastAction.SetText("LOG IN");
                            toastAction.SetAction(OnToastActionLogin);

                            UserDialogs.Instance.Toast(
                                new ToastConfig("User already exists")
                                .SetAction(toastAction)
                            );
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
                    Console.Debug.WriteLine("RESPONSE_ERROR-UserExistCheck: " + responseContent);
                    Notifications.Internal.ServerError();
                }
            }
            catch (TaskCanceledException e)
            {
                UserDialogs.Instance.Toast(new ToastConfig("Bad Connection Error"));
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }

        //---end---------------------------------------------------------------------------------------SERVER COMMS---//
    }
}