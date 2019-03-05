using KamooniHost.Models.Control;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;

namespace KamooniHost.Models
{
    public class Room : BaseModel
    {
        private int id;

        [JsonProperty("id")]
        public int Id { get => id; set => SetProperty(ref id, value); }

        private string type;

        [JsonProperty("type")]
        public string Type { get => type; set => SetProperty(ref type, value); }

        private string roomType;

        [JsonProperty("room_type")]
        public string RoomType { get => roomType; set => SetProperty(ref roomType, value); }

        private bool @private;

        [JsonProperty("private")]
        public bool Private { get => @private; set => SetProperty(ref @private, value); }

        private bool camping;

        [JsonProperty("camping")]
        public bool Camping { get => camping; set => SetProperty(ref camping, value); }

        private int capacity;

        [JsonProperty("capacity")]
        public int Capacity { get => capacity; set => SetProperty(ref capacity, value); }

        private string price;

        [JsonProperty("price")]
        public string Price { get => price; set => SetProperty(ref price, value, onChanged: () => OnPropertyChanged(nameof(Total))); }

        private int available;

        [JsonProperty("available")]
        public int Available { get => available; set => SetProperty(ref available, value); }

        private int totalGuest;

        [JsonProperty("guest_total")]
        public int TotalGuest { get => totalGuest; set => SetProperty(ref totalGuest, value, onChanged: () => OnPropertyChanged(nameof(Total))); }

        [JsonProperty]
        public double Total
        {
            get { try { return Convert.ToDouble(Price) * TotalGuest; } catch { return 0; }; }
        }

        private bool canRemove;

        [JsonIgnore]
        public bool CanRemove { get => canRemove; set => SetProperty(ref canRemove, value); }

        [JsonIgnore]
        public ObservableCollection<GuestSelectButton> ListGuestSelectButton { get; set; } = new ObservableCollection<GuestSelectButton>();
    }
}