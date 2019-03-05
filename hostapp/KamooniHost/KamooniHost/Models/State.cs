using KamooniHost.Models.Response;
using KamooniHost.Models.Result;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace KamooniHost.Models
{
    public class State : BaseModel
    {
        private int id;

        [JsonProperty("id")]
        public int Id { get => id; set => SetProperty(ref id, value); }

        private string name;

        [JsonProperty("name")]
        public string Name { get => name; set => SetProperty(ref name, value); }

        private string code;

        [JsonProperty("code")]
        public string Code { get => code; set => SetProperty(ref code, value); }
    }

    public class StatesResponse : BaseResponse
    {
        [JsonProperty("result")]
        public StatesResult Result { get; set; }
    }

    public class StatesResult : BaseResult
    {
        [JsonProperty("states / provinces")]
        public List<State> ListState { get; set; }
    }
}