using System.Collections.Generic;

namespace TravellerApp.Models.Response
{
    public class ListRoomResponse
    {
        public string jsonrpc { get; set; }

        public int? id { get; set; }

        public List<RoomList> result { get; set; }
    }
}