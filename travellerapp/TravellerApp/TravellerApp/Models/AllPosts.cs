using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Newtonsoft.Json;
using Realms;
using TravellerApp.Constants;

namespace TravellerApp.Models
{
    internal class AllPosts : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //public string traveller_partner_id { get; set; }

        public string traveller_partner_id { get; set; }

        public string image_medium { get; set; }

        public DateTime date { get; set; }

        public string rating { get; set; }

        public string text { get; set; }

        public int total_visits { get; set; }

        public string image_url { get; set; }

        public string rating_token { get; set; }

        public bool visibleImage
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(image_url) && !"false".Equals(image_url))
                {
                    return true;
                }

                return false;
            }
        }

        public string dateDisplay
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(date.ToString()) && !"false".Equals(date.ToString()))
                {
                    var datefomat = date.ToString("hh MMM ");
                    var timefomat = date.ToString(" HH:mm");

                    return datefomat + "at" + timefomat;
                }

                return string.Empty;
            }
        }

        public int id { get; set; }

        public string host_partner_id { get; set; }
        public string traveller_partner_profile { get; set; }
        public string host_partner_name { get; set; }
        public string traveller_partner_name { get; set; }
        public string place_of_interest_id { get; set; }
        public string place_of_interest_name { get; set; }

        public bool visibleHostToBooking
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(host_partner_id) && !string.IsNullOrWhiteSpace(host_partner_name)
                    && !"false".Equals(host_partner_id) && !"false".Equals(host_partner_name))
                {
                    return true;
                }

                return false;
            }
        }

        public bool visibleHostToOpenPlace
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(place_of_interest_id) && !string.IsNullOrWhiteSpace(place_of_interest_name)
                    && !"false".Equals(place_of_interest_id) && !"false".Equals(place_of_interest_name))
                {
                    return true;
                }

                return false;
            }
        }

        public string current_comment
        {
            get
            {
                return traveller_partner_name + ": " + text;
            }
        }


        public int likes { get; set; }

        public int tempLike
        {
            get
            {
                return likes;
            }
            set
            {
                likes = value;
                OnPropertyChanged(nameof(tempLike));
            }
        }

        public int comments { get; set; }

        public List<CommentIds> comment_ids { get; set; }


        public bool visibleListComment { get; set; }

        public bool tempVisibleListComment
        {
            get
            {
                return visibleListComment;
            }
            set
            {
                visibleListComment = value;
                OnPropertyChanged(nameof(tempVisibleListComment));
                OnPropertyChanged(nameof(icon_down));
                OnPropertyChanged(nameof(text_down));
            }
        }

        public bool visibleComment { get; set; }

        public bool tempVisibleComment
        {
            get
            {
                return visibleComment;
            }
            set
            {
                visibleComment = value;
                OnPropertyChanged(nameof(tempVisibleComment));
            }
        }

        public string icon_down
        {
            get
            {
                if (visibleListComment)
                {
                    return "icon_up.png";
                }
                else
                {
                    return "icon_down.png";
                }
            }
        }

        public string text_down
        {
            get
            {
                if (visibleListComment)
                {
                    return "Hide";
                }
                else
                {
                    return "See " + comments + " comments";
                }
            }
        }

        public Xamarin.Forms.ImageSource Avatar
        {
            get
            {
                return Xamarin.Forms.ImageSource.FromStream(
                        () => new MemoryStream(Convert.FromBase64String(Realm.GetInstance().Find<User>(DBLocalID.USER).profile_pic)));
            }

        }

        public string editorText { get; set; }

    }

    class CommentIds
    {

        [JsonProperty("date")]
        public DateTime date { get; set; }

        [JsonProperty("traveller_name")]
        public string traveller_name { get; set; }

        [JsonProperty("comment")]
        public string comment { get; set; }

        public string dateDisplay
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(date.ToString()) && !"false".Equals(date.ToString()))
                {
                    DateTime currentDate = DateTime.Now;
                    long elapsedTicks = currentDate.Ticks - date.Ticks;
                    TimeSpan elapsedSpan = new TimeSpan(elapsedTicks);

                    if (elapsedSpan.TotalMinutes < 60)
                    {
                        return elapsedSpan.TotalMinutes + "min ago";
                    }
                    else if (elapsedSpan.TotalHours < 24)
                    {
                        return elapsedSpan.TotalHours + "hours ago";
                    }
                    else
                    {
                        var datefomat = date.ToString("hh MMM ");
                        var timefomat = date.ToString(" HH:mm");
                        return datefomat + "at" + timefomat;
                    }
                }

                return string.Empty;
            }
        }

    }

}