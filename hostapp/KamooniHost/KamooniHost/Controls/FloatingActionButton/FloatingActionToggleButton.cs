using Xamarin.Forms;

namespace Xamarin.Forms.Controls
{
    public class FloatingActionToggleButton : FloatingActionButton
    {
        public static readonly BindableProperty CheckedProperty = BindableProperty.Create(nameof(Checked), typeof(bool), typeof(FloatingActionToggleButton), false);

        public static readonly BindableProperty CheckedColorProperty = BindableProperty.Create(nameof(CheckedColor), typeof(Color), typeof(FloatingActionToggleButton), Color.Blue);

        public static readonly BindableProperty UnCheckedColorProperty = BindableProperty.Create(nameof(UnCheckedColor), typeof(Color), typeof(FloatingActionToggleButton), Color.White);

        public static readonly BindableProperty CheckedImageProperty = BindableProperty.Create(nameof(CheckedImage), typeof(ImageSource), typeof(FloatingActionToggleButton), null);

        public static readonly BindableProperty UnCheckedImageProperty = BindableProperty.Create(nameof(UnCheckedImage), typeof(ImageSource), typeof(FloatingActionToggleButton), null);

        public bool Checked
        {
            get { return (bool)GetValue(CheckedProperty); }
            set { SetValue(CheckedProperty, value); }
        }

        public Color CheckedColor
        {
            get { return (Color)GetValue(CheckedColorProperty); }
            set { SetValue(CheckedColorProperty, value); }
        }

        public Color UnCheckedColor
        {
            get { return (Color)GetValue(UnCheckedColorProperty); }
            set { SetValue(UnCheckedColorProperty, value); }
        }

        [TypeConverter(typeof(ImageSourceConverter))]
        public ImageSource CheckedImage
        {
            get { return (ImageSource)GetValue(CheckedImageProperty); }
            set { SetValue(CheckedImageProperty, value); }
        }

        [TypeConverter(typeof(ImageSourceConverter))]
        public ImageSource UnCheckedImage
        {
            get { return (ImageSource)GetValue(UnCheckedImageProperty); }
            set { SetValue(UnCheckedImageProperty, value); }
        }

        public override void SendClicked()
        {
            if (ImageSource == UnCheckedImage)
            {
                ImageSource = CheckedImage;
                NormalColor = CheckedColor;
                Checked = true;
            }
            else
            {
                ImageSource = UnCheckedImage;
                NormalColor = UnCheckedColor;
                Checked = false;
            }

            base.SendClicked();
        }

        protected override void OnParentSet()
        {
            base.OnParentSet();

            ImageSource = UnCheckedImage;
            NormalColor = UnCheckedColor;
        }
    }
}