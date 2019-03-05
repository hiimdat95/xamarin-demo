using System;
using System.Threading.Tasks;

namespace TravellerApp.Interfaces
{
    public interface IPeachPayment
    {
        Task<bool> OpenPaymentUi(string CheckoutId);
    }
}
