using System;
using Android.App;
using Android.Content;
using Com.Oppwa.Mobile.Connect.Checkout.Dialog;

namespace TravellerApp.Droid
{
    [BroadcastReceiver(Enabled = true, Exported = false)]
    public class CheckoutBroadcastReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            // Do stuff here.
            String action = intent.Action;

            if (CHECKOUT_ACTIVITY.ActionOnBeforeSubmit.Equals(action))
            {
                String paymentBrand = intent.GetStringExtra(CHECKOUT_ACTIVITY.ExtraPaymentBrand);
                String checkoutId = intent.GetStringExtra(CHECKOUT_ACTIVITY.ExtraCheckoutId);

                ComponentName senderComponentName = (Android.Content.ComponentName)intent.GetParcelableExtra(
                    CHECKOUT_ACTIVITY.ExtraSenderComponentName);

                /* This callback can be used to request a new checkout ID if selected payment brand requires
                   some specific parameters or just send back the same checkout id to continue checkout process */
                intent = new Intent(CHECKOUT_ACTIVITY.ActionOnBeforeSubmit);
                intent.SetComponent(senderComponentName);
                intent.SetPackage(senderComponentName.PackageName);

                intent.AddFlags(ActivityFlags.NewTask);
                intent.PutExtra(CHECKOUT_ACTIVITY.ExtraCheckoutId, checkoutId);

                /* Also it can be used to cancel the checkout process by sending
                   the CheckoutActivity.EXTRA_CANCEL_CHECKOUT */
                intent.PutExtra(CHECKOUT_ACTIVITY.ExtraTransactionAborted, false);

                context.StartActivity(intent);
            }
        }
    }
}
