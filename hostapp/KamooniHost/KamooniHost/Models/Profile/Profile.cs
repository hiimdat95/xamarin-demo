using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;

namespace KamooniHost.Models.Profile
{
    public class Profile : BaseModel
    {
        private Uri url;

        [JsonProperty("url")]
        public Uri Url { get => url; set => SetProperty(ref url, value); }

        private int id;

        [JsonProperty("id")]
        public int Id { get => id; set => SetProperty(ref id, value); }

        private string external_id;

        [JsonProperty("external_id")]
        public string ExternalId { get => external_id; set => SetProperty(ref external_id, value); }

        private string first_name;

        [JsonProperty("first_name")]
        public string FirstName { get => first_name; set => SetProperty(ref first_name, value); }

        private string last_name;

        [JsonProperty("last_name")]
        public string LastName { get => last_name; set => SetProperty(ref last_name, value); }

        private int age;

        [JsonProperty("age")]
        public int Age { get => age; set => SetProperty(ref age, value); }

        private DateTimeOffset date_born;

        [JsonProperty("date_born")]
        public DateTimeOffset DateBorn { get => date_born; set => SetProperty(ref date_born, value); }

        private string email;

        [JsonProperty("email")]
        public string Email { get => email; set => SetProperty(ref email, value); }

        private string gender;

        [JsonProperty("gender")]
        public string Gender { get => gender; set => SetProperty(ref gender, value); }

        private string nickname;

        [JsonProperty("nickname")]
        public string Nickname { get => nickname; set => SetProperty(ref nickname, value); }

        private string city;

        [JsonProperty("city")]
        public string City { get => city; set => SetProperty(ref city, value); }

        private Country country = new Country();

        [JsonProperty("country")]
        public Country Country { get => country; set => SetProperty(ref country, value); }

        private Uri image;

        [JsonProperty("image")]
        public Uri Image { get => image; set => SetProperty(ref image, value); }

        private MembershipLevel membership_level = new MembershipLevel();

        [JsonProperty("membership_level")]
        public MembershipLevel MembershipLevel { get => membership_level; set => SetProperty(ref membership_level, value); }

        private ObservableCollection<Information> information = new ObservableCollection<Information>();

        [JsonProperty("information")]
        public ObservableCollection<Information> Information { get => information; set => SetProperty(ref information, value); }

        private ObservableCollection<Purchase> purchases = new ObservableCollection<Purchase>();

        [JsonProperty("purchases")]
        public ObservableCollection<Purchase> Purchases { get => purchases; set => SetProperty(ref purchases, value); }

        private LastVisit last_visit = new LastVisit();

        [JsonProperty("last_visit")]
        public LastVisit LastVisit { get => last_visit; set => SetProperty(ref last_visit, value); }

        private bool active_campaign;

        [JsonProperty("active_campaign")]
        public bool ActiveCampaign { get => active_campaign; set => SetProperty(ref active_campaign, value); }

        private Uri visit;

        [JsonProperty("visit")]
        public Uri Visit { get => visit; set => SetProperty(ref visit, value); }
    }
}