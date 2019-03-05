using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using KamooniHost.IDependencyServices;
using KamooniHost.iOS.DependencyService;
using UIKit;

[assembly: Xamarin.Forms.Dependency(typeof(WindowSoftInputModeAdjustService))]

namespace KamooniHost.iOS.DependencyService
{
    public class WindowSoftInputModeAdjustService : IWindowSoftInputModeAdjust
    {
        public void UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust modeAdjust)
        {

        }
    }
}