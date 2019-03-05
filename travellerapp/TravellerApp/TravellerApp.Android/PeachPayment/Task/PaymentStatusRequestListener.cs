using System;
namespace TravellerApp.Droid.PeachPayment.Task
{
    public interface PaymentStatusRequestListener
    {
        void onErrorOccurred();
        void onPaymentStatusReceived(String paymentStatus);
    }
}
