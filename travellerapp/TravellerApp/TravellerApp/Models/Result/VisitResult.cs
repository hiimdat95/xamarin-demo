using System.Collections.Generic;

namespace TravellerApp.Models.Result
{
    public class VisitResult
    {
        public bool success { get; set; }  
        public string message { get; set; }      
        public List<Visits> visits { get; set; }
    }
}