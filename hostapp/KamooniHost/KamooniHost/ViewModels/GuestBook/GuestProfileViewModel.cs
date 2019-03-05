using KamooniHost.Helpers;
using KamooniHost.IServices;
using KamooniHost.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Extensions;
using Xamarin.Forms.TinyMVVM;

namespace KamooniHost.ViewModels
{
    public class GuestProfileViewModel : TinyViewModel
    {
        private readonly IGuestBookService guestBookService;

        private int guestId;
        private string guestToken;

        private GuestProfile guestProfile;
        public GuestProfile GuestProfile { get => guestProfile; set => SetProperty(ref guestProfile, value); }

        private string voteNote;
        public string VoteNote { get => voteNote; set => SetProperty(ref voteNote, value); }

        private bool voteNoteVisible;
        public bool VoteNoteVisible { get => voteNoteVisible; set => SetProperty(ref voteNoteVisible, value); }

        public ICommand DownVoteGuestCommand { get; private set; }
        public ICommand CloseVoteNoteCommand { get; private set; }
        public ICommand DownVoteGuestConfirmCommand { get; private set; }
        public ICommand UpVoteGuestCommand { get; private set; }

        public GuestProfileViewModel(IGuestBookService guestBookService)
        {
            this.guestBookService = guestBookService;

            DownVoteGuestCommand = new AwaitCommand(DownVoteGuest);
            CloseVoteNoteCommand = new AwaitCommand(CloseVoteNote);
            DownVoteGuestConfirmCommand = new AwaitCommand(DownVoteGuestConfirm);
            UpVoteGuestCommand = new AwaitCommand(UpVoteGuest);
        }

        public override void Init(object data)
        {
            base.Init(data);

            guestId = Parameters.GetValue<int>(Constants.ContentKey.GUEST_ID);
            guestToken = Parameters.GetValue<string>(Constants.ContentKey.GUEST_TOKEN);
        }

        public override void OnPageCreated()
        {
            base.OnPageCreated();

            GetGuestProfile();
        }

        private void GetGuestProfile()
        {
            if (IsBusy || !ConnectivityHelper.IsNetworkAvailable())
            {
                return;
            }

            IsBusy = true;
            Task.Run(async () =>
            {
                return await guestBookService.GetGuestProfile(guestId);
            }).ContinueWith(task => Device.BeginInvokeOnMainThread(async () =>
            {
                IsBusy = false;
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    if (task.Result != null)
                    {
                        if (task.Result.Success)
                        {
                            if (task.Result.GuestProfiles != null && task.Result.GuestProfiles.Count > 0)
                            {
                                GuestProfile = task.Result.GuestProfiles.FirstOrDefault();
                            }
                            else
                            {
                                await CoreMethods.DisplayAlert("", "Guest Profile not found.", TranslateExtension.GetValue("ok"));

                                await CoreMethods.PopViewModel();
                            }
                        }
                        else if (!string.IsNullOrWhiteSpace(task.Result.Message))
                        {
                            await CoreMethods.DisplayAlert("", task.Result.Message, TranslateExtension.GetValue("ok"));
                        }
                    }
                    else
                    {
                        await CoreMethods.DisplayAlert("", TranslateExtension.GetValue("dialog_message_server_error"), TranslateExtension.GetValue("ok"));
                    }
                }
                else if (task.IsFaulted)
                {
                    await CoreMethods.DisplayAlert(TranslateExtension.GetValue("error"), task.Exception?.GetBaseException().Message, TranslateExtension.GetValue("ok"));
                }
            }));
        }

        private void DownVoteGuest(object sender, TaskCompletionSource<bool> tcs)
        {
            VoteNote = "";
            VoteNoteVisible = true;
            tcs.SetResult(true);
        }

        private void CloseVoteNote(object sender, TaskCompletionSource<bool> tcs)
        {
            VoteNoteVisible = false;
            tcs.SetResult(true);
        }

        private void DownVoteGuestConfirm(object sender, TaskCompletionSource<bool> tcs)
        {
            if (IsBusy || !ConnectivityHelper.IsNetworkAvailable())
            {
                return;
            }

            IsBusy = true;
            Task.Run(async () =>
            {
                return await guestBookService.DownVoteGuest(guestToken, VoteNote);
            }).ContinueWith(task => Device.BeginInvokeOnMainThread(() =>
            {
                IsBusy = false;
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    if (task.Result != null)
                    {
                        if (task.Result.Success)
                        {
                            CoreMethods.DisplayAlert("", "Report Success", TranslateExtension.GetValue("ok"));
                            VoteNoteVisible = false;
                        }
                        else if (!string.IsNullOrWhiteSpace(task.Result.Message))
                        {
                            CoreMethods.DisplayAlert("", task.Result.Message, TranslateExtension.GetValue("ok"));
                        }
                    }
                    else
                    {
                        CoreMethods.DisplayAlert("", TranslateExtension.GetValue("dialog_message_server_error"), TranslateExtension.GetValue("ok"));
                    }
                }
                else if (task.IsFaulted)
                {
                    CoreMethods.DisplayAlert(TranslateExtension.GetValue("error"), task.Exception?.GetBaseException().Message, TranslateExtension.GetValue("ok"));
                }
                tcs.SetResult(true);
            }));
        }

        private void UpVoteGuest(object sender, TaskCompletionSource<bool> tcs)
        {
            if (IsBusy || !ConnectivityHelper.IsNetworkAvailable())
            {
                return;
            }

            IsBusy = true;
            Task.Run(async () =>
            {
                return await guestBookService.UpVoteGuest(guestToken);
            }).ContinueWith(task => Device.BeginInvokeOnMainThread(() =>
            {
                IsBusy = false;
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    if (task.Result != null)
                    {
                        if (task.Result.Success)
                        {
                            CoreMethods.DisplayAlert("", "Report Success", TranslateExtension.GetValue("ok"));
                        }
                        else if (!string.IsNullOrWhiteSpace(task.Result.Message))
                        {
                            CoreMethods.DisplayAlert("", task.Result.Message, TranslateExtension.GetValue("ok"));
                        }
                    }
                    else
                    {
                        CoreMethods.DisplayAlert("", TranslateExtension.GetValue("dialog_message_server_error"), TranslateExtension.GetValue("ok"));
                    }
                }
                else if (task.IsFaulted)
                {
                    CoreMethods.DisplayAlert(TranslateExtension.GetValue("error"), task.Exception?.GetBaseException().Message, TranslateExtension.GetValue("ok"));
                }
                tcs.SetResult(true);
            }));
        }
    }
}