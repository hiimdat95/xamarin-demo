using System;
namespace TravellerApp.Droid.PeachPayment.Task
{
    public interface CheckoutIdRequestListener
    {
        void onCheckoutIdReceived(String checkoutId);
    }
}
