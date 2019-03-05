using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms.TinyMVVM;

namespace KamooniHost.ViewModels.Settings
{

    public class AddNewOrUpdatePINViewModel : TinyViewModel
    {
        public ICommand CancelTask { get; private set; }
        public ICommand UpdateNewPIN { get; private set; }
        public string PIN = Helpers.Settings.PIN;
        public string tempPIN { get; set; }
        public string tempConfirmPIN { get; set; }
        public string title { get; set; }
        public AddNewOrUpdatePINViewModel()
        {
            CancelTask = new AwaitCommand(cancelSetPIN);
            UpdateNewPIN = new AwaitCommand(UpdatePIN);
        }
        public override void Init(object data)
        {
            base.Init(data);
            //TODO 
            tempPIN = Helpers.Settings.PIN;
            tempConfirmPIN = Helpers.Settings.PIN;

        }
        private async void cancelSetPIN(object sender, TaskCompletionSource<bool> tcs)
        {
            await CoreMethods.PopViewModel();
            tcs.SetResult(true);
        }
        private async void UpdatePIN(object sender, TaskCompletionSource<bool> tcs)
        {
            if (string.IsNullOrEmpty(tempPIN) || string.IsNullOrEmpty(tempConfirmPIN) || !tempPIN.Equals(tempConfirmPIN))
            {
                if (string.IsNullOrEmpty(tempPIN))
                {
                    await CoreMethods.DisplayAlert("Fail", "New PIN cannot be empty ", "OK");
                    tcs.SetResult(true);
                    return;
                }
                if (string.IsNullOrEmpty(tempConfirmPIN))
                {
                    await CoreMethods.DisplayAlert("Fail", "Confirmation PIN cannot be empty ", "OK");
                    tcs.SetResult(true);
                    return;
                }
                if (!tempPIN.Equals(tempConfirmPIN))
                {
                    await CoreMethods.DisplayAlert("Fail", "Confirm new PIN is not matching ", "OK");
                    tcs.SetResult(true);
                    return;
                }

            }
            Helpers.Settings.PIN = tempConfirmPIN;
            await CoreMethods.DisplayAlert("Successed", "You just created a new PIN", "OK");
            await CoreMethods.PopViewModel();
            tcs.SetResult(true);
        }

    }
}
