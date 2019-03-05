using KamooniHost.IServices;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KamooniHost.Views.Popup
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PremindNotification : AbsoluteLayout
    {
        public PremindNotification()
        {
            InitializeComponent();
        }

        public PremindNotification(Uri icon)
        {
            InitializeComponent();

            this.icon.Source = ImageSource.FromUri(icon);
        }

        private void OnBackGroundTapped(object sender, EventArgs e)
        {
            DependencyService.Get<IPremindService>()?.HideContent();
        }

        private void OnBackGroundSwiped(object sender, SwipedEventArgs e)
        {

        }
    }
}