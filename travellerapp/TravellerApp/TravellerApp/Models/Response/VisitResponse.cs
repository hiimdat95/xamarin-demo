using Newtonsoft.Json;
using TravellerApp.Models.Result;

namespace TravellerApp.Models.Response
{
    public class VisitResponse
    {
        public string jsonrpc { get; set; }    
        public int? id { get; set; }      
        public VisitResult result { get; set; }
    }
}