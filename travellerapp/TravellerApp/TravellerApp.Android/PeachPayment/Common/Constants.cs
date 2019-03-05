using System;
using System.Collections.Generic;

namespace TravellerApp.Droid.PeachPayment.Common
{
    public class Constants
    {
        public static int CONNECTION_TIMEOUT = 5000;

        public static String BASE_URL = "http://52.59.56.185";

        public static String LOG_TAG = "msdk.demo";

        /* The configuration values to change across the app */
        public static class Config
        {
            /* The payment brands for Ready-to-Use UI and Payment Button */
            public static HashSet<String> PAYMENT_BRANDS = new HashSet<string> { "VISA", "MASTER", "PAYPAL" };

            /* The default payment brand for payment button */
            public static String PAYMENT_BUTTON_BRAND = "VISA";

            /* The default amount and currency */
            public static String AMOUNT = "49.99";
            public static String CURRENCY = "EUR";

            /* The card info for SDK & Your Own UI*/
            public static String CARD_BRAND = "VISA";
            public static String CARD_HOLDER_NAME = "JOHN DOE";
            public static String CARD_NUMBER = "4200000000000000";
            public static String CARD_EXPIRY_MONTH = "07";
            public static String CARD_EXPIRY_YEAR = "21";
            public static String CARD_CVV = "123";
        }
    }

}
