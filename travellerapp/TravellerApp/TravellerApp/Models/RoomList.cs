namespace TravellerApp.Models
{
    public class RoomList
    {
        public int id { get; set; }

        public bool camping { get; set; }

        public int available { get; set; }

        public int capacity { get; set; }

        public string price { get; set; }

        public bool Private { get; set; }

        public string room_type { get; set; }

        public string type { get; set; }

        public string roomAvailableType
        {
            get
            {
                return room_type + ' ' + '(' + available + ')';
            }
        }

        public int[] list_room
        {
            get
            {
                if (available == 0)
                    return null;

                var result = new int[available];
                for (int i = 0; i < available; i++)
                {
                    result[i] = i + 1;
                }
                return result;
            }
        }

        public decimal totalPrice { get; set; }

        public int selected_room { get; set; }

        public string selected_room_type
        {
            get
            {
                return room_type + ' ' + '(' + selected_room + ')';
            }
        }
    }
}