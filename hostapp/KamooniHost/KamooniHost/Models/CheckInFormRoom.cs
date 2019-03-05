namespace KamooniHost.Models
{
    public class CheckInFormRoom : BaseModel
    {
        private int checkInFormItemId;

        public int CheckInFormItemId { get => checkInFormItemId; set => SetProperty(ref checkInFormItemId, value); }

        private string name;

        public string Name { get => name; set => SetProperty(ref name, value); }
    }
}