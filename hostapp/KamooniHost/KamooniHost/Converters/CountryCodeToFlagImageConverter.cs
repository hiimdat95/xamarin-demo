using KamooniHost.Models;
using KamooniHost.Utils;
using System;
using System.Globalization;
using System.Linq;

namespace Xamarin.Forms.Converters
{
    public class CountryCodeToFlagImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (!(value is string CountryCode))
                    return null;

                if (CountryUtil.AllCountries.FirstOrDefault(c => c.Code == CountryCode || c.Name == CountryCode || c.DialCode == CountryCode || c.DialCode.Replace("+", "") == CountryCode.Replace("+", "")) is Country country)
                {
                    return country.FlagImage;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return default;
        }
    }
}