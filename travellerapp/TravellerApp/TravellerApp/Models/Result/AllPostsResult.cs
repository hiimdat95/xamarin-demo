using System;
using System.Collections.Generic;
using System.Text;

namespace TravellerApp.Models.Result
{
    class AllPostsResult
    {
        public List<AllPosts> posts { get; set; }

        public bool success { get; set; }
    }
}
