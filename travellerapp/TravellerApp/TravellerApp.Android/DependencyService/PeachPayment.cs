using System;
using System.Runtime.CompilerServices;
using TravellerApp.Droid.DependencyService;
using TravellerApp.Interfaces;

[assembly: Dependency(typeof(PeachPayment))]

namespace TravellerApp.Droid.DependencyService
{
    interface class PeachPayment : IPeachPayment
    {
        public void OpenPaymentUi(string CheckoutId)
        {
            
        }
    }
}
