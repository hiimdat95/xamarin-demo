using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TravellerApp.Helpers
{
    public static class PhotoHelper
    {
        public const string PROFILE = "PROFILE";
        public const string PASSPORT = "PASSPORT";

        public static async Task<bool> CheckPermissionsCamera()
        {
            var result = false;

            try
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
                if (status != PermissionStatus.Granted)
                {
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Camera))
                    {
                        await Application.Current.MainPage.DisplayAlert("Camera Permission", "Allow permissions to access your camera", "OK");
                    }

                    var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Camera });
                    status = results[Permission.Camera];
                }
                if (status == PermissionStatus.Granted)
                {
                    result = true;
                }
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Error", e.Message, "OK");
            }

            return result;
        }

        public static async Task<MediaFile> TakePhotoFileAsync()
        {
            MediaFile file = null;

            try
            {
                if (await CameraHelper.CanTakePhoto() && CrossMedia.Current.IsTakePhotoSupported)
                {
                    file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                    {
                        PhotoSize = PhotoSize.MaxWidthHeight,
                        MaxWidthHeight = 2000,
                        DefaultCamera = CameraDevice.Front,
                        CustomPhotoSize = 50,
                        SaveToAlbum = true,
                        CompressionQuality = 75,
                        Directory = "Kamooni",
                        Name = "profile.jpg"
                    });
                }
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Error", e.Message, "OK");
            }

            return file;
        }

        public static async Task<Stream> TakePhotoStreamAsync()
        {
            Stream result = null;
            try
            {
                if (await CameraHelper.CanTakePhoto() && CrossMedia.Current.IsTakePhotoSupported)
                {
                    var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                    {
                        PhotoSize = PhotoSize.MaxWidthHeight,
                        MaxWidthHeight = 2000,
                        DefaultCamera = CameraDevice.Front,
                        CustomPhotoSize = 50,
                        SaveToAlbum = true,
                        CompressionQuality = 75,
                        Directory = "Kamooni",
                        Name = "profile.jpg"
                    });

                    if (file == null)
                        return null;

                    result = file.GetStream();

                    file.Dispose();
                }
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Error", e.Message, "OK");
            }

            return result;
        }

        public static async Task<string> TakePhotoPathAsync(string screen)
        {
            var path = string.Empty;

            try
            {
                MediaFile file = null;

                if (await CameraHelper.CanTakePhoto() && CrossMedia.Current.IsTakePhotoSupported)
                {
                    if ("SignUpProfilePicturePage".Equals(screen) || "MyProfilePage".Equals(screen))
                    {
                        file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                        {
                            PhotoSize = PhotoSize.MaxWidthHeight,
                            MaxWidthHeight = 2000,
                            DefaultCamera = CameraDevice.Front,
                            CustomPhotoSize = 50,
                            SaveToAlbum = true,
                            CompressionQuality = 75,
                            Directory = "Kamooni",
                            Name = "profile.jpg"
                        });
                    }
                    else if ("SignUpPassportPage".Equals(screen))
                    {
                        file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                        {
                            PhotoSize = PhotoSize.MaxWidthHeight,
                            MaxWidthHeight = 2000,
                            DefaultCamera = CameraDevice.Rear,
                            CustomPhotoSize = 50,
                            SaveToAlbum = true,
                            CompressionQuality = 75,
                            Directory = "Kamooni",
                            Name = "profile.jpg"
                        });
                    }
                    else
                    {
                        file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                        {
                            PhotoSize = PhotoSize.MaxWidthHeight,
                            MaxWidthHeight = 2000,
                            DefaultCamera = CameraDevice.Front,
                            CustomPhotoSize = 50,
                            SaveToAlbum = true,
                            CompressionQuality = 75,
                            Directory = "Kamooni",
                            Name = "profile.jpg"
                        });
                    }
                }

                if (file == null)
                    return null;

                path = file.Path;
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Error", e.Message, "OK");
            }

            return path;
        }

        public static async Task<MediaFile> TakePhotoPassport()
        {
            MediaFile file = null;

            try
            {
                if (await CameraHelper.CanTakePhoto() && CrossMedia.Current.IsTakePhotoSupported)
                {
                    file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                    {
                        AllowCropping = true,
                        PhotoSize = PhotoSize.MaxWidthHeight,
                        MaxWidthHeight = 2000,
                        DefaultCamera = CameraDevice.Front,
                        CustomPhotoSize = 50,
                        SaveToAlbum = true,
                        CompressionQuality = 75,
                        Directory = "Kamooni",
                        Name = "profile.jpg"
                    });
                }
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Error", e.Message, "OK");
            }

            return file;
        }

        public static async Task<MediaFile> PickPhotoFileAsync()
        {
            MediaFile result = null;

            try
            {
                if (await CameraHelper.CanTakePhoto() && CrossMedia.Current.IsPickPhotoSupported)
                {
                    result = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
                    {
                        PhotoSize = PhotoSize.MaxWidthHeight
                    });
                }
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Error", e.Message, "OK");
            }

            return result;
        }

        public static async Task<Stream> PickPhotoStreamAsync()
        {
            Stream result = null;

            try
            {
                if (await CameraHelper.CanTakePhoto() && CrossMedia.Current.IsPickPhotoSupported)
                {
                    var file = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
                    {
                        PhotoSize = PhotoSize.MaxWidthHeight
                    });

                    if (file == null)
                        return null;

                    result = file.GetStream();

                    file.Dispose();
                }
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Error", e.Message, "OK");
            }

            return result;
        }

        public static async Task<Stream> TakeVideoStreamAsync()
        {
            Stream result = null;

            try
            {
                if (await CameraHelper.CanTakePhoto() && CrossMedia.Current.IsTakeVideoSupported)
                {
                    var file = await CrossMedia.Current.TakeVideoAsync(new StoreVideoOptions
                    {
                        Name = "video.mp4",
                        Directory = "DefaultVideos",
                    });

                    if (file == null)
                        return null;

                    result = file.GetStream();

                    file.Dispose();
                }
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Error", e.Message, "OK");
            }

            return result;
        }

        public static async Task<Stream> PicVideoStreamAsync()
        {
            Stream result = null;

            try
            {
                if (await CameraHelper.CanTakePhoto() && CrossMedia.Current.IsPickVideoSupported)
                {
                    var file = await CrossMedia.Current.PickVideoAsync();

                    if (file == null)
                        return null;

                    result = file.GetStream();

                    file.Dispose();
                }
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Error", e.Message, "OK");
            }

            return result;
        }
    }
}