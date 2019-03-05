using System.IO;
using TravellerApp.Interfaces;
using TravellerApp.iOS;
using Xamarin.Forms;
using ZXing;
using ZXing.QrCode.Internal;

[assembly: Dependency(typeof(QRCodeService))]

namespace TravellerApp.iOS
{
    public class QRCodeService : IBarcodeService
    {
        public Stream GenerateQR(string text, int width = 300, int height = 300)
        {
            var barcodeWriter = new ZXing.Mobile.BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new ZXing.Common.EncodingOptions
                {
                    Width = width,
                    Height = height,
                    Margin = 0,
                    PureBarcode = false
                }
            };
            barcodeWriter.Renderer = new ZXing.Mobile.BitmapRenderer();
            barcodeWriter.Options.Hints.Add(EncodeHintType.ERROR_CORRECTION, ErrorCorrectionLevel.H);
            var bitmap = barcodeWriter.Write(text);
            var stream = bitmap.AsPNG().AsStream(); // this is the difference
            stream.Position = 0;

            return stream;
        }
    }
}