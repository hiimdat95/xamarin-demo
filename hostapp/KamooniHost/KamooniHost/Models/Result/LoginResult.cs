using Newtonsoft.Json;

namespace KamooniHost.Models.Result
{
    public class LoginResult : BaseModel
    {
        private string username;

        [JsonProperty("username")]
        public string UserName { get => username; set => SetProperty(ref username, value); }

        private int uid;

        [JsonProperty("uid")]
        public int UId { get => uid; set => SetProperty(ref uid, value); }

        private string db;

        [JsonProperty("db")]
        public string Db { get => db; set => SetProperty(ref db, value); }

        private bool isAdmin;

        [JsonProperty("is_admin")]
        public bool IsAdmin { get => isAdmin; set => SetProperty(ref isAdmin, value); }

        private string webBaseUrl;

        [JsonProperty("web.base.url")]
        public string WebBaseUrl { get => webBaseUrl; set => SetProperty(ref webBaseUrl, value); }

        private string name;

        [JsonProperty("name")]
        public string Name { get => name; set => SetProperty(ref name, value); }

        private int partnerId;

        [JsonProperty("partner_id")]
        public int PartnerId { get => partnerId; set => SetProperty(ref partnerId, value); }

        private int companyId;

        [JsonProperty("company_id")]
        public int CompanyId { get => companyId; set => SetProperty(ref companyId, value); }

        private string sessionId;

        [JsonProperty("session_id")]
        public string SessionId { get => sessionId; set => SetProperty(ref sessionId, value); }

        private bool isSuperuser;

        [JsonProperty("is_superuser")]
        public bool IsSuperuser { get => isSuperuser; set => SetProperty(ref isSuperuser, value); }

        private string userCompanies;

        [JsonProperty("user_companies")]
        public string UserCompanies { get => userCompanies; set => SetProperty(ref userCompanies, value); }
    }
}