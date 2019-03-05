using KamooniHost.Constants;
using Plugin.SimpleAudioPlayer;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Extensions;
using ZXing;
using ZXing.Mobile;

namespace KamooniHost.Helpers
{
    public class BarcodeHelper
    {
        public static async Task StartScan(bool useFrontCamera = false, MobileBarcodeScanningOptions options = null)
        {
            try
            {
                var scanner = new MobileBarcodeScanner()
                {
                    TopText = "Scan Code"
                };
                scanner.ScanContinuously(options ?? new MobileBarcodeScanningOptions
                {
                    PossibleFormats = new List<BarcodeFormat>
                    {
                        BarcodeFormat.EAN_8,
                        BarcodeFormat.EAN_13,
                        BarcodeFormat.CODE_39,
                        BarcodeFormat.CODE_128,
                        BarcodeFormat.QR_CODE
                    },
                    AutoRotate = false,
                    UseFrontCameraIfAvailable = useFrontCamera,
                    TryHarder = true,
                    DisableAutofocus = false
                }, (result) =>
                {
                    CrossSimpleAudioPlayer.Current.Load("beep.wav");
                    CrossSimpleAudioPlayer.Current.Play();
                    MessagingCenter.Send(typeof(BarcodeHelper), MessageKey.BARCODE_SCANNED, result.Text);
                });

                Device.StartTimer(TimeSpan.FromSeconds(3), () =>
                {
                    scanner.AutoFocus();
                    return true;
                });
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert(TranslateExtension.GetValue("error"), ex.Message, TranslateExtension.GetValue("ok"));
            }
        }

        public async static Task<string> ScanSerial(bool useFrontCamera = false, MobileBarcodeScanningOptions options = null)
        {
            try
            {
                var result = await new MobileBarcodeScanner()
                {
                    TopText = "Scan Code"
                }.Scan(options ?? new MobileBarcodeScanningOptions
                {
                    PossibleFormats = new List<BarcodeFormat>
                    {
                        BarcodeFormat.EAN_8,
                        BarcodeFormat.EAN_13,
                        BarcodeFormat.CODE_39,
                        BarcodeFormat.CODE_128
                    },
                    AutoRotate = false,
                    UseFrontCameraIfAvailable = useFrontCamera,
                    TryHarder = true,
                    DisableAutofocus = false
                });

                if (result != null)
                {
                    CrossSimpleAudioPlayer.Current.Load("beep.wav");
                    CrossSimpleAudioPlayer.Current.Play();
                    return result.Text;
                }

                return null;
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert(TranslateExtension.GetValue("error"), ex.Message, TranslateExtension.GetValue("ok"));
                return null;
            }
        }

        public async static Task<string> ScanQRCode(bool useFrontCamera = false)
        {
            try
            {
                var result = await new MobileBarcodeScanner()
                {
                    TopText = "Scan QR"
                }.Scan(new MobileBarcodeScanningOptions
                {
                    PossibleFormats = new List<BarcodeFormat>
                    {
                        BarcodeFormat.QR_CODE
                    },
                    AutoRotate = false,
                    UseFrontCameraIfAvailable = useFrontCamera,
                    TryHarder = true,
                    DisableAutofocus = false
                });

                if (result != null)
                {
                    CrossSimpleAudioPlayer.Current.Load("beep.wav");
                    CrossSimpleAudioPlayer.Current.Play();
                    return result.Text;
                }

                return null;
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert(TranslateExtension.GetValue("error"), ex.Message, TranslateExtension.GetValue("ok"));
                return null;
            }
        }
    }
}