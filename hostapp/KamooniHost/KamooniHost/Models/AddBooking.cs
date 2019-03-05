using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;

namespace KamooniHost.Models
{
    public class AddBooking : BaseModel
    {
        private DateTime checkInDate;

        [JsonProperty("checkin_date")]
        public DateTime CheckInDate { get => checkInDate; set => SetProperty(ref checkInDate, value); }

        private DateTime checkOutDate;

        [JsonProperty("checkout_date")]
        public DateTime CheckOutDate { get => checkOutDate; set => SetProperty(ref checkOutDate, value); }

        private Customer customer = new Customer();

        [JsonProperty("customer")]
        public Customer Customer { get => customer; set => SetProperty(ref customer, value); }

        private ObservableCollection<Room> listRoom = new ObservableCollection<Room>();

        [JsonProperty("room_data")]
        public ObservableCollection<Room> ListRoom { get => listRoom; set => SetProperty(ref listRoom, value); }
    }
}