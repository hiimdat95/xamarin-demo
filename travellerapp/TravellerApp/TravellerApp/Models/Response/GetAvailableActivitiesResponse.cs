using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace TravellerApp.Models.Response
{
    class GetAvailableActivitiesResponse : BaseResponse
    {
        [JsonProperty("result")]
        public List<ActivitiesResult> Result { get; set; }
    }

    class ActivitiesResult
    {
        [JsonProperty("activity_name")]
        public string activity_name { get; set; }

        [JsonProperty("image_url")]
        public string image_url { get; set; }

        [JsonProperty("activity_id")]
        public string activity_id { get; set; }

        [JsonProperty("period_ids")]
        public List<PeriodId> period_ids { get; set; }

        [JsonProperty("summary")]
        public string summary { get; set; }

       
    }

    class PeriodId
    {
        [JsonProperty("period_name")]
        public bool period_name { get; set; }

        [JsonProperty("end_date")]
        public string end_date { get; set; }

        [JsonProperty("price")]
        public double price { get; set; }

        [JsonProperty("available_qty")]
        public int available_qty { get; set; }

        [JsonProperty("period_id")]
        public int period_id { get; set; }

        [JsonProperty("start_date")]
        public string start_date { get; set; }

        [JsonProperty("pax_capacity")]
        public int pax_capacity { get; set; }

        public string detail {
            get {
                return "R"+ price + " per person • " + available_qty + " spots left";
            }
        }

        public string detail_date
        {
            get
            {
                DateTime start = DateTime.ParseExact(start_date, "yyyy-MM-dd HH:mm:ss",
                                       CultureInfo.InvariantCulture);
                DateTime end = DateTime.ParseExact(end_date, "yyyy-MM-dd HH:mm:ss",
                                       CultureInfo.InvariantCulture);
                return (start.ToString("ddd, MMM dd HH:mm") + " - " + end.ToString("ddd, MMM dd HH:mm"));
            }
        }

        public string title_date
        {
            get
            {
                DateTime start = DateTime.ParseExact(start_date, "yyyy-MM-dd HH:mm:ss",
                                       CultureInfo.InvariantCulture);
                return start.ToString("ddd, MMM dd");
            }
        }

        public int selected_available_qty { get; set; }

        public int[] list_person
        {
            get
            {
                if (available_qty == 0)
                    return null;

                var result = new int[available_qty];
                for (int i = 0; i < available_qty; i++)
                {
                    result[i] = i + 1;
                }
                return result;
            }
        }
        public int selected_person { get; set; }


        public string name_button
        {
            get
            {
                if (selected_person > 0)
                {
                    return "";
                }
                else
                    return "Choice";
            }
        }
        public string activity_id { get; set; }

        public string activity_name { get; set; }

        public double totalPrice { get; set; }

    }
}
