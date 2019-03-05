using System;
using System.Threading.Tasks;
using TravellerApp.Droid;
using TravellerApp.Droid.PeachPayment.Activity;
using TravellerApp.Interfaces;
using Xamarin.Forms;

[assembly: Dependency(typeof(PeachPaymentService))]

namespace TravellerApp.Droid
{
    internal class PeachPaymentService : IPeachPayment
    {
        public Task<bool> OpenPaymentUi(string CheckoutId)
        {
            (Forms.Context as BasePaymentActivity).requestCheckoutId(Forms.Context.GetString(Resource.String.checkout_ui_callback_scheme));
            //(Forms.Context as BasePaymentActivity).openCheckoutUI(CheckoutId);
            ((BasePaymentActivity)Forms.Context).PaymentTaskCompletion = new TaskCompletionSource<bool>();
            return ((BasePaymentActivity)Forms.Context).PaymentTaskCompletion.Task;
        }
    }
}   
