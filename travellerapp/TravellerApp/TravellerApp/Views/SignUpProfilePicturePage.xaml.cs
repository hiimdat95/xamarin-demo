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
    public partial class SignUpProfilePicturePage : ContentPage
    {
        private const string screen = "SignUpProfilePicturePage";

        public SignUpProfilePicturePage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, true);
        }

        //---start-------------------------------------------------------------------------------------CLICK EVENTS---//

        private async void OnTakePhotoButton(object sender, EventArgs e)
        {
            var imageFile = await PhotoHelper.TakePhotoPathAsync(screen);

            if (imageFile == null)
                return;

            new ImageCropper()
            {
                PageTitle = "Profile Picture",
                AspectRatioX = 1,
                AspectRatioY = 1,
                CropShape = ImageCropper.CropShapeType.Oval,
                Success = (result) =>
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        profilePicture.Source = ImageSource.FromFile(result);

                        takePhotoButton.Text = "RETAKE PHOTO";
                        proceedButton.IsVisible = true;

                        AddBase64ImageToLocalDB(result);
                    });
                }
            }.Show(this, imageFile);
        }

        private async void OnProceedButton(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SignUpPassportPage());
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
                user.profile_pic = imageBase64;
                realm.Add(user, update: true);
            });
        }
    }
}