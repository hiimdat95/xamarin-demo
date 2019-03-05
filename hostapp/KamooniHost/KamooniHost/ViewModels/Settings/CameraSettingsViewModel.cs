using KamooniHost.Models;
using Xamarin.Forms.TinyMVVM;

namespace KamooniHost.ViewModels.Settings
{
    public class CameraSettingsViewModel : TinyViewModel
    {
        public AppSettings AppSettings { get; set; } = Helpers.Settings.AppSettings;

        public CameraSettingsViewModel()
        {
        }

        public override void OnPopped()
        {
            base.OnPopped();

            Helpers.Settings.AppSettings = AppSettings;
        }
    }
}