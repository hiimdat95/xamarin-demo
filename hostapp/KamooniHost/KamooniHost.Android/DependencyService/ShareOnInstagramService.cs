using Android.Content;
using Android.Content.PM;
using Android.Database;
using Android.Net;
using Android.Provider;
using Android.Support.V4.Content;
using Java.IO;
using System.Threading.Tasks;
using Xamarin.Forms;
using KamooniHost.Droid.DependencyService;
using KamooniHost.IDependencyServices;

[assembly: Dependency(typeof(ShareOnInstagramService))]
namespace KamooniHost.Droid.DependencyService
{
    internal class ShareOnInstagramService : IShareOnInstagramAPI
    {
        public TaskCompletionSource<bool> ShareInsTaskCompletion { set; get; }

        public Task<bool> OpenShareOnInstagram(string path, string content)
        {
            ShareInsTaskCompletion = new TaskCompletionSource<bool>();
            // Copy content to clip broad.
            CopyToClipbroad(content);
            // Open share.
            CreateShareInstagram(path);
            return ShareInsTaskCompletion.Task;
        }

        private void CopyToClipbroad(string content)
        {
            ClipboardManager clipboard = (ClipboardManager)Forms.Context.GetSystemService(Context.ClipboardService);
            clipboard.Text = content;
        }


        private void CreateShareInstagram(string path)
        {
            if (IsAppInstalled())
            {
                Intent share = new Intent(Intent.ActionSend);
                // Set the MIME type
                share.SetType("image/*");
                // Create the URI from the media
                File media = new File(path);
                Uri uri = null;
                if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
                {
                    uri = FileProvider.GetUriForFile(Forms.Context, "com.kamooni.host.fileprovider", media);
                }
                else
                {
                    uri = Uri.FromFile(media);
                }
                // Add the URI to the Intent.
                share.PutExtra(Intent.ExtraStream, uri);
                share.SetPackage("com.instagram.android");
                // Broadcast the Intent.
                Forms.Context.StartActivity(share);
                ShareInsTaskCompletion.SetResult(true);
            }
            else
            {
                try
                {
                    Forms.Context.StartActivity(new Intent(Intent.ActionView, Uri.Parse("market://details?id=" + "com.instagram.android")));
                }
                catch (ActivityNotFoundException anfe)
                {
                    Forms.Context.StartActivity(new Intent(Intent.ActionView, Uri.Parse("https://play.google.com/store/apps/details?id=" + "com.instagram.android")));
                }
            }
        }

        private bool IsAppInstalled()
        {
            PackageManager pm = Forms.Context.PackageManager;
            bool installed = false;
            try
            {
                pm.GetPackageInfo("com.instagram.android", PackageInfoFlags.Activities);
                installed = true;
            }
            catch (PackageManager.NameNotFoundException e)
            {
                installed = false;
            }

            return installed;
        }
    }
}