using KamooniHost.Models;
using System;
using System.Collections.Generic;
using TravellerApp.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KamooniHost.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PostsAndReviewsPage : ContentPage
    {
        public Host Host => Helpers.Settings.CurrentHost;
        public List<Post> Post => Helpers.Settings.CurrentPost;

        public PostsAndReviewsPage()
        {
            InitializeComponent();
        }

        public async void addNewPlaceOfInterestOnclick(object sender, EventArgs e)
        {
            //await Navigation.PushAsync(new AddPlacePage(this), true);
            await Navigation.PushAsync(new WriteReviewPage(new Review { Host = Host.Name, Token = Host.Token, VisitToken = "" }), true);
        }
    }
}