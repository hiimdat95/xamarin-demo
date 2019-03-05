using Xamarin.Forms;

namespace KamooniHost.Models.Control
{
    public class ButtonProperty : BaseModel
    {
        public bool IsActived;

        private Color backgroundColor;
        public Color BackgroundColor { get => backgroundColor; set => SetProperty(ref backgroundColor, value); }

        private Color borderColor;
        public Color BorderColor { get => borderColor; set => SetProperty(ref borderColor, value); }

        private Color textColor;
        public Color TextColor { get => textColor; set => SetProperty(ref textColor, value); }

        private bool isVisible = true;
        public bool IsVisible { get => isVisible; set => SetProperty(ref isVisible, value); }

        public ButtonProperty()
        {
            Deactive();
        }

        public void Active()
        {
            BackgroundColor = Color.FromHex("#6bc5a9");
            TextColor = Color.White;
            BorderColor = Color.Transparent;
            IsActived = true;
        }

        public void Deactive()
        {
            BackgroundColor = Color.White;
            TextColor = Color.Gray;
            BorderColor = Color.FromHex("#a3a3a3");
            IsActived = false;
        }
    }
}