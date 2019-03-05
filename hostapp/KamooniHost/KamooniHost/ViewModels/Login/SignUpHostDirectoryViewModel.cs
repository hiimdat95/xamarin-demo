using Acr.UserDialogs;
using KamooniHost.Helpers;
using KamooniHost.IServices;
using KamooniHost.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Extensions;
using Xamarin.Forms.TinyMVVM;

namespace KamooniHost.ViewModels
{
    internal class SignUpHostDirectoryViewModel : TinyViewModel
    {
        private readonly ISignUpService SignUpService;

        private CancellationTokenSource cts;

        private List<HostDirectoryItem> hostDirectoryOriginal;
        private List<HostDirectoryItem> hostDirectory = new List<HostDirectoryItem>();
        public List<HostDirectoryItem> HostDirectory { get => hostDirectory; set => SetProperty(ref hostDirectory, value); }

        private string searchKey;
        public string SearchKey { get => searchKey; set => SetProperty(ref searchKey, value); }

        public ICommand SelectHostCommand { get; private set; }

        public SignUpHostDirectoryViewModel(ISignUpService SignUpService)
        {
            this.SignUpService = SignUpService;

            SelectHostCommand = new AwaitCommand<HostDirectoryItem>(SelectHost);
        }

        public override void OnPageCreated()
        {
            base.OnPageCreated();

            GetHostDirectory();

            this.WhenAny(OnSearchKeyChanged, vm => vm.SearchKey);

            Task.Run(async () =>
            {
                await Task.Delay(3500);
            }).ContinueWith(task => Device.BeginInvokeOnMainThread(() =>
            {
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    ShowCreateHostToast();
                }
            }));
        }

        public override void OnPopped()
        {
            base.OnPopped();
        }

        private void GetHostDirectory()
        {
            if (IsBusy || !ConnectivityHelper.IsNetworkAvailable())
            {
                return;
            }

            IsBusy = true;
            Task.Run(async () =>
            {
                return await SignUpService.GetHostDirectory();
            }).ContinueWith(task => Device.BeginInvokeOnMainThread(() =>
            {
                IsBusy = false;
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    if (task.Result != null)
                    {
                        if (task.Result.Success)
                        {
                            if (task.Result.Hosts != null)
                            {
                                hostDirectoryOriginal = task.Result.Hosts.OrderBy(b => b.Name).ToList();
                                HostDirectory = hostDirectoryOriginal;
                            }
                        }
                        else if (!string.IsNullOrWhiteSpace(task.Result.Message))
                        {
                            CoreMethods.DisplayAlert("", task.Result.Message, TranslateExtension.GetValue("ok"));
                        }
                    }
                    else
                    {
                        CoreMethods.DisplayAlert(TranslateExtension.GetValue("dialog_title_sign_up"), TranslateExtension.GetValue("dialog_message_server_error"), TranslateExtension.GetValue("ok"));
                    }
                }
                else if (task.IsFaulted)
                {
                    CoreMethods.DisplayAlert(TranslateExtension.GetValue("error"), task.Exception?.GetBaseException().Message, TranslateExtension.GetValue("ok"));
                }
            }));
        }

        private void OnSearchKeyChanged(string sender)
        {
            if (cts != null)
                cts.Cancel();

            cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;

            Task.Run(async () =>
            {
                await Task.Delay(3500, token);
            }, token).ContinueWith(task => Device.BeginInvokeOnMainThread(() =>
            {
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    ShowCreateHostToast();
                }
            }));

            Task.Run(async () =>
            {
                await Task.Delay(1500, token);

                if (string.IsNullOrWhiteSpace(SearchKey))
                    return hostDirectoryOriginal.ToList();
                else
                    return hostDirectoryOriginal.FindAll(b => b.Name.ToLower().Contains(SearchKey.ToLower()));
            }, token).ContinueWith(task => Device.BeginInvokeOnMainThread(() =>
            {
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    HostDirectory = task.Result;
                }
            }));
        }

        private void SelectHost(HostDirectoryItem selectedHost, TaskCompletionSource<bool> tcs)
        {
            if (IsBusy || !ConnectivityHelper.IsNetworkAvailable())
            {
                return;
            }

            IsBusy = true;
            Task.Run(async () =>
            {
                return await SignUpService.SendPortalInvite(selectedHost.Id);
            }).ContinueWith(task => Device.BeginInvokeOnMainThread(async () =>
            {
                IsBusy = false;
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    if (task.Result != null)
                    {
                        if (task.Result.Success)
                        {
                            var okPressed = await CoreMethods.DisplayAlert(TranslateExtension.GetValue("dialog_title_sign_up"), TranslateExtension.GetValue("dialog_message_host_selected"), "GOT IT", " ");

                            if (okPressed)
                            {
                                Application.Current.MainPage = new NavigationContainer(ViewModelResolver.ResolveViewModel<LoginViewModel>())
                                {
                                    BarBackgroundColor = Color.FromHex("#835e7e"),
                                    BarTextColor = Color.White
                                };
                            }
                            else
                            { // To accomodate if the blank cancel text is pressed
                                Application.Current.MainPage = new NavigationContainer(ViewModelResolver.ResolveViewModel<LoginViewModel>())
                                {
                                    BarBackgroundColor = Color.FromHex("#835e7e"),
                                    BarTextColor = Color.White
                                };
                            }
                        }
                        else if (!string.IsNullOrWhiteSpace(task.Result.Message))
                        {
                            await CoreMethods.DisplayAlert(TranslateExtension.GetValue("dialog_title_sign_up"), task.Result.Message, TranslateExtension.GetValue("ok"));
                        }
                        else
                        {
                            await CoreMethods.DisplayAlert(TranslateExtension.GetValue("dialog_title_sign_up"), TranslateExtension.GetValue("dialog_message_server_error"), TranslateExtension.GetValue("ok"));
                        }
                    }
                    else
                    {
                        await CoreMethods.DisplayAlert(TranslateExtension.GetValue("dialog_title_sign_up"), TranslateExtension.GetValue("dialog_message_server_error"), TranslateExtension.GetValue("ok"));
                    }
                }
                else if (task.IsFaulted)
                {
                    await CoreMethods.DisplayAlert(TranslateExtension.GetValue("error"), task.Exception?.GetBaseException().Message, TranslateExtension.GetValue("ok"));
                }

                tcs.SetResult(true);
            }));
        }

        private async void SearchHost(string sender, TaskCompletionSource<bool> tcs)
        {
            if (string.IsNullOrWhiteSpace(sender))
            {
                HostDirectory = hostDirectoryOriginal.ToList();
            }
            else
            {
                HostDirectory = hostDirectoryOriginal.FindAll(b => b.Name.ToLower().Contains(sender.ToLower()));
            }

            await Task.Delay(15000);
            ShowCreateHostToast();

            tcs.SetResult(true);
        }

        ////////////
        // TOASTS //
        ////////////

        private void ShowCreateHostToast()
        {
            ToastAction toastAction = new ToastAction()
            {
                Text = "SIGN UP",
                Action = CreateHost
            };

            UserDialogs.Instance.Toast(
                new ToastConfig("Can't find your property?")
                .SetDuration(TimeSpan.FromSeconds(5))
                .SetAction(toastAction)
            );
        }

        private void CreateHost()
        {
            CoreMethods.PushViewModel<SignUpCreateHostViewModel>();
        }
    }
}