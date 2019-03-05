namespace KamooniHost.Models
{
    public class Database : BaseModel
    {
        private string name;
        public string Name { get => name; set => SetProperty(ref name, value); }

        private bool selected;
        public bool Selected { get => selected; set => SetProperty(ref selected, value); }
    }
}