using Realms;

namespace TravellerApp.Models
{
    public class User : RealmObject
    {
        [PrimaryKey]
        public string dbID { get; set; }

        public int id { get; set; }

        public string email { get; set; }

        public string name { get; set; }

        public string gender { get; set; }

        public string birthdate { get; set; }

        public string passport_id { get; set; }

        public string mobile { get; set; }

        public string country { get; set; }

        public string traveller_token { get; set; }

        public string profile_pic { get; set; }

        public string passport_pic { get; set; }
    }
}