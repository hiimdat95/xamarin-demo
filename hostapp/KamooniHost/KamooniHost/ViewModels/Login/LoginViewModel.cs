using Acr.UserDialogs;
using KamooniHost.Helpers;
using KamooniHost.IServices;
using KamooniHost.Models;
using Premind;
using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Extensions;
using Xamarin.Forms.TinyMVVM;

namespace KamooniHost.ViewModels
{
    public class LoginViewModel : TinyViewModel
    {
        private readonly ILoginService loginService;

        public string email;
        public string Email { get => email; set => SetProperty(ref email, value); }

        public string password;
        public string Password { get => password; set => SetProperty(ref password, value); }

        public string validationError;
        public string ValidationError { get => validationError; set => SetProperty(ref validationError, value); }

        public ICommand LoginCommand { get; private set; }
        public ICommand SignUpCommand { get; private set; }
        public ICommand ForgotPasswordCommand { get; private set; }
        public ICommand ScanLoginCommand { get; private set; }

        public LoginViewModel(ILoginService loginService)
        {
            this.loginService = loginService;

            LoginCommand = new AwaitCommand<Entry>(LoginTrigger);
            SignUpCommand = new AwaitCommand<Entry>(SignUp);
            ForgotPasswordCommand = new AwaitCommand<Entry>(ForgotPassword);
            ScanLoginCommand = new AwaitCommand<Entry>(ScanLogin);
        }

        private void LoginTrigger(object sender, TaskCompletionSource<bool> tcs)
        {
            if (!AllFormsValid() || IsBusy || !ConnectivityHelper.IsNetworkAvailable())
            {
                tcs.SetResult(true);
                return;
            }

            IsBusy = true;

            Task.Run(async () =>
            {
                return await loginService.CheckIfExist(Email);
            }).ContinueWith(task => Device.BeginInvokeOnMainThread(() =>
            {
                IsBusy = false;

                if (task.Status == TaskStatus.RanToCompletion)
                {
                    if (task.Result != null)
                    {
                        if (task.Result.Success)
                        {
                            if (task.Result.UserExists)
                            {
                                Login();
                            }
                            else
                            {
                                UserDialogs.Instance.Toast(new ToastConfig("User with this email does not exist").SetAction(new ToastAction()
                                {
                                    Text = "SIGN UP",
                                    Action = () => CoreMethods.PushViewModel<SignUpCreateHostViewModel>()
                                }));
                            }
                        }
                        else if (!string.IsNullOrWhiteSpace(task.Result.Message))
                        {
                            CoreMethods.DisplayAlert(TranslateExtension.GetValue("dialog_title_login"), task.Result.Message, TranslateExtension.GetValue("ok"));
                        }
                        else
                        {
                            CoreMethods.DisplayAlert(TranslateExtension.GetValue("dialog_title_login"), TranslateExtension.GetValue("dialog_message_server_error"), TranslateExtension.GetValue("ok"));
                        }
                    }
                    else
                    {
                        CoreMethods.DisplayAlert(TranslateExtension.GetValue("dialog_title_login"), TranslateExtension.GetValue("dialog_message_server_error"), TranslateExtension.GetValue("ok"));
                    }
                }
                else if (task.IsFaulted)
                {
                    CoreMethods.DisplayAlert(TranslateExtension.GetValue("error"), task.Exception?.GetBaseException().Message, TranslateExtension.GetValue("ok"));
                }

                tcs.SetResult(true);
            }));
        }

        private void Login()
        {
            if (IsBusy || !ConnectivityHelper.IsNetworkAvailable())
            {
                return;
            }

            IsBusy = true;

            Task.Run(async () =>
            {
                return await loginService.Login(Email, Password);
            }).ContinueWith(task => Device.BeginInvokeOnMainThread(() =>
            {
                IsBusy = false;

                if (task.Status == TaskStatus.RanToCompletion)
                {
                    if (task.Result != null)
                    {
                        if (task.Result.Success && task.Result.Host != null)
                        {
                            if (!string.IsNullOrWhiteSpace(task.Result.Host.PremindPassword))
                            {
                                PremindLogin(task.Result.Host);
                            }

                            OnLogInSuccess(task.Result.Host);
                        }
                        else
                        {
                            UserDialogs.Instance.Toast(new ToastConfig("Password Invalid"));
                        }
                    }
                    else
                    {
                        CoreMethods.DisplayAlert(TranslateExtension.GetValue("dialog_title_login"), TranslateExtension.GetValue("dialog_message_server_error"), TranslateExtension.GetValue("ok"));
                    }
                }
                else if (task.IsFaulted)
                {
                    CoreMethods.DisplayAlert(TranslateExtension.GetValue("error"), task.Exception?.GetBaseException().Message, TranslateExtension.GetValue("ok"));
                }
            }));
        }

