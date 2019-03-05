namespace Xamarin.Forms.Controls
{
    public class CustomSearchBar: SearchBar
    {
        public static readonly BindableProperty BarTintColorProperty = BindableProperty.Create(nameof(BarTintColor), typeof(Color), typeof(CustomSearchBar), default);

        public Color BarTintColor
        {
            get { return (Color)GetValue(BarTintColorProperty); }
            set { SetValue(BarTintColorProperty, value); }
        }

        public CustomSearchBar()
        {
        }

    }
}
