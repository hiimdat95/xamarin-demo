using KamooniHost.Helpers;
using KamooniHost.IServices;
using KamooniHost.Models;
using KamooniHost.Models.Control;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Extensions;
using Xamarin.Forms.TinyMVVM;

namespace KamooniHost.ViewModels
{
    internal class HostVerificationViewModel : TinyViewModel
    {
        private readonly ISignUpService SignUpService;

        public ButtonProperty AccommodationButton { get; set; } = new ButtonProperty();
        public ButtonProperty ActivityButton { get; set; } = new ButtonProperty();
        public ButtonProperty TransportButton { get; set; } = new ButtonProperty();

        private string validationError;
        public string ValidationError { get => validationError; set => SetProperty(ref validationError, value); }

        public ICommand MobileUnfocusedCommand { get; private set; }
        public ICommand BankNumberUnfocusedCommand { get; private set; }
        public ICommand BankCodeUnfocusedCommand { get; private set; }
        public ICommand AccommodationSelectedCommand { get; private set; }
        public ICommand ActivitySelectedCommand { get; private set; }
        public ICommand TransportSelectedCommand { get; private set; }
        public ICommand SelectLogoCommand { get; private set; }
        public ICommand BankLaterToggleCommand { get; private set; }
        public ICommand LogoLaterToggleCommand { get; private set; }
        public ICommand SubmitCommand { get; private set; }

        private HostToVerify Host = new HostToVerify();

        public bool _TypeSelected { get; set; } = false;
        public bool _LogoSelected { get; set; } = false;
        public bool BankLaterEnabled { get; set; } = false;
        public bool LogoLaterEnabled { get; set; } = false;

        public HostVerificationViewModel(ISignUpService SignUpService)
        {
            this.SignUpService = SignUpService;

            MobileUnfocusedCommand = new AwaitCommand<Entry>(OnMobileUnfocused);
            BankNumberUnfocusedCommand = new AwaitCommand<Entry>(OnMobileUnfocused);
            BankCodeUnfocusedCommand = new AwaitCommand<Entry>(OnMobileUnfocused);
            AccommodationSelectedCommand = new AwaitCommand(OnAccomodationSelected);
            ActivitySelectedCommand = new AwaitCommand(OnActivitySelected);
            TransportSelectedCommand = new AwaitCommand(OnTransportSelected);
            SelectLogoCommand = new AwaitCommand(SelectLogo);
            BankLaterToggleCommand = new AwaitCommand(OnBankLaterToggle);
            LogoLaterToggleCommand = new AwaitCommand(OnLogoLaterToggle);
            SubmitCommand = new AwaitCommand(Submit);
        }

        public override void OnPageCreated()
        {
            base.OnPageCreated();

            AccommodationButton.Deactive();
            ActivityButton.Deactive();
            TransportButton.Deactive();

            //CheckHostVerify(Settings.CurrentHost.Token);
        }

        private void OnMobileUnfocused(Entry entry, TaskCompletionSource<bool> tcs)
        {
            Host.Mobile = entry.Text.Trim();
            tcs.SetResult(true);
        }

        private void OnBankNumberUnfocused(Entry entry, TaskCompletionSource<bool> tcs)
        {
            Host.BankNumber = entry.Text.Trim();
            tcs.SetResult(true);
        }

        private void OnBankCodeUnfocused(Entry entry, TaskCompletionSource<bool> tcs)
        {
            Host.BankCode = entry.Text.Trim();
            tcs.SetResult(true);
        }

        private void OnAccomodationSelected(object sender, TaskCompletionSource<bool> tcs)
        {
            SelectAccomodation();
            tcs.SetResult(true);
        }

        private void OnActivitySelected(object sender, TaskCompletionSource<bool> tcs)
        {
            SelectActivity();
            tcs.SetResult(true);
        }

        private void OnTransportSelected(object sender, TaskCompletionSource<bool> tcs)
        {
            SelectTransport();
            tcs.SetResult(true);
        }

        private void SelectAccomodation()
        {
            AccommodationButton.Active();
            ActivityButton.Deactive();
            TransportButton.Deactive();

            Host.IsAccomodationCompany = true;
            Host.IsActivityCompany = false;
            Host.IsTransportCompany = false;

            _TypeSelected = true;
        }

        private void SelectActivity()
        {
            AccommodationButton.Deactive();
            ActivityButton.Active();
            TransportButton.Deactive();

            Host.IsAccomodationCompany = false;
            Host.IsActivityCompany = true;
            Host.IsTransportCompany = false;

            _TypeSelected = true;
        }

