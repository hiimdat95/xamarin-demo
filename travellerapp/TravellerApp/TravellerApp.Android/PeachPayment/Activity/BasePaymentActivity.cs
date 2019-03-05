using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Com.Oppwa.Mobile.Connect.Checkout.Dialog;
using Com.Oppwa.Mobile.Connect.Checkout.Meta;
using Com.Oppwa.Mobile.Connect.Exception;
using Com.Oppwa.Mobile.Connect.Provider;
using TravellerApp.Droid.PeachPayment.Task;
using Com.Oppwa.Mobile.Connect.Service;
using Com.Oppwa.Mobile.Connect.Payment;
using System.Collections.Generic;

namespace TravellerApp.Droid.PeachPayment.Activity
{
    [Activity(Label = "BasePaymentActivity")]
    public class BasePaymentActivity : BaseActivity, CheckoutIdRequestListener, PaymentStatusRequestListener
    {
        private static String STATE_RESOURCE_PATH = "STATE_RESOURCE_PATH";
        protected String resourcePath;
        public TaskCompletionSource<bool> PaymentTaskCompletion { set; get; }

        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            base.OnCreate(savedInstanceState, persistentState);
            if (savedInstanceState != null)
            {
                resourcePath = savedInstanceState.GetString(STATE_RESOURCE_PATH);
            }

        }
            
        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            Intent = intent;

            /* Check if the intent contains the callback scheme. */
            if (resourcePath != null && hasCallbackScheme(intent))
            {
                PaymentTaskCompletion.SetResult(true);
            }
        }


        protected Boolean hasCallbackScheme(Intent intent)
        {
            String scheme = intent.Scheme;

            return GetString(Resource.String.checkout_ui_callback_scheme).Equals(scheme) ||
                                                                         GetString(Resource.String.payment_button_callback_scheme).Equals(scheme) ||
                                                                         GetString(Resource.String.custom_ui_callback_scheme).Equals(scheme);
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            outState.PutString(STATE_RESOURCE_PATH, resourcePath);

        }


        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            /* Override onActivityResult to get notified when the checkout process is done. */
            if (requestCode == CHECKOUT_ACTIVITY.CheckoutActivity)
            {
                switch ((int)resultCode)
                {
                    case CHECKOUT_ACTIVITY.ResultOk:
                        /* Transaction completed. */
                        Transaction transaction = (Com.Oppwa.Mobile.Connect.Provider.Transaction)data.GetParcelableExtra(
                            CHECKOUT_ACTIVITY.CheckoutResultTransaction);

                        resourcePath = data.GetStringExtra(
                            CHECKOUT_ACTIVITY.CheckoutResultResourcePath);

                        /* Check the transaction type. */
                        if (transaction.TransactionType == TransactionType.Sync)
                        {
                            /* Check the status of synchronous transaction. */
                            PaymentTaskCompletion.SetResult(true);
                        }
                        else
                        {
                            /* The on onNewIntent method may be called before onActivityResult
                               if activity was destroyed in the background, so check
                               if the intent already has the callback scheme */
                            if (hasCallbackScheme(Intent))
                            {
                                PaymentTaskCompletion.SetResult(true);
                            }
                            else
                            {
                                /* The on onNewIntent method wasn't called yet,
                                   wait for the callback. */
                                showProgressDialog(Resource.String.progress_message_please_wait);
                            }
                        }

                        break;

                    case CHECKOUT_ACTIVITY.ResultCanceled:
                        hideProgressDialog();
                        break;

                    case CHECKOUT_ACTIVITY.ResultError:
                        hideProgressDialog();
                        PaymentError error = (Com.Oppwa.Mobile.Connect.Exception.PaymentError)data.GetParcelableExtra(
                            CHECKOUT_ACTIVITY.CheckoutResultError);

                        showAlertDialog(Resource.String.error_message);
                        break;
                }
            }
        }

        public void requestCheckoutId(String callbackScheme)
        {
            showProgressDialog(Resource.String.progress_message_checkout_id);

            new CheckoutIdRequestAsyncTask(this)
                .Execute(TravellerApp.Droid.PeachPayment.Common.Constants.Config.AMOUNT, TravellerApp.Droid.PeachPayment.Common.Constants.Config.CURRENCY);
        }

        public void onCheckoutIdReceived(string checkoutId)
        {
            hideProgressDialog();

            if (checkoutId == null)
            {
                showAlertDialog(Resource.String.error_message);
            }
            else
            {
                openCheckoutUI(checkoutId);
            }
        }

        public void onErrorOccurred()
        {
            hideProgressDialog();
            showAlertDialog(Resource.String.error_message);
        }

        public void onPaymentStatusReceived(string paymentStatus)
        {
            hideProgressDialog();

            if ("OK".Equals(paymentStatus))
            {
                showAlertDialog(Resource.String.message_successful_payment);
                PaymentTaskCompletion.SetResult(true);

                return;
            }
            else
            {
                PaymentTaskCompletion.SetResult(false);
            }

            showAlertDialog(Resource.String.message_unsuccessful_payment);

        }

        protected void requestPaymentStatus(String resourcePath)
        {
            showProgressDialog(Resource.String.progress_message_payment_status);
            new PaymentStatusRequestAsyncTask(this).Execute(resourcePath);
        }


        public CheckoutSettings createCheckoutSettings(String checkoutId, String callbackScheme)
        {
            return new CheckoutSettings(checkoutId, TravellerApp.Droid.PeachPayment.Common.Constants.Config.PAYMENT_BRANDS,
                                        Connect.ProviderMode.Test)
                .SetSkipCVVMode(CheckoutSkipCVVMode.ForStoredCards)
                .SetWindowSecurityEnabled(false);
                //.SetShopperResultUrl(callbackScheme + "://result");
        }

        public void openCheckoutUI(String checkoutId)
        {
            CheckoutSettings checkoutSettings = createCheckoutSettings(checkoutId, GetString(Resource.String.checkout_ui_callback_scheme));
            //checkoutSettings.SetSecurityPolicyModeForBrand("VISA", CheckoutSecurityPolicyMode.DeviceAuthRequired);
            //checkoutSettings.SetSecurityPolicyModeForBrand("MASTER", CheckoutSecurityPolicyMode.DeviceAuthRequiredIfAvailable);
            //checkoutSettings.SetSecurityPolicyModeForTokens(CheckoutSecurityPolicyMode.DeviceAuthRequired);
            //checkoutSettings.SetTotalAmountRequired(true);
            /* Set componentName if you want to receive callbacks from the checkout */
            ComponentName componentName = new ComponentName(this.PackageName, Java.Lang.Class.FromType(typeof(CheckoutBroadcastReceiver)).Name);

            /* Set up the Intent and start the checkout activity. */
            Intent intent = checkoutSettings.CreateCheckoutActivityIntent(this, componentName);

            StartActivityForResult(intent, CHECKOUT_ACTIVITY.RequestCodeCheckout);
        }
    }
}
