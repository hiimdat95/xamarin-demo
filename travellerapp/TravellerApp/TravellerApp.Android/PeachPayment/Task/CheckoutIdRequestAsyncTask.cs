using System;
using Android.OS;
using Android.Util;
using Java.IO;
using Java.Net;

namespace TravellerApp.Droid.PeachPayment.Task
{
    public class CheckoutIdRequestAsyncTask : AsyncTask
    {

        private CheckoutIdRequestListener listener;

        public CheckoutIdRequestAsyncTask(CheckoutIdRequestListener listener)
        {
            this.listener = listener;
        }

        protected override void OnPreExecute()
        {
        }

        protected override Java.Lang.Object DoInBackground(params Java.Lang.Object[] @params)
        {
            if (@params.Length != 2)
            {
                return null;
            }

            String amount = (string)@params[0];
            String currency = (string)@params[1];

            return requestCheckoutId(amount, currency);
        }

        protected override void OnPostExecute(Java.Lang.Object result)
        {
            if (listener != null)
            {
                listener.onCheckoutIdReceived((String)result);
            }
        }

        private String requestCheckoutId(String amount,
                                     String currency)
        {
            String urlString = TravellerApp.Droid.PeachPayment.Common.Constants.BASE_URL + "/token?" +
                    "amount=" + amount +
                    "&currency=" + currency +
                    "&paymentType=PA" +
                    /* store notificationUrl on your server to change it any time without updating the app */
                    "&notificationUrl=http://52.59.56.185:80/notification";
            URL url;
            HttpURLConnection connection = null;
            String checkoutId = null;

            try
            {
                url = new URL(urlString);
                connection = (HttpURLConnection)url.OpenConnection();
                connection.ConnectTimeout = TravellerApp.Droid.PeachPayment.Common.Constants.CONNECTION_TIMEOUT;

                JsonReader reader = new JsonReader(
                    new InputStreamReader(connection.InputStream, "UTF-8"));

                reader.BeginObject();

                while (reader.HasNext)
                {
                    if (reader.NextName().Equals("checkoutId"))
                    {
                        checkoutId = reader.NextString();

                        break;
                    }
                }

                reader.EndObject();
                reader.Close();

                Log.Debug(TravellerApp.Droid.PeachPayment.Common.Constants.LOG_TAG, "Checkout ID: " + checkoutId);
            }
            catch (Exception e)
            {
                Log.Debug(TravellerApp.Droid.PeachPayment.Common.Constants.LOG_TAG, "Error: ", e);
            }
            finally
            {
                if (connection != null)
                {
                    connection.Disconnect();
                }
            }

            return checkoutId;
        }

    }
}
