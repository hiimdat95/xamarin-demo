using KamooniHost.Models.Control;
using System.Collections.ObjectModel;

namespace KamooniHost.Models
{
    public class ExtraItemsByGroup : BaseModel
    {
        private string key;
        public string Key { get => key; set => SetProperty(ref key, value); }

        private ObservableCollection<ExtraItem> items = new ObservableCollection<ExtraItem>();
        public ObservableCollection<ExtraItem> Items { get => items; set => SetProperty(ref items, value); }

        public ButtonProperty Button { get; set; } = new ButtonProperty();
    }
}