        private void PremindLogin(Host host)
        {
            Task.Run(async () =>
            {
                return await loginService.PremindLogin(host.Email, host.PremindPassword);
            }).ContinueWith(task => Device.BeginInvokeOnMainThread(async () =>
            {
                IsBusy = false;

                if (task.Status == TaskStatus.RanToCompletion)
                {
                    if (!string.IsNullOrWhiteSpace(task.Result))
                    {
                        Helpers.Settings.PremindUri = task.Result;

                        await PremindClient.StartClient();
                    }
                }
                else if (task.IsFaulted)
                {
                    await CoreMethods.DisplayAlert(TranslateExtension.GetValue("error"), task.Exception?.GetBaseException().Message, TranslateExtension.GetValue("ok"));
                }
            }));
        }

        private void OnLogInSuccess(Host host)
        {
            if (Helpers.Settings.ListHost.Find(h => h.Id == host.Id) is Host result)
            {
                Helpers.Settings.ListHost.Remove(result);
            }

            Helpers.Settings.ListHost.Add(host);
            Helpers.Settings.CurrentHost = host;
            Helpers.Settings.Password = password;
            Helpers.Settings.Email = email;

            Helpers.Settings.HostID = host.Id;
            Helpers.Settings.LoggedIn = true;

            Application.Current.MainPage = new MainPage();
        }

        private async void SignUp(object sender, TaskCompletionSource<bool> tcs)
        {
            await CoreMethods.PushViewModel<SignUpCreateHostViewModel>();

            tcs.SetResult(true);
        }

        private void ForgotPassword(object sender, TaskCompletionSource<bool> tcs)
        {
            ValidationError = "";

            if (EmailEmpty())
            {
                tcs.SetResult(true);
                return;
            }

            if (IsBusy || !ConnectivityHelper.IsNetworkAvailable())
            {
                return;
            }

            IsBusy = true;

            Task.Run(async () =>
            {
                return await loginService.ForgotPassword(Email);
            }).ContinueWith(task => Device.BeginInvokeOnMainThread(() =>
            {
                IsBusy = false;

                if (task.Status == TaskStatus.RanToCompletion)
                {
                    if (task.Result != null)
                    {
                        if (task.Result.Success)
                        {
                            UserDialogs.Instance.Toast(new ToastConfig("Password reset link sent to email"));
                        }
                        else if (!string.IsNullOrWhiteSpace(task.Result.Message))
                        {
                            UserDialogs.Instance.Toast(new ToastConfig(task.Result.Message));
                        }
                        else
                        {
                            CoreMethods.DisplayAlert(TranslateExtension.GetValue("dialog_title_forgot_password"), TranslateExtension.GetValue("dialog_message_server_error"), TranslateExtension.GetValue("ok"));
                        }
                    }
                    else
                    {
                        CoreMethods.DisplayAlert(TranslateExtension.GetValue("dialog_title_forgot_password"), TranslateExtension.GetValue("dialog_message_server_error"), TranslateExtension.GetValue("ok"));
                    }
                }
                else if (task.IsFaulted)
                {
                    CoreMethods.DisplayAlert(TranslateExtension.GetValue("error"), task.Exception?.GetBaseException().Message, TranslateExtension.GetValue("ok"));
                }

                tcs.SetResult(true);
            }));
        }

        private async void ScanLogin(object sender, TaskCompletionSource<bool> tcs)
        {
            await CoreMethods.PushViewModel<SignInDescriptionViewModel>();

            tcs.SetResult(true);
        }

        //---start----------------------------------------------------------------------------------FORM VALIDATION---//

        private bool AllFormsValid()
        {
            ValidationError = "";

            if (!EmailEmpty() && !PasswordEmpty() && EmailValid())
            {
                return true;
            }
            return false;
        }

        private bool EmailEmpty()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                ValidationError = "Please enter Email";
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool PasswordEmpty()
        {
            if (string.IsNullOrWhiteSpace(Password))
            {
                ValidationError = "Please enter Password";
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
                new MailAddress(Email.Trim());
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());

                ValidationError = "Email is invalid";
                valid = false;
            }

            return valid;
        }

        //---end------------------------------------------------------------------------------------FORM VALIDATION---//
    }
}