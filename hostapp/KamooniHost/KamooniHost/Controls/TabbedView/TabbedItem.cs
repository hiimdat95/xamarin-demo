namespace Xamarin.Forms.Controls
{
    [ContentProperty(nameof(Content))]
    public class TabbedItem : BindableObject
    {
        public TabbedItem()
        {
            //Parameterless constructor required for xaml instantiation.
        }

        public TabbedItem(string headerText, View content, ImageSource headerIcon = null)
        {
            HeaderText = headerText;
            Content = content;
            if (headerIcon != null)
                HeaderIcon = headerIcon;
        }

        public static readonly BindableProperty HeaderIconProperty = BindableProperty.Create(nameof(HeaderIcon), typeof(ImageSource), typeof(TabbedItem));

        public ImageSource HeaderIcon
        {
            get => (ImageSource)GetValue(HeaderIconProperty);
            set { SetValue(HeaderIconProperty, value); }
        }

        public readonly BindableProperty HeaderIconSizeProperty = BindableProperty.Create(nameof(HeaderIconSize), typeof(double), typeof(TabbedItem), 32.0);

        public double HeaderIconSize
        {
            get => (double)GetValue(HeaderIconSizeProperty);
            set { SetValue(HeaderIconSizeProperty, value); }
        }

        public static readonly BindableProperty HeaderTextProperty = BindableProperty.Create(nameof(HeaderText), typeof(string), typeof(TabbedItem), string.Empty);

        public string HeaderText
        {
            get => (string)GetValue(HeaderTextProperty);
            set { SetValue(HeaderTextProperty, value); }
        }

        public static readonly BindableProperty ContentProperty = BindableProperty.Create(nameof(Content), typeof(View), typeof(TabbedItem));

        public View Content
        {
            get => (View)GetValue(ContentProperty);
            set { SetValue(ContentProperty, value); }
        }

        public static readonly BindableProperty IsCurrentProperty = BindableProperty.Create(nameof(IsCurrent), typeof(bool), typeof(TabbedItem), false);

        public bool IsCurrent
        {
            get => (bool)GetValue(IsCurrentProperty);
            set { SetValue(IsCurrentProperty, value); }
        }

        public static readonly BindableProperty HeaderTextColorProperty = BindableProperty.Create(nameof(HeaderTextColor), typeof(Color), typeof(TabbedItem), Color.White);

        public Color HeaderTextColor
        {
            get => (Color)GetValue(HeaderTextColorProperty);
            set { SetValue(HeaderTextColorProperty, value); }
        }

        public static readonly BindableProperty HeaderSelectionUnderlineColorProperty = BindableProperty.Create(nameof(HeaderSelectionUnderlineColor), typeof(Color), typeof(TabbedItem), Color.White);

        public Color HeaderSelectionUnderlineColor
        {
            get => (Color)GetValue(HeaderSelectionUnderlineColorProperty);
            set { SetValue(HeaderSelectionUnderlineColorProperty, value); }
        }

        public static readonly BindableProperty HeaderSelectionUnderlineThicknessProperty = BindableProperty.Create(nameof(HeaderSelectionUnderlineThickness), typeof(double), typeof(TabbedItem), (double)5);

        public double HeaderSelectionUnderlineThickness
        {
            get => (double)GetValue(HeaderSelectionUnderlineThicknessProperty);
            set { SetValue(HeaderSelectionUnderlineThicknessProperty, value); }
        }

        public static readonly BindableProperty HeaderSelectionUnderlineWidthProperty = BindableProperty.Create(nameof(HeaderSelectionUnderlineWidth), typeof(double), typeof(TabbedItem), (double)40);

        public double HeaderSelectionUnderlineWidth
        {
            get => (double)GetValue(HeaderSelectionUnderlineWidthProperty);
            set { SetValue(HeaderSelectionUnderlineWidthProperty, value); }
        }

        public static readonly BindableProperty HeaderTabTextFontSizeProperty = BindableProperty.Create(nameof(HeaderTabTextFontSize), typeof(double), typeof(TabbedItem), TabDefaults.DefaultTextSize);

        [TypeConverter(typeof(FontSizeConverter))]
        public double HeaderTabTextFontSize
        {
            get => (double)GetValue(HeaderTabTextFontSizeProperty);
            set { SetValue(HeaderTabTextFontSizeProperty, value); }
        }

        public static readonly BindableProperty HeaderTabTextFontFamilyProperty = BindableProperty.Create(nameof(HeaderTabTextFontFamily), typeof(string), typeof(TabbedItem));

        public string HeaderTabTextFontFamily
        {
            get => (string)GetValue(HeaderTabTextFontFamilyProperty);
            set { SetValue(HeaderTabTextFontFamilyProperty, value); }
        }

        public static readonly BindableProperty HeaderTabTextFontAttributesProperty = BindableProperty.Create(nameof(HeaderTabTextFontAttributes), typeof(FontAttributes), typeof(TabbedItem), FontAttributes.None);

        public FontAttributes HeaderTabTextFontAttributes
        {
            get => (FontAttributes)GetValue(HeaderTabTextFontAttributesProperty);
            set { SetValue(HeaderTabTextFontAttributesProperty, value); }
        }
    }
}