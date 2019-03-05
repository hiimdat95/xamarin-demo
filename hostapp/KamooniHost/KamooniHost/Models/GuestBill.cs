using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace KamooniHost.Models
{
    public class GuestBill : BaseModel
    {
        private CheckedInGuest guestDetails;

        [JsonProperty("guest_details")]
        public CheckedInGuest GuestDetails { get => guestDetails; set => SetProperty(ref guestDetails, value); }

        private List<BillItem> billItems = new List<BillItem>();

        [JsonProperty("bill_lines")]
        public List<BillItem> BillItems { get => billItems; set => SetProperty(ref billItems, value); }

        private ObservableCollection<BillItemsByGroup> billItemsByGroup = new ObservableCollection<BillItemsByGroup>();
        public ObservableCollection<BillItemsByGroup> BillItemsByGroup { get => billItemsByGroup; set => SetProperty(ref billItemsByGroup, value); }

        public bool CanPayment => GuestDetails.Paid < GuestDetails.Amount;
    }

    public class BillItemsByGroup : BaseModel
    {
        private string key;
        public string Key { get => key; set => SetProperty(ref key, value); }

        private ObservableCollection<BillItem> itemsByGroup = new ObservableCollection<BillItem>();
        public ObservableCollection<BillItem> ItemsByGroup { get => itemsByGroup; set => SetProperty(ref itemsByGroup, value); }

        private double total;
        public double Total { get => total; set => SetProperty(ref total, value); }

        private bool isExpanded;
        public bool IsExpanded { get => isExpanded; set => SetProperty(ref isExpanded, value); }
    }
}