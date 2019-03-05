using System;
using System.Collections.Generic;
using TravellerApp.Constants;

namespace TravellerApp.Models
{
    public class Bookings
    {
        public int id { get; set; }

        public string host_token { get; set; }

        public string host_image { get; set; }

        public string image_url { get; set; }

        public string booking_date { get; set; }

        public string depart_date { get; set; }

        public string booking_total { get; set; }

        public string booking_token { get; set; }

        public List<string> booking_source { get; set; }

        public List<string> host_partner_id { get; set; }

        public List<string> host_state { get; set; }

        public string state { get; set; }

        public string host_city { get; set; }

        public string guests { get; set; }

        public string arrive_date { get; set; }

        public string expire_date { get; set; }

        public string peach_payment_link_url { get; set; }

        public string will_expire_date
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(expire_date) && !"false".Equals(expire_date.Trim()))
                {
                    var responseDate = DateTime.Parse(expire_date) - DateTime.Today;
                    return "This booking will expire in" + ' ' + responseDate.Hours.ToString() + ' ' + "hours";
                }

                return string.Empty;     
            }
        }

        public bool isVisibleExpired
        {
            get
            {
                if (States.Draft.Equals(state.ToLower()))
                {
                    return true;
                }

                return false;   
            }
        }
        

        public DateTime arrive_dates
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(arrive_date) && !"false".Equals(arrive_date.Trim()))
                {
                    return DateTime.Parse(arrive_date);
                }

                return default;
            }
        }

        public DateTime depart_dates
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(depart_date) && !"false".Equals(depart_date.Trim()))
                {
                    return DateTime.Parse(depart_date);
                }

                return default;
            }
        }

        public string buttonConfirm
        {
            get
            {
                if (States.Draft.Equals(state.ToLower()))
                {
                    return States.Confirm_Booking;
                }
                else if (States.Confirm.Equals(state.ToLower()))
                {
                    return States.Manage_Booking;
                }
                else if (States.Sale.Equals(state.ToLower()))
                {
                    return States.Open_Booking;
                }
                else
                {
                    return string.Empty;
                }          
            }
        }

        public string states
        {
            get
            {
                if (States.Draft.Equals(state.ToLower()))
                {
                    return "Provisional";
                }
                else if (States.Confirm.Equals(state.ToLower()))
                {
                    return "Confirmed";
                }
                else if(States.Sale.Equals(state.ToLower()))
                {
                    return "Checked In";
                }
                else if(States.Check_Out.Equals(state.ToLower()))
                {
                    return "Checked Out";
                }
                else if(States.Cancel.Equals(state.ToLower()))
                {
                    return "Cancelled";
                }
                else
                {
                    return string.Empty;
                }            
            }
        }

        public bool isVisibleButton
        {
            get
            {
                if (States.Cancel.Equals(state.ToLower()))
                {
                    return false;
                }

                return true;
            }
        }
    }
}