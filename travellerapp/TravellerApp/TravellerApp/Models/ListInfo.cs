using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using FFImageLoading.Forms;
using Xamarin.Forms;

namespace TravellerApp.Models
{
    public class ListInfo
    {
        public List<string> property_images { get; set; }

        public int id { get; set; }

        public string name { get; set; }

        public string image { get; set; }

        public string image_small { get; set; }

        public string image_medium { get; set; }

        public string image_url { get; set; }

        public string total_ratings { get; set; }

        public string host_url { get; set; }

        public string email { get; set; }

        public string mobile { get; set; }

        public decimal host_rating { get; set; }

        public string usr_token { get; set; }

        public bool is_live { get; set; }

        public bool is_activity_company { get; set; }

        public bool is_transport { get; set; }

        public bool is_accommodation { get; set; }

        public double partner_latitude { get; set; }

        public double partner_longitude { get; set; }

        public double distance { get; set; }

        public string distance_display { get; set; }

        public string summary { get; set; }

        public string city { get; set; }

        public string state_id { get; set; }

        public string route_id { get; set; }

        public bool is_museum { get; set; }

        public bool is_food { get; set; }

        public bool is_volunteer { get; set; }

        public string country_id { get; set; }

        public string website { get; set; }

        public bool is_tour { get; set; }

        public string street2 { get; set; }

        public string street { get; set; }

        public string province
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(city) && !"false".Equals(city)    
                    && !string.IsNullOrWhiteSpace(state_id) && !"false".Equals(state_id))
                {
                    return city + ',' + state_id;
                }

                return string.Empty;         
            }
        }

        public string Display_host_info
        {
            get {
                return String.Format("{0} - ({1})",province,distance_display);
            }
        }

        public string host_rating_string
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(host_rating.ToString()) && !"false".Equals(host_rating))
                {                                                     
                     return Math.Round(host_rating, 1).ToString();
                }

                return string.Empty;
            }
        }

        List<View> _myItemsSource;
        public List<View> MyItemsSource
        {
            get
            {
                _myItemsSource = new List<View>();
                if (property_images == null ||property_images.Count == 0)
                {
                    _myItemsSource.Add(new Image() { Source = image_url, Aspect = Aspect.AspectFill});
                }
                else
                {
                    for(int i =0; i < property_images.Count; i++)
                    {
                        _myItemsSource.Add(new Image() { Source = property_images[i],  Aspect = Aspect.AspectFill });
                    }
                }
                return _myItemsSource;
            }
        }
    }
}