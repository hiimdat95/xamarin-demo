using Xamarin.Forms;

namespace KamooniHost.Models.Control
{
    public class TabButton : BaseModel
    {
        private string text;
        public string TextC { get => text; set => SetProperty(ref text, value); }

        private Color borderColor;
        public Color BorderColor { get => borderColor; set => SetProperty(ref borderColor, value); }

        private Color backgroundColor;
        public Color BackgroundColor { get => backgroundColor; set => SetProperty(ref backgroundColor, value); }

        private Color textColor;
        public Color TextColor { get => textColor; set => SetProperty(ref textColor, value); }

        private bool isVisible = true;
        public bool IsVisible { get => isVisible; set => SetProperty(ref isVisible, value); }
    }
}