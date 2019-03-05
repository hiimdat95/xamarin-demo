using System;
using System.Globalization;
using System.IO;

namespace Xamarin.Forms.Converters
{
    public class Base64StringToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null || value is bool || (value is string rawString && (rawString.Equals("False") || rawString.Equals("false"))))
            {
                return null;
            }

            return ImageSource.FromStream(() => new MemoryStream(System.Convert.FromBase64String((string)value)));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}