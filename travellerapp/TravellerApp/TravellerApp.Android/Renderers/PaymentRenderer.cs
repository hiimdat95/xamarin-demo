using System;
using Android.App;
using Android.Content;
using Android.Widget;
using Com.Oppwa.Mobile.Connect.Checkout.Dialog;
using Com.Oppwa.Mobile.Connect.Checkout.Meta;
using TravellerApp.Controls;
using TravellerApp.Droid.PeachPayment.Activity;
using TravellerApp.Droid.Receiver;
using TravellerApp.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Controls;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(PaymentButton), typeof(PaymentRenderer))]
namespace TravellerApp.Droid.Renderers
{
    public class PaymentRenderer : ButtonRenderer
    {

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Button> e)
        {
            base.OnElementChanged(e);
            var button = e.NewElement;

            if (this.Control != null)
            {
                Control.Click += (o, a) =>
                {
                    (Context as MainActivity).requestCheckoutId(Context.GetString(Resource.String.checkout_ui_callback_scheme));;
                };
            }
        }
    }
}
    