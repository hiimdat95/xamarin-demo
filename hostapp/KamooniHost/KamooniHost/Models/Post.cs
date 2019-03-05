using Newtonsoft.Json;
using System;

namespace KamooniHost.Models
{
    public class Post : BaseModel
    {
        private DateTime checkInDate;

        [JsonProperty("date")]
        public DateTime Date { get => checkInDate; set => SetProperty(ref checkInDate, value); }

        private string imageMedium;

        [JsonProperty("image_medium")]
        public string Image_medium { get => imageMedium; set => SetProperty(ref imageMedium, value); }

        private string imageUrl;

        [JsonProperty("image_url")]
        public string Image_Url { get => imageUrl; set => SetProperty(ref imageUrl, value); }

        private string totalRatings;

        [JsonProperty("rating")]
        public string TotalRatings { get => totalRatings; set => SetProperty(ref totalRatings, value); }

        private string TextLogin;
        [JsonProperty("text")]
        public string Text { get => TextLogin; set => SetProperty(ref TextLogin, value); }

        private string totalVisits;
        [JsonProperty("total_visits")]
        public string Total_Visits { get => totalVisits; set => SetProperty(ref totalVisits, value); }

        private string travellerPartnerId;
        [JsonProperty("traveller_partner_id")]
        public string Traveller_Partner_Id { get => travellerPartnerId; set => SetProperty(ref travellerPartnerId, value); }

        public bool VisibleImage
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(imageUrl) && !"false".Equals(imageUrl))
                {
                    return true;
                }

                return false;
            }
        }

        public string Date_Display
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(checkInDate.ToString()) && !"false".Equals(checkInDate.ToString()))
                {
                    var datefomat = checkInDate.ToString("hh MMM ");
                    var timefomat = checkInDate.ToString(" HH:mm");

                    return datefomat + "at" + timefomat;
                }

                return string.Empty;
            }
        }
    }
}