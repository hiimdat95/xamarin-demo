using SQLite;

namespace KamooniHost.Models.Local
{
    [Table("UserData")]
    public class UserData : BaseModel
    {
        private string name;

        [Column("name"), PrimaryKey]
        public string Name { get => name; set => SetProperty(ref name, value); }

        private string value;

        [Column("value")]
        public string Value { get => value; set => SetProperty(ref this.value, value); }
    }
}