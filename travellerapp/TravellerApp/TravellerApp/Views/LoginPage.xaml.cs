using Acr.UserDialogs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Realms;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using TravellerApp.Constants;
using TravellerApp.Interfaces;
using TravellerApp.Interfaces.ComectChatCallback;
using TravellerApp.Models;
using TravellerApp.Models.Response;
using TravellerApp.Models.Result;
using TravellerApp.Utils;
using Xamarin.Forms;

namespace TravellerApp.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();

            InitObjects();
        }

        private void InitObjects()
        {
            emailForm.Completed += (s, e) => passwordForm.Focus();

            forgotPasswordText.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => OnForgotPassword())
            });

            signUpText.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => OnSignUp())
            });
        }

        //---start-------------------------------------------------------------------------------------CLICK EVENTS---//

        private void OnLoginButton(object sender, EventArgs e)
        {
            validationError.Text = "";

            if (!EmailEmpty() && !PasswordEmpty())
            {
                if (EmailValid())
                {
                    var AuthService = DependencyService.Get<IAuthService>();
                    AuthService.SaveCredentials(emailForm.Text.ToLower().Trim(), StringFormatUtil.ToBase64(passwordForm.Text.Trim()));

                    UserExistCheck();
                }
            }
        }

        private async void OnSignUp()
        {
            await Navigation.PushAsync(new SignUpPage());
        }

        private void OnForgotPassword()
        {
            validationError.Text = "";

            if (!EmailEmpty())
            {
                if (EmailValid())
                {
                    TriggerForgotPassword();
                }
            }
        }

        private async void OnToastActionSignUp()
        {
            await Navigation.PushAsync(new SignUpPage());
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

        private bool PasswordEmpty()
        {
            if (string.IsNullOrWhiteSpace(passwordForm.Text))
            {
                validationError.Text = "Please enter Password";
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
                Debug.WriteLine(e.ToString());

                validationError.Text = "Email is invalid";
                valid = false;
            }

            return valid;
        }

        //---end------------------------------------------------------------------------------------FORM VALIDATION---//

        //---start-------------------------------------------------------------------------------------SERVER COMMS---//

        private async void UserExistCheck()
        {
            UserDialogs.Instance.Loading(title: "Finding User...").Show();

            var client = new HttpClient()
            {
                Timeout = TimeSpan.FromSeconds(20)
            };

            JObject jsonDataObject = new JObject {
                { "email", emailForm.Text.Trim()}
            };

            JObject jsonData = new JObject {
                { "params", jsonDataObject }
            };

            var data = jsonData.ToString();
            var content = new StringContent(data, Encoding.UTF8, "application/json");
            Debug.WriteLine("REQUEST-UserExistCheck: " + data);

            try
            {
                HttpResponseMessage response = await client.PostAsync(ApiUri.BASE_URL + ApiUri.CHECK_USER, content);
                string responseContent = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Debug.WriteLine("RESPONSE-UserExistCheck: " + responseContent);

                    UserExistCheckResponse userExistCheckResponse = JsonConvert.DeserializeObject<UserExistCheckResponse>(responseContent, App.DefaultSerializerSettings);
                    UserExistCheckResult userExistCheckResult = userExistCheckResponse.result;

                    if (userExistCheckResult.success)
                    {
                        if (userExistCheckResult.user_exist)
                        {
                            UserDialogs.Instance.Loading(title: "Logging in...").Show();
                            // Login sever.
                            ServerLogin();
                        }
                        else
                        {
                            UserDialogs.Instance.Loading().Hide();

                            var toastAction = new ToastAction();
                            toastAction.SetText("SIGN UP");
                            toastAction.SetAction(OnToastActionSignUp);

                            UserDialogs.Instance.Toast(
                                new ToastConfig("User doesn't exist")
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
                    Debug.WriteLine("RESPONSE_ERROR-UserExistCheck: " + responseContent);
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

        private void initCometChatSuccess(string success)
        {
            if (success != null)
            {
                System.Console.WriteLine("initCometChatSuccess" + success.ToString());
                User User = Realm.GetInstance().Find<User>(DBLocalID.USER);
                // Login comet chat.
                DependencyService.Get<ICometChatService>().loginWithUID(User.traveller_token, new Callbacks(successlogin => loginCometChatSuccess(successlogin), fail => loginCometChatFail(fail)));
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

        private void loginCometChatSuccess(string success)
        {
            if (success != null)
            {
                System.Console.WriteLine("loginCometChatSuccess" + success.ToString());
            }
        }

        private void loginCometChatFail(string fail)
        {
            if (fail != null)
            {
                System.Console.WriteLine("loginCometChatSuccess" + fail.ToString());
            }
        }

        private async void ServerLogin()
        {
            var client = new HttpClient()
            {
                Timeout = TimeSpan.FromSeconds(20)
            };

            var AuthService = DependencyService.Get<IAuthService>();

            JObject jsonAuth = new JObject {
                { "db", ServerAuth.DB },
                { "login", AuthService.UserName },
                { "password", AuthService.Password }
            };

            JObject jsonDataObject = new JObject {
                { "auth", jsonAuth }
            };

            JObject jsonData = new JObject {
                { "params", jsonDataObject }
            };

            var data = jsonData.ToString();
            var content = new StringContent(data, Encoding.UTF8, "application/json");
            Debug.WriteLine("REQUEST-ServerLogin: " + data);

            try
            {
                HttpResponseMessage response = await client.PostAsync(ApiUri.BASE_URL + ApiUri.SIGN_IN, content);
                string responseContent = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Debug.WriteLine("RESPONSE-ServerLogin: " + responseContent);

                    UserResponse userResponse = JsonConvert.DeserializeObject<UserResponse>(responseContent, App.DefaultSerializerSettings);
                    UserResult userResult = userResponse.result;

                    if (userResult.success)
                    {
                        User user = userResult.traveller;
                        user.dbID = DBLocalID.USER;

                        var realm = Realm.GetInstance();
                        realm.Write(() =>
                        {
                            realm.RemoveAll<User>();
                            realm.Add(user);
                        });

                        AppSettings.LoggedIn = true;

                        //calll premind start
                        AuthService.onLoginSuccess(user.traveller_token);

                        /*if (DependencyService.Get<ICometChatService>().isCometChaInitialize())
                        {
                            // Login comet chat with traveller_token.
                            DependencyService.Get<ICometChatService>().loginWithUID(user.traveller_token, new Callbacks(successlogin => loginCometChatSuccess(successlogin), fail => loginCometChatFail(fail)));
                        }
                        else
                        {
                            // Init comet chat.
                            DependencyService.Get<ICometChatService>().initializeCometChat(CometChatConstants.siteurl, CometChatConstants.licenseKey, CometChatConstants.apiKey, CometChatConstants.isCometOnDemand, new Callbacks(successinit => initCometChatSuccess(successinit), fail => initCometChatFail(fail)));
                        }*/

                        UserDialogs.Instance.Loading().Hide();

                        Application.Current.MainPage = new MainNavigation();
                    }
                    else
                    {
                        UserDialogs.Instance.Toast(new ToastConfig("Wrong Password"));
                    }
                    UserDialogs.Instance.Loading().Hide();
                }
                else
                {
                    UserDialogs.Instance.Loading().Hide();
                    Debug.WriteLine("RESPONSE_ERROR-ServerLogin: " + responseContent);
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

        private async void TriggerForgotPassword()
        {
            UserDialogs.Instance.Loading("Processing...").Show();

            var client = new HttpClient()
            {
                Timeout = TimeSpan.FromSeconds(20)
            };

            JObject jsonDataObject = new JObject {
                { "email", emailForm.Text.Trim()}
            };

            JObject jsonData = new JObject {
                { "params", jsonDataObject }
            };

            var data = jsonData.ToString();
            var content = new StringContent(data, Encoding.UTF8, "application/json");
            Debug.WriteLine("REQUEST-ForgotPassword: " + data);

            try
            {
                HttpResponseMessage response = await client.PostAsync(ApiUri.BASE_URL + ApiUri.FORGOT_PASSWORD, content);
                string responseContent = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    UserDialogs.Instance.Loading().Hide();
                    Debug.WriteLine("RESPONSE-ForgotPassword: " + responseContent);

                    SuccessCheckResponse successCheckResponse = JsonConvert.DeserializeObject<SuccessCheckResponse>(responseContent, App.DefaultSerializerSettings);
                    SuccessCheckResult successCheckResult = successCheckResponse.result;

                    if (successCheckResult.success)
                    {
                        UserDialogs.Instance.Toast(new ToastConfig("Password reset link sent to email"));
                    }
                    else
                    {
                        Notifications.Internal.ServerError();
                    }
                }
                else
                {
                    UserDialogs.Instance.Loading().Hide();
                    Debug.WriteLine("RESPONSE_ERROR-ForgotPassword: " + responseContent);
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