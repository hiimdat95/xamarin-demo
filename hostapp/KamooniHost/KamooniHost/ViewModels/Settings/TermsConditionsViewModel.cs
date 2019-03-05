using KamooniHost.Helpers;
using KamooniHost.IServices;
using KamooniHost.Models;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Extensions;
using Xamarin.Forms.TinyMVVM;

namespace KamooniHost.ViewModels
{
    public class TermsConditionsViewModel : TinyViewModel
    {
        private readonly ITermsConditionsService termsConditionsService;

        public Host Host { get; set; } = Helpers.Settings.CurrentHost;

        public ICommand SaveNewTermCommand { get; private set; }

        public TermsConditionsViewModel(ITermsConditionsService termsConditionsService)
        {
            this.termsConditionsService = termsConditionsService;

            SaveNewTermCommand = new Command(NewTermsConditions);
        }

        private void NewTermsConditions(object sender)
        {
            if (IsBusy || !ConnectivityHelper.IsNetworkAvailable())
            {
                return;
            }

            Task.Run(async () =>
            {
                return await termsConditionsService.NewTermsConditions(Host);
            }).ContinueWith(task => Device.BeginInvokeOnMainThread(() =>
            {
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    if (task.Result != null)
                    {
                        if (task.Result.Success)
                        {
                            CoreMethods.DisplayAlert("Update Successfully", task.Result.Message, TranslateExtension.GetValue("ok"));
                            CoreMethods.PopViewModel();
                        }
                        else
                        {
                            CoreMethods.DisplayAlert("Update Unsuccessfully", task.Result.Message, TranslateExtension.GetValue("ok"));
                        }
                    }
                    else
                    {
                        CoreMethods.DisplayAlert("Update Unsuccessfully", TranslateExtension.GetValue("dialog_message_server_error"), TranslateExtension.GetValue("ok"));
                    }
                }
                else if (task.IsFaulted)
                {
                    CoreMethods.DisplayAlert(TranslateExtension.GetValue("error"), task.Exception?.GetBaseException().Message, TranslateExtension.GetValue("ok"));
                }
            }));
        }
    }
}