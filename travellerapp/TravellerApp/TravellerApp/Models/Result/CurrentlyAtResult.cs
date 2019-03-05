using System;
using TravellerApp.Constants;

namespace TravellerApp.Models.Result
{
    public class CurrentlyAtResult
    {
        public bool success { get; set; }

        public string message { get; set; }

        public string Host { get; set; }

        public string Token { get; set; }

        public string URL { get; set; }

        public string visit_token { get; set; }

        public bool agreed { get; set; }

        public string image { get; set; }

        public string summary { get; set; }

        public DateTimeOffset? date { get; set; }

        public DateTimeOffset? date_out { get; set; }

        public string state { get; set; }   

        public string stateDisplay
        {
            get
            {
                if (string.IsNullOrEmpty(state))
                    return string.Empty;
                if (States.Draft.Equals(state.ToLower()))
                {
                    return "Provisional";
                }
                else if (States.Confirm.Equals(state.ToLower()))
                {
                    return "Confirmed";
                }
                else if (States.Sale.Equals(state.ToLower()))
                {
                    return "Checked In";
                }
                else if (States.Check_Out.Equals(state.ToLower()))
                {
                    return "Checked Out";
                }
                else if (States.Cancel.Equals(state.ToLower()))
                {
                    return "Cancelled";
                }
                else
                {
                    return string.Empty;
                }
            }
        }
    }
}