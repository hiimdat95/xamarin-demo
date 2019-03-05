using KamooniHost.IDependencyServices;
using KamooniHost.Models;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Extensions;
using Xamarin.Forms.TinyMVVM;

namespace KamooniHost.ViewModels
{
    public class HomeViewModel : TinyViewModel
    {
        public Host Host => Helpers.Settings.CurrentHost;

        private Stream qrCode;
        public Stream QrCode { get => qrCode; set => SetProperty(ref qrCode, value); }

        public ICommand LogOutCommand { get; private set; }

        public HomeViewModel()
        {
            LogOutCommand = new AwaitCommand(LogOut);
        }

        public override void Init(object data)
        {
            base.Init(data);

            try
            {
                QrCode = DependencyService.Get<IQRCodeService>().GenerateQR(Host.Token);
            }
            catch { }
        }

        private async void LogOut(object sender, TaskCompletionSource<bool> tcs)
        {
            if (await CoreMethods.DisplayAlert(TranslateExtension.GetValue("dialog_title_logout"), TranslateExtension.GetValue("dialog_message_logout"), TranslateExtension.GetValue("yes"), TranslateExtension.GetValue("no")))
            {
                Helpers.Settings.LoggedIn = false;

                Application.Current.MainPage = new NavigationContainer(ViewModelResolver.ResolveViewModel<LoginViewModel>())
                {
                    BarBackgroundColor = Color.FromHex("#835e7e"),
                    BarTextColor = Color.White
                };
            }

            tcs.SetResult(true);
        }
    }
}