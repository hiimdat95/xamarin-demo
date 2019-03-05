using Xamarin.Forms;

namespace KamooniHost.Models.Control
{
    public class GuestSelectButton : BaseModel
    {
        public object Parent;

        public bool IsSelected;

        private Color backgroundColor;
        public Color BackgroundColor { get => backgroundColor; set => SetProperty(ref backgroundColor, value); }

        private Color outlineColor;
        public Color OutlineColor { get => outlineColor; set => SetProperty(ref outlineColor, value); }

        private Color imageColor;
        public Color ImageColor { get => imageColor; set => SetProperty(ref imageColor, value); }

        private Color textColor;
        public Color TextColor { get => textColor; set => SetProperty(ref textColor, value); }

        private int quantity;
        public int Quantity { get => quantity; set => SetProperty(ref quantity, value); }

        public GuestSelectButton()
        {
            BackgroundColor = Color.White;
            OutlineColor = Color.LightGray;
            ImageColor = Color.FromHex("#c3c3c3");
            TextColor = Color.Gray;
        }

        public void Select()
        {
            if (IsSelected)
                return;

            BackgroundColor = Color.FromHex("#6bc5a9");
            OutlineColor = Color.Transparent;
            ImageColor = Color.FromHex("#3eb993");
            TextColor = Color.White;
            IsSelected = true;
        }

        public void Deselect()
        {
            if (!IsSelected)
                return;

            BackgroundColor = Color.White;
            OutlineColor = Color.LightGray;
            ImageColor = Color.FromHex("#c3c3c3");
            TextColor = Color.Gray;
            IsSelected = false;
        }
    }
}