using System;
using Android.App;
using Android.Support.V7.App;

namespace TravellerApp.Droid.PeachPayment.Activity
{
    [Activity(Label = "BaseActivity")]
    public class BaseActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private Android.App.ProgressDialog progressDialog;

        protected void showProgressDialog(int messageId)
        {
            if (progressDialog != null && progressDialog.IsShowing)
            {
                return;
            }

            if (progressDialog == null)
            {
                progressDialog = new Android.App.ProgressDialog(this);
                progressDialog.SetCancelable(false);
            }
            progressDialog.SetMessage(Resources.GetString(messageId));
            progressDialog.Show();
        }

        protected void hideProgressDialog()
        {
            if (progressDialog == null)
            {
                return;
            }

            progressDialog.Dismiss();
        }

        protected void showAlertDialog(String message)
        {
            Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(this);
            alert.SetMessage(message);
            alert.SetPositiveButton(Resources.GetString(Resource.String.button_ok), (senderAlert, args) =>
            {
            });
            Dialog dialog = alert.Create();
            dialog.Show();
        }

        protected void showAlertDialog(int messageId)
        {
            showAlertDialog(Resources.GetString(messageId));
        }
    }
}