        private void SelectTransport()
        {
            AccommodationButton.Deactive();
            ActivityButton.Deactive();
            TransportButton.Active();

            Host.IsAccomodationCompany = false;
            Host.IsActivityCompany = false;
            Host.IsTransportCompany = true;

            _TypeSelected = true;
        }

        private void SelectLogo(object sender, TaskCompletionSource<bool> tcs)
        {
            tcs.SetResult(true);
        }

        private void OnBankLaterToggle(object sender, TaskCompletionSource<bool> tcs)
        {
            BankLaterEnabled = !BankLaterEnabled;
            tcs.SetResult(true);
        }

        private void OnLogoLaterToggle(object sender, TaskCompletionSource<bool> tcs)
        {
            LogoLaterEnabled = !LogoLaterEnabled;
            tcs.SetResult(true);
        }

        //---start----------------------------------------------------------------------------------FORM VALIDATION---//

        private bool AllFormsValid()
        {
            ValidationError = "";

            if (TypeSelected() && !MobileEmpty() && BankNumberValid() && BankCodeValid() && LogoValid())
            {
                return true;
            }
            return false;
        }

        private bool TypeSelected()
        {
            if (_TypeSelected)
            {
                return true;
            }
            else
            {
                ValidationError = "Please select Type";
                return false;
            }
        }

        private bool BankNumberValid()
        {
            if (!string.IsNullOrWhiteSpace(Host.BankNumber))
            {
                return true;
            }
            else
            {
                if (BankLaterEnabled)
                {
                    return true;
                }
                else
                {
                    ValidationError = "Please enter Account Number";
                    return false;
                }
            }
        }

        private bool BankCodeValid()
        {
            if (!string.IsNullOrWhiteSpace(Host.BankCode))
            {
                return true;
            }
            else
            {
                if (BankLaterEnabled)
                {
                    return true;
                }
                else
                {
                    ValidationError = "Please enter Branch Code";
                    return false;
                }
            }
        }

        private bool MobileEmpty()
        {
            if (string.IsNullOrWhiteSpace(Host.Mobile))
            {
                ValidationError = "Please enter Mobile Number";
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool LogoValid()
        {
            if (_LogoSelected)
            {
                return true;
            }
            else
            {
                if (LogoLaterEnabled)
                {
                    return true;
                }
                else
                {
                    ValidationError = "Please select Logo";
                    return false;
                }
            }
        }

        //---end------------------------------------------------------------------------------------FORM VALIDATION---//

        private void CheckHostVerify(string hostToken)
        {
            if (IsBusy || !ConnectivityHelper.IsNetworkAvailable())
            {
                return;
            }

            IsBusy = true;
            Task.Run(async () =>
            {
                return await SignUpService.CheckHostVerify(hostToken);
            }).ContinueWith(task => Device.BeginInvokeOnMainThread(() =>
            {
                IsBusy = false;
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    if (task.Result != null)
                    {
                        if (task.Result.Success)
                        {
                            Host = task.Result.HostToVerify;
                        }
                        else
                        {
                            if (task.Result.Message != null)
                            {
                                CoreMethods.DisplayAlert(TranslateExtension.GetValue("dialog_title_verification"), task.Result.Message, TranslateExtension.GetValue("ok"));
                            }
                            else
                            {
                                CoreMethods.DisplayAlert(TranslateExtension.GetValue("dialog_title_verification"), TranslateExtension.GetValue("dialog_message_server_error"), TranslateExtension.GetValue("ok"));
                            }
                        }
                    }
                    else
                    {
                        CoreMethods.DisplayAlert(TranslateExtension.GetValue("dialog_title_verification"), TranslateExtension.GetValue("dialog_message_server_error"), TranslateExtension.GetValue("ok"));
                    }
                }
                else if (task.IsFaulted)
                {
                    CoreMethods.DisplayAlert(TranslateExtension.GetValue("error"), task.Exception?.GetBaseException().Message, TranslateExtension.GetValue("ok"));
                }
            }));
        }

        private void Submit(object sender, TaskCompletionSource<bool> tcs)
        {
            if (AllFormsValid())
            {
                Debug.WriteLine("SCOPE: Valid");
            }
            else
            {
                Debug.WriteLine("SCOPE: InValid");
            }
            tcs.SetResult(true);
        }
    }
}