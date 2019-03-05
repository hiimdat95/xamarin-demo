using ImageCircle.Forms.Plugin.Abstractions;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KamooniHost.Views.Popup
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PremindIcon : CircleImage
    {
        public PremindIcon()
        {
            InitializeComponent();
        }

        public PremindIcon(Uri icon)
        {
            InitializeComponent();

            Source = ImageSource.FromUri(icon);
        }
    }
}