using Realms;
using Stormlion.ImageCropper;
using System;
using TravellerApp.Constants;
using TravellerApp.Helpers;
using TravellerApp.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TravellerApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignUpPassportPage : ContentPage
    {
        private const string screen = "SignUpPassportPage";

        public SignUpPassportPage()
        {
            InitializeComponent();
        }

        //---start-------------------------------------------------------------------------------------CLICK EVENTS---//

        private async void OnTakePhotoButton(object sender, EventArgs e)
        {
            var imageFile = await PhotoHelper.TakePhotoPathAsync(screen);

            if (imageFile == null)
                return;

            new ImageCropper()
            {
                PageTitle = "Passport Picture",
                AspectRatioX = 1,
                AspectRatioY = 1,
                CropShape = ImageCropper.CropShapeType.Rectangle,
                Success = (result) =>
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        passportPicture.Source = ImageSource.FromFile(result);

                        takePhotoButton.Text = "RETAKE PHOTO";
                        proceedButton.IsVisible = true;

                        AddBase64ImageToLocalDB(result);
                    });
                }
            }.Show(this, imageFile);
        }

        private async void OnProceedButton(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SignUpProfilePage());
        }

        //---end---------------------------------------------------------------------------------------CLICK EVENTS---//

        private async void AddBase64ImageToLocalDB(string imageFile)
        {
            byte[] imageBytes = await StorageHelper.LoadImage(imageFile);
            string imageBase64 = Convert.ToBase64String(imageBytes);

            var realm = Realm.GetInstance();
            User user = realm.Find<User>(DBLocalID.USER_TEMP_SIGN_UP);

            realm.Write(() =>
            {
                user.passport_pic = imageBase64;
                realm.Add(user, update: true);
            });
        }
    }
}