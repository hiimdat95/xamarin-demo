using Xamarin.Forms;

namespace TravellerApp
{
    internal class VisualConstants
    {
        public static readonly Font StandardFont;

        static VisualConstants()
        {
            switch (Device.RuntimePlatform)
            {
                case Device.Android:
                    StandardFont = Font.SystemFontOfSize(14);
                    break;

                case Device.iOS:
                    StandardFont = Font.SystemFontOfSize(12);
                    break;

                case Device.UWP:
                    StandardFont = Font.SystemFontOfSize(14);
                    break;
            }
        }
    }
}