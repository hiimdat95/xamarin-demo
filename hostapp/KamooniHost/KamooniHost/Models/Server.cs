namespace KamooniHost.Models
{
    public class Server : BaseModel
    {
        private int serverId;

        public int ServerId { get => serverId; set => SetProperty(ref serverId, value); }

        private string createdDate;

        public string CreatedDate { get => createdDate; set => SetProperty(ref createdDate, value); }

        private string updatedDate;

        public string UpdatedDate { get => updatedDate; set => SetProperty(ref updatedDate, value); }

        private string hostName;

        public string HostName { get => hostName; set => SetProperty(ref hostName, value); }

        private string url;

        public string Url { get => url; set => SetProperty(ref url, value); }

        private string databaseName;

        public string DatabaseName { get => databaseName; set => SetProperty(ref databaseName, value); }

        private string userName;

        public string UserName { get => userName; set => SetProperty(ref userName, value); }

        private string password;

        public string Password { get => password; set => SetProperty(ref password, value); }
    }
}