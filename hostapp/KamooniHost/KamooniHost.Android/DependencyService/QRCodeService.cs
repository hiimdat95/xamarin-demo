using Android.Graphics;
using KamooniHost.Droid;
using KamooniHost.IDependencyServices;
using System.IO;
using Xamarin.Forms;
using ZXing;
using ZXing.QrCode.Internal;

[assembly: Dependency(typeof(QRCodeService))]

namespace KamooniHost.Droid
{
    internal class QRCodeService : IQRCodeService
    {
        public Stream GenerateQR(string text, int width, int height)
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
                },
                Renderer = new ZXing.Mobile.BitmapRenderer()
            };
            barcodeWriter.Options.Hints.Add(EncodeHintType.ERROR_CORRECTION, ErrorCorrectionLevel.H);
            var bitmap = barcodeWriter.Write(text);
            var stream = new MemoryStream();

            bitmap.Compress(Bitmap.CompressFormat.Png, 100, stream);
            stream.Position = 0;

            return stream;
        }
    }
}