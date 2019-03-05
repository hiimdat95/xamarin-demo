using System;
using Xamarin.Forms;

namespace Xamarin.Forms.Controls
{
    public class PaymentButton : Button
    {
        public class PaymentEventArgs : EventArgs
        {
            public string Email { get; set; }
            public string Name { get; set; }
        }


        public Action<object, PaymentEventArgs> OnRegister;
        public void Register(object sender, PaymentEventArgs args)
        {
            if (OnRegister != null)
                OnRegister(sender, args);
        }

    }
}
