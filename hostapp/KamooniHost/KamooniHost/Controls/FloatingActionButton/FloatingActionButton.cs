using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace Xamarin.Forms.Controls
{
    public class FloatingActionButton : View
    {
        public static readonly BindableProperty SizeProperty = BindableProperty.Create(nameof(Size), typeof(FabSize), typeof(FloatingActionButton), FabSize.Normal);

        public static readonly BindableProperty NormalColorProperty = BindableProperty.Create(nameof(NormalColor), typeof(Color), typeof(FloatingActionButton), Color.DodgerBlue);

        public static readonly BindableProperty PressedColorProperty = BindableProperty.Create(nameof(PressedColor), typeof(Color), typeof(FloatingActionButton), Color.White);

        public static readonly BindableProperty RippleColorProperty = BindableProperty.Create(nameof(RippleColor), typeof(Color), typeof(FloatingActionButton), Color.White);

        public static readonly BindableProperty DisabledColorProperty = BindableProperty.Create(nameof(DisabledColor), typeof(Color), typeof(FloatingActionButton), Color.Gray);

        public static readonly BindableProperty HasShadowProperty = BindableProperty.Create(nameof(HasShadow), typeof(bool), typeof(FloatingActionButton), true);

        public static readonly BindableProperty ImageSourceProperty = BindableProperty.Create(nameof(ImageSource), typeof(ImageSource), typeof(FloatingActionButton), null);

        public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(FloatingActionButton), null, propertyChanged: (b, o, n) => ((FloatingActionButton)b).OnCommandChanged());

        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(FloatingActionButton), null, propertyChanged: (b, o, n) => ((FloatingActionButton)b).OnCommandCanExecuteChanged(b, EventArgs.Empty));

        public static readonly BindableProperty AnimateOnSelectionProperty = BindableProperty.Create(nameof(AnimateOnSelection), typeof(bool), typeof(FloatingActionButton), true);

        public event EventHandler<EventArgs> Clicked;

        public FabSize Size
        {
            get { return (FabSize)GetValue(SizeProperty); }
            set { SetValue(SizeProperty, value); }
        }

        public Color NormalColor
        {
            get { return (Color)GetValue(NormalColorProperty); }
            set { SetValue(NormalColorProperty, value); }
        }

        public Color PressedColor
        {
            get { return (Color)GetValue(PressedColorProperty); }
            set { SetValue(PressedColorProperty, value); }
        }

        public Color RippleColor
        {
            get { return (Color)GetValue(RippleColorProperty); }
            set { SetValue(RippleColorProperty, value); }
        }

        public Color DisabledColor
        {
            get { return (Color)GetValue(DisabledColorProperty); }
            set { SetValue(DisabledColorProperty, value); }
        }

        public bool HasShadow
        {
            get { return (bool)GetValue(HasShadowProperty); }
            set { SetValue(HasShadowProperty, value); }
        }

        [TypeConverter(typeof(ImageSourceConverter))]
        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
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

        public bool AnimateOnSelection
        {
            get { return (bool)GetValue(AnimateOnSelectionProperty); }
            set { SetValue(AnimateOnSelectionProperty, value); }
        }

        public virtual void SendClicked()
        {
            var param = CommandParameter;

            if (Command != null && Command.CanExecute(param))
            {
                Command.Execute(param);
            }

            Clicked?.Invoke(this, EventArgs.Empty);
        }

        private void OnCommandChanged()
        {
            if (Command != null)
            {
                Command.CanExecuteChanged += OnCommandCanExecuteChanged;
                OnCommandCanExecuteChanged(this, EventArgs.Empty);
            }
            else
                IsEnabled = true;
        }

        private void OnCommandCanExecuteChanged(object sender, EventArgs eventArgs)
        {
            ICommand cmd = Command;
            if (cmd != null)
                IsEnabled = cmd.CanExecute(CommandParameter);
        }

        public delegate void AttachToListViewDelegate(ListView listView);

        public delegate void ShowHideDelegate(bool animate = true);

        public ShowHideDelegate Show { get; set; }

        public ShowHideDelegate Hide { get; set; }
    }
}