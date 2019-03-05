using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TravellerApp.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ExplainTheStats : ContentPage
	{
		public ExplainTheStats ()
		{
			InitializeComponent ();
        }

        void BackClicked(object sender, EventArgs args)
        {
            Navigation.PopModalAsync();
        }
    }
}