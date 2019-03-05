using Newtonsoft.Json;
using System.Collections.Generic;

namespace KamooniHost.Models.Result
{
    public class AvailableRoomResult
    {
        [JsonProperty("result")]
        public List<Room> Rooms { get; set; }
    }
}