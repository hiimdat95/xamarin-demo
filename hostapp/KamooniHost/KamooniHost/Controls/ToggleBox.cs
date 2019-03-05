using System.Threading.Tasks;
using System.Windows.Input;

namespace Xamarin.Forms.Controls
{
    public class ToggleBox : ContentView
    {
        public static readonly BindableProperty CheckedProperty = BindableProperty.Create("Checked", typeof(bool), typeof(ToggleBox), false, BindingMode.TwoWay);

        public static readonly BindableProperty AnimateProperty = BindableProperty.Create("Animate", typeof(bool), typeof(ToggleBox), false);

        public static readonly BindableProperty EnabledProperty = BindableProperty.Create("Enabled", typeof(bool), typeof(ToggleBox), false);

        public static readonly BindableProperty CheckedImageProperty = BindableProperty.Create("CheckedImage", typeof(ImageSource), typeof(ToggleBox), null);

        public static readonly BindableProperty UnCheckedImageProperty = BindableProperty.Create("UnCheckedImage", typeof(ImageSource), typeof(ToggleBox), null);

        public static readonly BindableProperty CommandProperty = BindableProperty.Create("Command", typeof(ICommand), typeof(ToggleBox), null);

        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create("CommandParameter", typeof(object), typeof(ToggleBox), null);

        public ToggleBox()
        {
            Initialize();
        }

        public bool Checked
        {
            get { return (bool)GetValue(CheckedProperty); }
            set { SetValue(CheckedProperty, value); }
        }

        public bool Enabled
        {
            get { return (bool)GetValue(EnabledProperty); }
            set { SetValue(EnabledProperty, value); }
        }

        public bool Animate
        {
            get { return (bool)GetValue(AnimateProperty); }
            set { SetValue(AnimateProperty, value); }
        }

        public ImageSource CheckedImage
        {
            get { return (ImageSource)GetValue(CheckedImageProperty); }
            set { SetValue(CheckedImageProperty, value); }
        }

        public ImageSource UnCheckedImage
        {
            get { return (ImageSource)GetValue(UnCheckedImageProperty); }
            set { SetValue(UnCheckedImageProperty, value); }
        }

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        private Image _toggleImage;

        public ICommand TogleCommand { get; private set; }

        private void Initialize()
        {
            TogleCommand = new Command(() =>
            {
                if (!Enabled)
                {
                    return;
                }

                Checked = _toggleImage.Source == UnCheckedImage;

                if (Animate)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        this.ScaleTo(0.8, 50, Easing.Linear);
                        Task.Delay(100);
                        this.ScaleTo(1, 50, Easing.Linear);
                    });
                }
                if (Command != null)
                {
                    Command.Execute(CommandParameter);
                }
            });

            Animate = true;

            GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = TogleCommand
            });

            _toggleImage = new Image
            {
                Source = UnCheckedImage,
                Aspect = Aspect.Fill,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand
            };
            var stk = new StackLayout
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };
            stk.Children.Add(_toggleImage);

            Frame fr = new Frame()
            {
                BackgroundColor = Color.White,
                BorderColor = Color.Gray,
                HasShadow = false,
                Content = stk
            };

            Content = fr;
        }

        protected override void OnParentSet()
        {
            base.OnParentSet();

            _toggleImage.Source = UnCheckedImage;
            Content = _toggleImage;
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged();

            if (propertyName == null)
                return;

            if (Equals(propertyName, "Checked"))
            {
                _toggleImage.Source = Equals(Checked, true) ? CheckedImage : UnCheckedImage;
            }
        }
    }
}