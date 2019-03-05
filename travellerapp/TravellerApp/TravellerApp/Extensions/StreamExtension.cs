using System;
using System.IO;

namespace Xamarin.Forms.Extensions
{
    public static class StreamExtension
    {
        public static string ToString(this Stream s)
        {
            s.Position = 0;
            StreamReader reader = new StreamReader(s);
            string text = reader.ReadToEnd();

            return text;
        }

        public static string ConvertImageToBase64String(this Stream image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.CopyTo(ms);
                return Convert.ToBase64String(ms.ToArray());
            }
        }
    }
}