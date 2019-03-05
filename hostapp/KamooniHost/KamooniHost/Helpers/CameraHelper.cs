using Plugin.Media;
using Plugin.Permissions.Abstractions;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Extensions;

namespace KamooniHost.Helpers
{
    public static class CameraHelper
    {
        public static async Task<bool> CheckCameraPermission()
        {
            return await PermissionsHelper.CheckPermissions(Permission.Camera);
        }

        public static async Task<bool> CanTakePhoto()
        {
            var cameraStatus = await PermissionsHelper.CheckPermissions(Permission.Camera);

            if (!cameraStatus)
            {
                await Application.Current.MainPage.DisplayAlert(Permission.Camera.ToString() + " Denied", "Unable to take photos.", TranslateExtension.GetValue("ok"));
                return false;
            }

            var storageStatus = await PermissionsHelper.CheckPermissions(Permission.Storage);

            if (!storageStatus)
            {
                await Application.Current.MainPage.DisplayAlert(Permission.Storage.ToString() + " Denied", "Unable to take photos.", TranslateExtension.GetValue("ok"));
                return false;
            }

            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable)
            {
                await Application.Current.MainPage.DisplayAlert("No Camera", "No camera available.", TranslateExtension.GetValue("ok"));
                return false;
            }

            return true;
        }
    }
}