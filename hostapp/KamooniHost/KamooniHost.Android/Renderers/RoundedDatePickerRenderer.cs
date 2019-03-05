using Android.Content;
using Android.Support.V4.Content;
using KamooniHost.Droid.Renderers;

using Xamarin.Forms;
using Xamarin.Forms.Controls;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(RoundedDatePicker), typeof(RoundedDatePickerRenderer))]

namespace KamooniHost.Droid.Renderers
{
    public class RoundedDatePickerRenderer : DatePickerRenderer
    {
        public RoundedDatePickerRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<DatePicker> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.Background = ContextCompat.GetDrawable(Android.App.Application.Context, Resource.Drawable.RoundedBackground);
                Control.SetPadding(20, 20, 20, 20);
                Control.TextAlignment = Android.Views.TextAlignment.Gravity;
            }
        }
    }
}