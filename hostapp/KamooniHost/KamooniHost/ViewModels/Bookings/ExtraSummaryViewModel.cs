using KamooniHost.Constants;
using KamooniHost.Helpers;
using KamooniHost.IServices;
using KamooniHost.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Extensions;
using Xamarin.Forms.TinyMVVM;

namespace KamooniHost.ViewModels
{
    public class ExtraSummaryViewModel : TinyViewModel
    {
        private readonly IBookingsService openBookingsService;

        private Booking booking;
        private GuestBill selectedGuest;
        private List<ExtraItem> selectedExtraItems;
        private bool fromBills;

        public List<ExtraItem> SelectedExtraItems { get => selectedExtraItems; set => SetProperty(ref selectedExtraItems, value); }

        private double totalPayment;
        public double TotalPayment { get => totalPayment; set => SetProperty(ref totalPayment, value); }

        private bool canPayment;
        public bool CanPayment { get => canPayment; set => SetProperty(ref canPayment, value); }

        public ICommand IncreaseQuantityCommand { get; private set; }
        public ICommand DecreaseQuantityCommand { get; private set; }
        public ICommand AddToBillsCommand { get; private set; }

        public ExtraSummaryViewModel(IBookingsService openBookingsService)
        {
            this.openBookingsService = openBookingsService;

            IncreaseQuantityCommand = new AwaitCommand<ExtraItem>(IncreaseQuantity);
            DecreaseQuantityCommand = new AwaitCommand<ExtraItem>(DecreaseQuantity);
            AddToBillsCommand = new AwaitCommand(AddToBills);
        }

        public override void Init(object data)
        {
            base.Init(data);

            booking = Parameters.GetValue<Booking>(ContentKey.BOOKING);
            selectedGuest = Parameters.GetValue<GuestBill>(ContentKey.SELECTED_GUEST);
            SelectedExtraItems = Parameters.GetValue<List<ExtraItem>>(ContentKey.SELECTED_EXTRA_ITEMS) ?? new List<ExtraItem>();
            fromBills = Parameters.GetValue<bool>(ContentKey.FROM_BILLS);

            TotalPayment = SelectedExtraItems.Sum(e => e.Total);
        }

        private void IncreaseQuantity(ExtraItem sender, TaskCompletionSource<bool> tcs)
        {
            sender.Quantity++;
            TotalPayment = SelectedExtraItems.Sum(e => e.Total);
            CanPayment = SelectedExtraItems.Count > 0;
            tcs.SetResult(true);
        }

        private void DecreaseQuantity(ExtraItem sender, TaskCompletionSource<bool> tcs)
        {
            if (sender.Quantity > 0)
            {
                sender.Quantity--;
                TotalPayment = SelectedExtraItems.Sum(e => e.Total);
                CanPayment = SelectedExtraItems.Count > 0;
            }
            tcs.SetResult(true);
        }

        private void AddToBills(object sender, TaskCompletionSource<bool> tcs)
        {
            if (IsBusy || !ConnectivityHelper.IsNetworkAvailable())
            {
                return;
            }

            IsBusy = true;
            Task.Run(async () =>
            {
                return await openBookingsService.AddToBill(booking.Id, selectedGuest.GuestDetails.Id, SelectedExtraItems.FindAll(e => e.Quantity > 0));
            }).ContinueWith(task => Device.BeginInvokeOnMainThread(async () =>
            {
                IsBusy = false;
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    if (task.Result?.Success ?? false)
                    {
                        await CoreMethods.DisplayAlert("", "Added Item(s) to Bill", TranslateExtension.GetValue("ok"));

                        CoreMethods.RemoveFromNavigation<ExtraAddViewModel>();

                        MessagingCenter.Send(this, MessageKey.EXTRA_ITEMS_ADDED);

                        await CoreMethods.PopViewModel();
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(task.Result?.Message))
                            await CoreMethods.DisplayAlert("", task.Result.Message, TranslateExtension.GetValue("ok"));
                        else
                            await CoreMethods.DisplayAlert("", TranslateExtension.GetValue("dialog_message_server_error"), TranslateExtension.GetValue("ok"));
                    }
                }
                else if (task.IsFaulted)
                {
                    await CoreMethods.DisplayAlert(TranslateExtension.GetValue("error"), task.Exception?.GetBaseException().Message, TranslateExtension.GetValue("ok"));
                }
            }));
            tcs.SetResult(true);
        }
    }
}