using KamooniHost.Constants;
using KamooniHost.Helpers;
using KamooniHost.IServices;
using KamooniHost.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Extensions;
using Xamarin.Forms.TinyMVVM;

namespace KamooniHost.ViewModels
{
    public class ExtraAddViewModel : TinyViewModel
    {
        private readonly IBookingsService openBookingsService;

        private Booking booking;
        private GuestBill selectedGuest;
        private bool fromBills;

        private List<ExtraItem> selectedExtraItems = new List<ExtraItem>();

        private ObservableCollection<ExtraItemsByGroup> extraItemsByGroup = new ObservableCollection<ExtraItemsByGroup>();
        public ObservableCollection<ExtraItemsByGroup> ExtraItemsByGroup { get => extraItemsByGroup; set => SetProperty(ref extraItemsByGroup, value); }

        private ObservableCollection<ExtraItem> extraItemsDisplay = new ObservableCollection<ExtraItem>();
        public ObservableCollection<ExtraItem> ExtraItemsDisplay { get => extraItemsDisplay; set => SetProperty(ref extraItemsDisplay, value); }

        private double totalPayment;
        public double TotalPayment { get => totalPayment; set => SetProperty(ref totalPayment, value); }

        private bool canPayment;
        public bool CanPayment { get => canPayment; set => SetProperty(ref canPayment, value); }

        public ICommand CategorySelectedCommand { get; private set; }
        public ICommand IncreaseQuantityCommand { get; private set; }
        public ICommand DecreaseQuantityCommand { get; private set; }
        public ICommand ExtrasPaymentCommand { get; private set; }

        public ExtraAddViewModel(IBookingsService openBookingsService)
        {
            this.openBookingsService = openBookingsService;

            CategorySelectedCommand = new AwaitCommand<ExtraItemsByGroup>(CategorySelected);
            IncreaseQuantityCommand = new AwaitCommand<ExtraItem>(IncreaseQuantity);
            DecreaseQuantityCommand = new AwaitCommand<ExtraItem>(DecreaseQuantity);
            ExtrasPaymentCommand = new AwaitCommand(ExtrasPayment);
        }

        public override void Init(object data)
        {
            base.Init(data);

            booking = Parameters.GetValue<Booking>(ContentKey.BOOKING);
            selectedGuest = Parameters.GetValue<GuestBill>(ContentKey.SELECTED_GUEST);
            fromBills = Parameters.GetValue<bool>(ContentKey.FROM_BILLS);

            Title = selectedGuest?.GuestDetails.Name;
        }

        public override void OnPageCreated()
        {
            base.OnPageCreated();

            GetExtras();
        }

        private void GetExtras()
        {
            if (IsBusy || !ConnectivityHelper.IsNetworkAvailable())
            {
                return;
            }

            IsBusy = true;
            Task.Run(async () =>
            {
                return await openBookingsService.GetExtras();
            }).ContinueWith(task => Device.BeginInvokeOnMainThread(() =>
            {
                IsBusy = false;
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    ExtraItemsByGroup.Clear();

                    if (task.Result?.Success ?? false)
                    {
                        foreach (var item in task.Result.ListExtra.OrderBy(e => e.Category).GroupBy(e => e.Category))
                        {
                            ExtraItemsByGroup.Add(new ExtraItemsByGroup()
                            {
                                Key = item.Key == "false" ? "Miscellaneous" : item.Key,
                                Items = new ObservableCollection<ExtraItem>(item.OrderBy(b => b.Name).ToList())
                            });
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(task.Result?.Message))
                            CoreMethods.DisplayAlert("", task.Result.Message, TranslateExtension.GetValue("ok"));
                        else
                            CoreMethods.DisplayAlert("", TranslateExtension.GetValue("dialog_message_server_error"), TranslateExtension.GetValue("ok"));
                    }
                }
                else if (task.IsFaulted)
                {
                    CoreMethods.DisplayAlert(TranslateExtension.GetValue("error"), task.Exception?.GetBaseException().Message, TranslateExtension.GetValue("ok"));
                }
            }));
        }

        private void CategorySelected(ExtraItemsByGroup sender, TaskCompletionSource<bool> tcs)
        {
            sender.Button.Active();
            foreach (var item in ExtraItemsByGroup.ToList().FindAll(e => e.Key != sender.Key))
            {
                item.Button.Deactive();
            }
            ExtraItemsDisplay = sender.Items;
            tcs.SetResult(true);
        }

        private void IncreaseQuantity(ExtraItem sender, TaskCompletionSource<bool> tcs)
        {
            sender.Quantity++;
            if (selectedExtraItems.Find(e => e.Id == sender.Id) is ExtraItem item)
            {
                selectedExtraItems[selectedExtraItems.IndexOf(item)].Quantity = sender.Quantity;
            }
            else
            {
                selectedExtraItems.Add(sender);
            }
            TotalPayment = selectedExtraItems.Sum(e => e.Total);
            CanPayment = selectedExtraItems.Count > 0;
            tcs.SetResult(true);
        }

        private void DecreaseQuantity(ExtraItem sender, TaskCompletionSource<bool> tcs)
        {
            if (sender.Quantity > 0)
            {
                sender.Quantity--;
                if (selectedExtraItems.Find(e => e.Id == sender.Id) is ExtraItem item)
                {
                    if (sender.Quantity == 0)
                    {
                        selectedExtraItems.Remove(item);
                    }
                    else
                    {
                        selectedExtraItems[selectedExtraItems.IndexOf(item)].Quantity = sender.Quantity;
                    }
                }
                TotalPayment = selectedExtraItems.Sum(e => e.Total);
                CanPayment = selectedExtraItems.Count > 0;
            }
            tcs.SetResult(true);
        }

        private async void ExtrasPayment(object sender, TaskCompletionSource<bool> tcs)
        {
            var @params = new NavigationParameters()
            {
                { ContentKey.BOOKING, booking },
                { ContentKey.SELECTED_GUEST, selectedGuest?.DeepCopy() },
                { ContentKey.SELECTED_EXTRA_ITEMS, selectedExtraItems?.DeepCopy() },
                { ContentKey.FROM_BILLS, fromBills }
            };
            await CoreMethods.PushViewModel<ExtraSummaryViewModel>(@params);
            tcs.SetResult(true);
        }
    }
}