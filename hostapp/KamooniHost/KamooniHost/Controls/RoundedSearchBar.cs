namespace Xamarin.Forms.Controls
{
    public class RoundedSearchBar : SearchBar
    {
        public static readonly BindableProperty BarTintColorProperty = BindableProperty.Create(nameof(BarTintColor), typeof(Color), typeof(RoundedSearchBar), default);

        public Color BarTintColor
        {
            get { return (Color)GetValue(BarTintColorProperty); }
            set { SetValue(BarTintColorProperty, value); }
        }

        public RoundedSearchBar()
        {
        }
    }
}