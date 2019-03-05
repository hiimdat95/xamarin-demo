using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using ZXing;
using ZXing.Mobile;

namespace TravellerApp.Helpers
{
    public class BarcodeHelper
    {
        private static readonly Lazy<BarcodeHelper> lazy = new Lazy<BarcodeHelper>(() => new BarcodeHelper());

        public static BarcodeHelper Instance => lazy.Value;

        private BarcodeHelper()
        {
        }

        public event EventHandler<string> BarcodeScanned;

        public async Task StartScan(bool useFrontCamera = false, MobileBarcodeScanningOptions options = null)
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
                    BarcodeScanned?.Invoke(this, result.Text);
                    MessagingCenter.Send(this, Constants.BARCODE_SCANNED, result.Text);
                });

                Device.StartTimer(TimeSpan.FromSeconds(3), () =>
                {
                    scanner.AutoFocus();
                    return true;
                });
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "Ok");
            }
        }

        public async Task<string> ScanSerial(bool useFrontCamera = false, MobileBarcodeScanningOptions options = null)
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
                    return result.Text;
                }

                return null;
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "Ok");
                return null;
            }
        }

        public async Task<string> ScanQRCode(bool useFrontCamera = false)
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
                    return result.Text;
                }

                return null;
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "Ok");
                return null;
            }
        }
    }
}