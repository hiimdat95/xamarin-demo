using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Extensions;

namespace KamooniHost.Helpers
{
    public static class PhotoHelper
    {
        public static async Task<Stream> TakePhotoStreamAsync(string imageName = null)
        {
            try
            {
                Stream result = null;

                if (await CameraHelper.CanTakePhoto())
                {
                    var name = imageName ?? ("IMG_" + DateTime.Now.ToString("yyyyMMddhhmmss"));

                    var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                    {
                        PhotoSize = PhotoSize.MaxWidthHeight,
                        MaxWidthHeight = 1600,
                        DefaultCamera = CameraDevice.Rear,
                        SaveToAlbum = false,
                        CompressionQuality = 85,
                        Directory = "Kamooni",
                        Name = name
                    });

                    if (file == null)
                        return null;

                    result = file.GetStream();

                    file.Dispose();
                }

                return result;
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert(TranslateExtension.GetValue("error"), e.GetBaseException().Message, TranslateExtension.GetValue("ok"));
                return null;
            }
        }

        public static async Task<string> TakePhotoPathAsync(string imageName = null)
        {
            try
            {
                var path = string.Empty;

                if (await CameraHelper.CanTakePhoto())
                {
                    var name = imageName ?? ("IMG_" + DateTime.Now.ToString("yyyyMMddhhmmss"));
                    MediaFile file = null;

                    file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                    {
                        PhotoSize = PhotoSize.MaxWidthHeight,
                        MaxWidthHeight = 1600,
                        DefaultCamera = CameraDevice.Rear,
                        SaveToAlbum = false,
                        CompressionQuality = 85,
                        Directory = "Kamooni",
                        Name = name
                    });

                    if (file == null)
                        return null;

                    path = file.Path;

                    //file.Dispose();
                }

                return path;
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert(TranslateExtension.GetValue("error"), e.GetBaseException().Message, TranslateExtension.GetValue("ok"));
                return null;
            }
        }

        public static async Task<Stream> PickPhotoStreamAsync()
        {
            try
            {
                Stream result = null;

                if (await CameraHelper.CanTakePhoto())
                {
                    var file = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
                    {
                        PhotoSize = PhotoSize.MaxWidthHeight,
                        MaxWidthHeight = 1600,
                        CompressionQuality = 85
                    });

                    if (file == null)
                        return null;

                    result = file.GetStream();

                    file.Dispose();
                }

                return result;
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert(TranslateExtension.GetValue("error"), e.GetBaseException().Message, TranslateExtension.GetValue("ok"));
                return null;
            }
        }

        public static async Task<string> PickPhotoPathAsync()
        {
            try
            {
                var path = string.Empty;

                if (await CameraHelper.CanTakePhoto())
                {
                    var file = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
                    {
                        PhotoSize = PhotoSize.MaxWidthHeight,
                        MaxWidthHeight = 1600,
                        CompressionQuality = 85
                    });

                    if (file == null)
                        return null;

                    path = file.Path;

                    //file.Dispose();
                }

                return path;
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert(TranslateExtension.GetValue("error"), e.GetBaseException().Message, TranslateExtension.GetValue("ok"));
                return null;
            }
        }

        public static async Task<Stream> TakeVideoStreamAsync(string videoName = null)
        {
            try
            {
                Stream stream = null;

                if (await CameraHelper.CanTakePhoto())
                {
                    var name = videoName ?? ("VID_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".mp4");

                    var file = await CrossMedia.Current.TakeVideoAsync(new StoreVideoOptions
                    {
                        Directory = "Videos",
                        Name = name
                    });

                    if (file == null)
                        return null;

                    stream = file.GetStream();

                    file.Dispose();
                }

                return stream;
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert(TranslateExtension.GetValue("error"), e.GetBaseException().Message, TranslateExtension.GetValue("ok"));
                return null;
            }
        }

        public static async Task<Stream> PickVideoStreamAsync()
        {
            try
            {
                Stream stream = null;

                if (await CameraHelper.CanTakePhoto())
                {
                    var file = await CrossMedia.Current.PickVideoAsync();

                    if (file == null)
                        return null;

                    stream = file.GetStream();

                    file.Dispose();
                }

                return stream;
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert(TranslateExtension.GetValue("error"), e.GetBaseException().Message, TranslateExtension.GetValue("ok"));
                return null;
            }
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
    }
}