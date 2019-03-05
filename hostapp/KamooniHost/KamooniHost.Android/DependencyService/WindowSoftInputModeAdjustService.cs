using KamooniHost.Droid.DependencyService;
using KamooniHost.IDependencyServices;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

[assembly: Xamarin.Forms.Dependency(typeof(WindowSoftInputModeAdjustService))]

namespace KamooniHost.Droid.DependencyService
{
    public class WindowSoftInputModeAdjustService : IWindowSoftInputModeAdjust
    {
        public void UseWindowSoftInputModeAdjust(IDependencyServices.WindowSoftInputModeAdjust modeAdjust)
        {
            switch (modeAdjust)
            {
                case IDependencyServices.WindowSoftInputModeAdjust.Pan:
                    Xamarin.Forms.Application.Current.On<Xamarin.Forms.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(Xamarin.Forms.PlatformConfiguration.AndroidSpecific.WindowSoftInputModeAdjust.Pan);
                    break;

                case IDependencyServices.WindowSoftInputModeAdjust.Resize:
                    Xamarin.Forms.Application.Current.On<Xamarin.Forms.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(Xamarin.Forms.PlatformConfiguration.AndroidSpecific.WindowSoftInputModeAdjust.Resize);
                    break;

                case IDependencyServices.WindowSoftInputModeAdjust.Unspecified:
                    Xamarin.Forms.Application.Current.On<Xamarin.Forms.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(Xamarin.Forms.PlatformConfiguration.AndroidSpecific.WindowSoftInputModeAdjust.Unspecified);
                    break;

                default:
                    Xamarin.Forms.Application.Current.On<Xamarin.Forms.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(Xamarin.Forms.PlatformConfiguration.AndroidSpecific.WindowSoftInputModeAdjust.Unspecified);
                    break;
            }
        }
    }
}