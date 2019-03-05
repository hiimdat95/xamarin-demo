using System.IO;

namespace TravellerApp.Utils
{
    internal class General
    {
        public static string StreamToString(Stream s)
        {
            s.Position = 0;
            StreamReader reader = new StreamReader(s);
            string text = reader.ReadToEnd();

            return text;
        }
    }
}