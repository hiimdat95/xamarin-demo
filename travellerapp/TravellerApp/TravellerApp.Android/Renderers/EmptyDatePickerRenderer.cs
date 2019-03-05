using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using TravellerApp.Controls;
using TravellerApp.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(EmptyDatePicker), typeof(EmptyDatePickerRenderer))]
namespace TravellerApp.Droid.Renderers
{

    public class EmptyDatePickerRenderer : DatePickerRenderer
    {
        public EmptyDatePickerRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.DatePicker> e)
        {
            base.OnElementChanged(e);

            if (Control == null || e.NewElement == null) return;
            Control.Background = null;
        }
    }
}