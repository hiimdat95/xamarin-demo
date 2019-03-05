using System;
using System.IO;
using TravellerApp.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TravellerApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ThingsToDoPages : ContentPage
    {
        public ThingsToDoPages()
        {                          
            InitializeComponent();
        }

        public ThingsToDoPages(Things things)
        {
            //LiveReload.Init();
            InitializeComponent();

            SetThingsToDo(things);
            
        }

        public void SetThingsToDo(Things things)
        {
            //ImageSource image = !(things.image is string base64) ? null : ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(base64)));
            if (things == null)
                return;

            imageHeader.Source = things.image_url;
             
            nameHeader.Text = things.name;

            summary.Text = things.summary;
        }
    }
}