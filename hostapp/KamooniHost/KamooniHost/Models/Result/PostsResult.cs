using System;
using System.Collections.Generic;
using System.Text;

namespace KamooniHost.Models.Result
{
    class PostsResult
    {
        public bool success { get; set; }

        public List<Posts> posts { get; set; }
    }
}
