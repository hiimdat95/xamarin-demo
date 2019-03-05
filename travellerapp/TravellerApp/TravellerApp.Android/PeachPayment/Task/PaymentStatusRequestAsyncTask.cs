using System;
using Android.OS;
using Android.Util;
using Java.IO;
using Java.Net;

namespace TravellerApp.Droid.PeachPayment.Task
{
    public class PaymentStatusRequestAsyncTask : AsyncTask
    {
        private PaymentStatusRequestListener listener;

        public PaymentStatusRequestAsyncTask(PaymentStatusRequestListener listener)
        {
            this.listener = listener;
        }

        protected override void OnPreExecute()
        {
        }

        protected override Java.Lang.Object DoInBackground(params Java.Lang.Object[] @params)
        {
            if (@params.Length != 1)
            {
                return null;
            }

            String resourcePath = (String)@params[0];

            if (resourcePath != null)
            {
                return requestPaymentStatus(resourcePath);
            }

            return null;
        }

        protected override void OnPostExecute(Java.Lang.Object result)
        {
            if (listener != null)
            {
                if (result == null)
                {
                    listener.onErrorOccurred();

                    return;
                }

                listener.onPaymentStatusReceived((String)result);
            }
        }


        private String requestPaymentStatus(String resourcePath)
        {
            if (resourcePath == null)
            {
                return null;
            }

            URL url;
            String urlString;
            HttpURLConnection connection = null;
            String paymentStatus = null;

            try
            {
                urlString = TravellerApp.Droid.PeachPayment.Common.Constants.BASE_URL + "/status?resourcePath=" +
                                        URLEncoder.Encode(resourcePath, "UTF-8");

                Log.Debug(TravellerApp.Droid.PeachPayment.Common.Constants.LOG_TAG, "Status request url: " + urlString);

                url = new URL(urlString);
                connection = (HttpURLConnection)url.OpenConnection();
                connection.ConnectTimeout = TravellerApp.Droid.PeachPayment.Common.Constants.CONNECTION_TIMEOUT;

                JsonReader jsonReader = new JsonReader(
                    new InputStreamReader(connection.InputStream, "UTF-8"));

                jsonReader.BeginObject();

                while (jsonReader.HasNext)
                {
                    if (jsonReader.NextName().Equals("paymentResult"))
                    {
                        paymentStatus = jsonReader.NextString();
                    }
                    else
                    {
                        jsonReader.SkipValue();
                    }
                }

                jsonReader.EndObject();
                jsonReader.Close();

                Log.Debug(TravellerApp.Droid.PeachPayment.Common.Constants.LOG_TAG, "Status: " + paymentStatus);
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

            return paymentStatus;
        }
    }
}
